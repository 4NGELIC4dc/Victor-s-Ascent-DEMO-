using UnityEngine;
using UnityEngine.InputSystem;

public class VictorController : MonoBehaviour
{
    [Header("Movement")]
    private float horizontal;
    private bool isFacingRight = true;
    private bool canDoubleJump;
    public float moveSpeed = 7f;
    public float jumpingPower = 16f;

    [Header("Dashing")]
    private bool canDash = true;
    private bool isDashing;
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;

    [Header("Wall Hold")]
    private bool isWallHolding;
    public float wallHoldSlideSpeed = 2f;

    [Header("Wall Jumping")]
    private bool isWallJumping;
    private float wallJumpingDirection;
    public float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    public float wallJumpingDuration = 0.4f;
    public Vector2 wallJumpingPower = new Vector2(8f, 16f);

    [Header("Death")]
    private bool isDead;
    [SerializeField] private LayerMask harmfulLayer;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Animator animator;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    private InputAction collectAction;
    private InputActionMap playerActionMap;

    private @InputSystemActions inputActions;

    private void Awake()
    {
        inputActions = new @InputSystemActions();

        // Access actions through the PlayerActions struct wrapper
        moveAction = inputActions.@Player.@Move;
        jumpAction = inputActions.@Player.@Jump;
        dashAction = inputActions.@Player.@Dash;
        collectAction = inputActions.@Player.@Collect;

        inputActions.@Player.Enable();
    }

    private void Update()
    {
        if (isDashing || isDead)
            return;

        horizontal = moveAction.ReadValue<float>();

        if (jumpAction.WasPressedThisFrame())
        {
            if (IsGrounded())
                Jump();
            else if (canDoubleJump)
                DoubleJump();
        }

        if (jumpAction.WasReleasedThisFrame() && rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);

        if (dashAction.WasPressedThisFrame() && canDash)
            StartCoroutine(Dash());

        if (collectAction.WasPressedThisFrame())
            TryCollect();

        if (IsGrounded())
            canDoubleJump = true;

        DetectWallHold();
        WallJump();

        if (!isWallJumping)
            Flip();

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (isDashing || isDead)
            return;

        if (isWallHolding)
            rb.linearVelocity = new Vector2(horizontal * moveSpeed * 0.3f, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        canDoubleJump = true;
    }

    private void DoubleJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        canDoubleJump = false;
    }

    private System.Collections.IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);

        yield return new WaitForSeconds(dashingTime);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private void DetectWallHold()
    {
        if (IsGrounded())
        {
            isWallHolding = false;
            return;
        }

        bool isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);

        if (isTouchingWall && !isWallJumping)
        {
            isWallHolding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallHoldSlideSpeed, float.MaxValue));
        }
        else
        {
            isWallHolding = false;
        }
    }

    private void WallJump()
    {
        if (isWallHolding)
        {
            isWallJumping = false;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (jumpAction.WasPressedThisFrame() && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = 0f;

            CancelInvoke(nameof(StopWallJumping));
            Invoke(nameof(StopWallJumping), wallJumpingDuration);

            rb.linearVelocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void TryCollect()
    {
        // TODO: Implement collectible interaction logic here
        // This will check for nearby collectibles and pick them up
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & harmfulLayer) != 0 && ((1 << collision.gameObject.layer) & groundLayer) == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        animator.SetBool("IsDead", true);
        // TODO: Add additional death effects (animation, sound, particle effects, etc.)
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void UpdateAnimator()
    {
        if (animator == null)
            return;

        animator.SetBool("IsGrounded", IsGrounded());
        animator.SetBool("IsWallHolding", isWallHolding);
        animator.SetBool("IsDashing", isDashing);
        animator.SetFloat("Speed", Mathf.Abs(horizontal));
    }

    private bool IsGrounded() => Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

    private void OnDestroy()
    {
        inputActions?.Dispose();
    }
}
