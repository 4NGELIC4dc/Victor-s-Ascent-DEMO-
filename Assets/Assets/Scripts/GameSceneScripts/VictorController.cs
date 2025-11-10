using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class VictorController : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource jumpSFX;
    [SerializeField] private AudioSource runSFX;
    [SerializeField] private AudioSource dashSFX;
    [SerializeField] private AudioSource hitSFX;

    [Header("Movement")]
    private float horizontal;
    private bool isFacingRight = true;
    public float moveSpeed = 7f;
    public float jumpingPower = 16f;

    [Header("Dash")]
    public float dashingPower = 12f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;
    private bool canDash = true;
    private bool isDashing;

    [Header("Wall Hold / Jump")]
    private bool isWallHold;
    private bool isWallJumping;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask chainLayer;
    [SerializeField] private float wallCheckDistance = 0.3f;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite wallHoldSprite;
    [SerializeField] private Sprite normalSprite;

    [Header("Death")]
    private bool isDead;
    [SerializeField] private LayerMask harmfulLayer;

    [Header("Interaction")]
    [SerializeField] private float interactRange = 1.5f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Transform interactionPoint;

    private InputAction interactAction;  // for F
    private InputAction pickupAction;    // for E

    [Header("Spawn / Respawn")]
    [SerializeField] private Transform spawnPoint;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private InputSystemActions inputActions;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;

    private bool startupSettled = false;
    private float normalGravity;

    [HideInInspector] public bool hasRope = false;

    // Animator parameter hashing
    private int hashIsRunning;
    private int hashIsGrounded;
    private int hashIsJumping;
    private int hashIsFalling;
    private int hashIsWallHold;
    private int hashIsDashing;
    private int hashIsDead;
    private int hashYVelocity;
    private int hashMoveSpeed;

    private void Awake()
    {
        inputActions = new InputSystemActions();
        moveAction = inputActions.Player.Move;
        jumpAction = inputActions.Player.Jump;
        dashAction = inputActions.Player.Dash;
        interactAction = inputActions.Player.Interact;
        pickupAction = inputActions.Player.Pickup;
        inputActions.Player.Enable();

        hashIsRunning = Animator.StringToHash("isRunning");
        hashIsGrounded = Animator.StringToHash("isGrounded");
        hashIsJumping = Animator.StringToHash("isJumping");
        hashIsFalling = Animator.StringToHash("isFalling");
        hashIsWallHold = Animator.StringToHash("isWallHold");
        hashIsDashing = Animator.StringToHash("isDashing");
        hashIsDead = Animator.StringToHash("isDead");
        hashYVelocity = Animator.StringToHash("yVelocity");
        hashMoveSpeed = Animator.StringToHash("moveSpeed");
    }

    private IEnumerator Start()
    {
        normalGravity = rb.gravityScale;
        SnapToGround();
        yield return new WaitForFixedUpdate();
        startupSettled = true;

        yield return null;
        ResetAllAnimatorBools();
        animator.Rebind();
        animator.Update(0f);
        UpdateAnimator();
    }

    private void Update()
    {
        if (pickupAction.WasPressedThisFrame())
        {
            TryPickUp();
        }

        if (interactAction.WasPressedThisFrame())
        {
            TryInteract();
        }


        if (isDead) return;

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        horizontal = moveInput.x;

        if (isDashing) return;

        DetectWallHold();

        // Wall Jump
        if (isWallHold && jumpAction.WasPressedThisFrame())
        {
            isWallHold = false;
            rb.gravityScale = normalGravity;

            // push away from wall (diagonally)
            float jumpDir = isFacingRight ? -1f : 1f;
            float wallJumpXPower = moveSpeed * 1.1f; // horizontal strength
            float wallJumpYPower = jumpingPower * 0.9f; // vertical boost
            rb.linearVelocity = new Vector2(jumpDir * wallJumpXPower, wallJumpYPower);

            animator.SetBool(hashIsWallHold, false);
            animator.SetBool(hashIsJumping, true);
            animator.Play("Victor_Jump");

            jumpSFX.Play();
            Debug.Log("[WHD] Wall Jump triggered");
        }

        // Wall Dash (diagonal push)
        if (isWallHold && dashAction.WasPressedThisFrame() && canDash)
        {
            // Exit wall hold
            isWallHold = false;
            rb.gravityScale = normalGravity;
            animator.SetBool(hashIsWallHold, false);

            // Optional dash animation
            animator.Play("Victor_Dash");

            float dashDir = isFacingRight ? -1f : 1f; // dash away from the wall
            float horizontalPower = dashingPower * 0.9f;
            float verticalPower = dashingPower * 0.6f;

            // Apply diagonal burst
            rb.linearVelocity = new Vector2(dashDir * horizontalPower, verticalPower);

            if (spriteRenderer && normalSprite)
                spriteRenderer.sprite = normalSprite;

            Debug.Log("[WHD] Wall Dash triggered (diagonal)!");

            dashSFX.Play();
            StartCoroutine(WallDashCooldown());
        }

        if (jumpAction.WasPressedThisFrame())
        {
            if (isWallHold)
            {
                isWallHold = false;
                rb.gravityScale = normalGravity;

                float jumpDir = isFacingRight ? -1f : 1f;
                rb.linearVelocity = new Vector2(jumpDir * moveSpeed * 0.8f, jumpingPower);

                animator.SetBool(hashIsWallHold, false);
                animator.SetBool(hashIsJumping, true);

                if (spriteRenderer && normalSprite)
                    spriteRenderer.sprite = normalSprite;

                jumpSFX.Play();
                Debug.Log("[WHD] Wall jump triggered");
            }
            else if (IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
                jumpSFX.Play();
                animator.SetBool(hashIsJumping, true);
            }
        }

        if (dashAction.WasPressedThisFrame() && canDash)
        {
            if (isWallHold)
            {
                isWallHold = false;
                rb.gravityScale = normalGravity;

                if (spriteRenderer && normalSprite)
                    spriteRenderer.sprite = normalSprite;

                animator.SetBool(hashIsWallHold, false);
                Debug.Log("[WHD] Dashing off wall!");
            }
            StartCoroutine(Dash());
        }

        if (!isWallJumping && !isWallHold)
            Flip();

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (isDead || isDashing) return;

        if (isWallHold)
        {
            // Stop sliding down
            rb.linearVelocity = new Vector2(0f, 0f);
            rb.gravityScale = 0f;

            // Only freeze rotation, not position
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            return;
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // allow normal movement
        }

        if (!isWallJumping)
        {
            float targetVelX = horizontal * moveSpeed;
            Vector2 newVelocity = rb.linearVelocity;
            newVelocity.x = Mathf.MoveTowards(newVelocity.x, targetVelX, moveSpeed * Time.fixedDeltaTime * 10f);
            rb.linearVelocity = new Vector2(newVelocity.x, rb.linearVelocity.y);
        }
    }

    private void DetectWallHold()
    {
        float direction = isFacingRight ? 1f : -1f;
        Vector2 origin = wallCheck.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * direction, wallCheckDistance, chainLayer);

        // Draw ray in Scene view (to check wall detection)
        Debug.DrawRay(origin, Vector2.right * direction * wallCheckDistance, hit ? Color.green : Color.red);

        // Add debug block
        if (hit.collider)
        {
            Debug.Log("[WHD] Hit chain: " + hit.collider.name);
        }
        else
        {
            Debug.Log("[WHD] No chain detected.");
        }

        bool touchingChain = hit.collider != null;

        if (touchingChain && !IsGrounded())
        {
            if (!isWallHold)
            {
                isWallHold = true;
                rb.gravityScale = 0f;
                rb.linearVelocity = Vector2.zero;

                // Force static sprite for wallhold
                if (spriteRenderer && wallHoldSprite)
                    spriteRenderer.sprite = wallHoldSprite;

                Debug.Log("[WHD] Wallhold sprite forced");
            }

            rb.linearVelocity = Vector2.zero;
        }
        else if (isWallHold)
        {
            isWallHold = false;
            rb.gravityScale = normalGravity;

            // Re-enable animator and restore normal sprite
            if (animator) animator.enabled = true;
            if (spriteRenderer && normalSprite)
                spriteRenderer.sprite = normalSprite;

            Debug.Log("[WHD] Wallhold released, animator restored");
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        animator.SetBool(hashIsDashing, true);
        dashSFX.Play();

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2((isFacingRight ? 1 : -1) * dashingPower, 0f);

        yield return new WaitForSeconds(dashingTime);

        rb.gravityScale = originalGravity;
        isDashing = false;
        animator.SetBool(hashIsDashing, false);

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
    private IEnumerator WallDashCooldown()
    {
        canDash = false;
        isDashing = true;

        yield return new WaitForSeconds(dashingTime);

        rb.gravityScale = normalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        hitSFX.Play();

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        animator.SetBool(hashIsDead, true);

        // Respawn after delay
        StartCoroutine(HandleDeathSequence());
    }
    private IEnumerator HandleDeathSequence()
    {
        // find fade controller in scene
        FadeController fade = FindAnyObjectByType<FadeController>();

        // Wait for death animation
        yield return new WaitForSeconds(1.5f);

        // Fade to black
        if (fade != null)
            yield return fade.FadeOut();

        // Respawn player
        yield return RespawnAfterDelay(0f); // instant reposition

        // Small delay before fading back in
        yield return new WaitForSeconds(0.3f);

        // Fade back in
        if (fade != null)
            yield return fade.FadeIn();
    }
    private IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        isDead = false;
        animator.SetBool(hashIsDead, false);

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = normalGravity;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
            Debug.Log("[Respawn] Victor respawned at spawn point.");
        }
        else
        {
            Debug.LogWarning("[Respawn] No spawn point assigned!");
        }

        FindAnyObjectByType<GameManager>()?.ResetLevel();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & harmfulLayer) != 0)
            Die();
    }

    private void Flip()
    {
        if ((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }

    private void UpdateAnimator()
    {
        if (!animator || !startupSettled) return;

        bool grounded = IsGrounded();
        bool jumping = rb.linearVelocity.y > 0.1f && !grounded;
        bool falling = rb.linearVelocity.y < -0.1f && !grounded;

        if (isWallHold)
        {
            jumping = false;
            falling = false;
        }

        animator.SetBool(hashIsRunning, Mathf.Abs(horizontal) > 0.1f && grounded);
        animator.SetBool(hashIsGrounded, grounded);
        animator.SetBool(hashIsJumping, jumping);
        animator.SetBool(hashIsFalling, falling);
        animator.SetBool(hashIsWallHold, isWallHold);
        animator.SetBool(hashIsDead, isDead);
        animator.SetFloat(hashYVelocity, rb.linearVelocity.y);
        animator.SetFloat(hashMoveSpeed, Mathf.Abs(rb.linearVelocity.x));
    }

    private void ResetAllAnimatorBools()
    {
        animator.SetBool(hashIsRunning, false);
        animator.SetBool(hashIsGrounded, false);
        animator.SetBool(hashIsJumping, false);
        animator.SetBool(hashIsFalling, false);
        animator.SetBool(hashIsWallHold, false);
        animator.SetBool(hashIsDashing, false);
        animator.SetBool(hashIsDead, false);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void SnapToGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2f, groundLayer);
        if (hit)
            transform.position = new Vector2(transform.position.x, hit.point.y + 0.1f);
    }
    private void TryPickUp()
    {
        Collider2D hit = Physics2D.OverlapCircle(interactionPoint.position, interactRange, interactableLayer);
        if (hit != null)
        {
            RopePickup rope = hit.GetComponent<RopePickup>();
            if (rope != null)
            {
                rope.Interact();
                hasRope = true; // player now owns the rope
                Debug.Log("[Interaction] Picked up rope!");
                return;
            }
        }
        Debug.Log("[Interaction] No item to pick up.");
    }

    private void TryInteract()
    {
        Collider2D hit = Physics2D.OverlapCircle(interactionPoint.position, interactRange, interactableLayer);
        if (hit != null)
        {
            WindowInteractable window = hit.GetComponent<WindowInteractable>();
            if (window != null)
            {
                window.Interact();
                Debug.Log("[Interaction] Interacted with window!");
                return;
            }
        }
        Debug.Log("[Interaction] No interactable object nearby.");
    }

    private void OnDestroy()
    {
        if (inputActions != null)
        {
            inputActions.Player.Disable();
            inputActions.Dispose();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (wallCheck != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * (isFacingRight ? 1 : -1) * wallCheckDistance);
        }
    }

    private void OnDrawGizmos()
    {
        if (interactionPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(interactionPoint.position, interactRange);
        }
    }
}
