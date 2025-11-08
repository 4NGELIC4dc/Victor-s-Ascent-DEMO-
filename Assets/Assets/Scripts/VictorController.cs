using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class VictorController : MonoBehaviour
{
    [Header("Movement")]
    private float horizontal;
    private bool isFacingRight = true;
    public float moveSpeed = 7f;
    public float jumpingPower = 16f;

    [Header("Jump Assistance")]
    public float coyoteTime = 0.15f;
    private float coyoteTimeCounter;
    public float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;

    [Header("Dash")]
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;
    private bool canDash = true;
    private bool isDashing;

    [Header("Wall Hold / Jump")]
    public float wallSlideSpeed = 2f;
    public Vector2 wallJumpingPower = new Vector2(8f, 16f);
    private bool isWallHold;
    private bool isWallJumping;
    private float wallJumpingDirection;
    public float wallJumpingDuration = 0.4f;
    private float wallJumpCounter;

    [Header("Death")]
    private bool isDead;
    [SerializeField] private LayerMask harmfulLayer;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    private InputActionMap playerActionMap;
    private InputSystemActions inputActions;

    // Prevents wrong animation on first frame before physics settles
    private bool startupSettled = false;

    // Animator parameter caching
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
        inputActions.Player.Enable();

        // Cache animator parameter hashes for performance
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
        SnapToGround();
        yield return new WaitForFixedUpdate();

        startupSettled = true;

        // Wait one more frame to let the animator fully initialize
        yield return null;

        ResetAllAnimatorBools();
        animator.Rebind();
        animator.Update(0f);

        UpdateAnimator();
    }
    private void Update()
    {
        if (isDead) return;

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        horizontal = moveInput.x;

        // ---- Jump Buffer and Coyote Time ----
        if (IsGrounded())
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        if (jumpAction.WasPressedThisFrame())
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            Jump();
            jumpBufferCounter = 0f;
        }

        if (jumpAction.WasReleasedThisFrame() && rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);

        if (dashAction.WasPressedThisFrame() && canDash)
            StartCoroutine(Dash());

        DetectWallHold();
        HandleWallJump();

        if (!isWallJumping)
            Flip();

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (isDead || isDashing) return;

        if (isWallHold)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
        }
        else if (!isWallJumping)
        {
            // Smooth horizontal movement to prevent invisible snagging
            float targetVelX = horizontal * moveSpeed;
            Vector2 newVelocity = rb.linearVelocity;
            newVelocity.x = Mathf.MoveTowards(newVelocity.x, targetVelX, moveSpeed * Time.fixedDeltaTime * 10f);

            // Small threshold to avoid micro-stuck states
            if (Mathf.Abs(newVelocity.x) < 0.05f) newVelocity.x = 0f;

            rb.linearVelocity = new Vector2(newVelocity.x, rb.linearVelocity.y);
        }
    }

    // ---------------- MOVEMENT ACTIONS ----------------

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        coyoteTimeCounter = 0f;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        animator.SetBool(hashIsDashing, true);

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

    private void DetectWallHold()
    {
        bool touchingWall = Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
        bool pressingTowardWall = (horizontal > 0 && isFacingRight) || (horizontal < 0 && !isFacingRight);

        if (!IsGrounded() && touchingWall && pressingTowardWall && rb.linearVelocity.y < 0f)
        {
            isWallHold = true;
        }
        else
        {
            isWallHold = false;
        }
    }

    private void HandleWallJump()
    {
        if (isWallHold)
        {
            wallJumpCounter = 0.2f;
        }
        else
        {
            wallJumpCounter -= Time.deltaTime;
        }

        if (jumpAction.WasPressedThisFrame() && wallJumpCounter > 0f)
        {
            isWallJumping = true;
            wallJumpCounter = 0f;

            wallJumpingDirection = -transform.localScale.x;
            rb.linearVelocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping() => isWallJumping = false;

    private void Die()
    {
        if (isDead) return; // Prevent double-trigger
        isDead = true;

        // Stop all movement
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Disable collisions so enemies can't push him
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Play death animation
        animator.SetBool(hashIsDead, true);

        // Optional: You can later add a respawn or restart logic here
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & harmfulLayer) != 0)
        {
            Die();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Border"))
        {
            // Stop horizontal movement to prevent going out of bounds
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            // Optionally, snap him slightly inward (so he doesn’t stick)
            Vector3 pos = transform.position;
            if (collision.transform.position.x < pos.x)
                pos.x += 0.05f;
            else
                pos.x -= 0.05f;
            transform.position = pos;
        }
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

    // ---------------- ANIMATOR SYNC ----------------

    private void UpdateAnimator()
    {
        if (animator == null) return;
        if (!startupSettled) return;

        bool grounded = IsGrounded();
        bool running = Mathf.Abs(horizontal) > 0.1f && grounded;
        bool jumping = rb.linearVelocity.y > 0.1f && !grounded;
        bool falling = rb.linearVelocity.y < -0.1f && !grounded;

        animator.SetBool(hashIsRunning, running);
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
        {
            transform.position = new Vector2(transform.position.x, hit.point.y + 0.1f);
        }
    }

    private void OnDestroy()
    {
        inputActions?.Dispose();
    }
}
