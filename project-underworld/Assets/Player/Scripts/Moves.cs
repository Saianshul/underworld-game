using UnityEngine;
using UnityEngine.InputSystem;

public class Moves : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private Animator animator;

    [Header("Horizontal Movement Settings")]
    [SerializeField] private float moveSpeed;
    private Vector2 moveInput;
    private bool isFacingRight;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float maxJumpKeyDownDuration;
    [SerializeField] private float somersaultThreshold;
    private bool isJumping;
    private float jumpKeyDownDuration;

    [Header("Colliders")]
    [SerializeField] private Collider2D horizontalCapsuleCollider;
    [SerializeField] private Collider2D verticalCapsuleCollider;
    [SerializeField] private Collider2D forwardSlashPolygonCollider;
    [SerializeField] private Collider2D forwardAirSlashPolygonCollider;
    [SerializeField] private Collider2D downAirSlashPolygonCollider;
    [SerializeField] private Collider2D upAirSlashPolygonCollider;
    [SerializeField] private Collider2D castPolygonCollider;
    
    [Header("Ground Check Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckHeight;
    [SerializeField] private float groundCheckDistance;
    private bool isGrounded;
    private bool wasGrounded;

    private bool isAttacking;
    private bool isCasting;

    [Header("Spell Settings")]
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float castCooldown;
    private float prevCastTime;

    private const float PLAYER_X_SCALE = 2;
    private const float PLAYER_Y_SCALE = 2;
    private const float PLAYER_Z_SCALE = 2;
    private const float GROUND_CHECK_WIDTH_REDUCING_FACTOR = 0.9f;
    private const float AIR_SLASH_Y_VELOCITY_THRESHOLD = 0.1f;
    private const float DEFAULT_GRAVITY_SCALE = 3;
    private const float CAST_GRAVITY_SCALE = 0.01f;
    private const float CAST_Y_OFFSET = -0.1f;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        isFacingRight = true;
        isJumping = false;
        jumpKeyDownDuration = 0;
        isGrounded = true;
        wasGrounded = true;
        isAttacking = false;
        isCasting = false;
        prevCastTime = -1;
    }

    private void Update()
    {
        animator.SetFloat("speed", Mathf.Abs(moveInput.x));
        animator.SetFloat("yVelocity", rigidBody.linearVelocityY);
        
        if (!isCasting)
        {
            if (moveInput.x < 0 && isFacingRight)
            {
                isFacingRight = false;
                transform.localScale = new Vector3(-PLAYER_X_SCALE, PLAYER_Y_SCALE, PLAYER_Z_SCALE);
            }
            else if (moveInput.x > 0 && !isFacingRight)
            {
                isFacingRight = true;
                transform.localScale = new Vector3(PLAYER_X_SCALE, PLAYER_Y_SCALE, PLAYER_Z_SCALE);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isCasting)
        {
            rigidBody.linearVelocity = Vector2.zero;
        }
        else
        {
            rigidBody.linearVelocityX = moveInput.x * moveSpeed;
        }

        Collider2D activeCollider;

        if (horizontalCapsuleCollider.gameObject.activeSelf)
        {
            activeCollider = horizontalCapsuleCollider;
        }
        else if (verticalCapsuleCollider.gameObject.activeSelf)
        {
            activeCollider = verticalCapsuleCollider;
        }
        else if (forwardSlashPolygonCollider.gameObject.activeSelf)
        {
            activeCollider = forwardSlashPolygonCollider;
        }
        else if (forwardAirSlashPolygonCollider.gameObject.activeSelf)
        {
            activeCollider = forwardAirSlashPolygonCollider;
        }
        else if (downAirSlashPolygonCollider.gameObject.activeSelf)
        {
            activeCollider = downAirSlashPolygonCollider;
        }
        else if (upAirSlashPolygonCollider.gameObject.activeSelf)
        {
            activeCollider = upAirSlashPolygonCollider;
        }
        else
        {
            activeCollider = castPolygonCollider;
        }

        Bounds colliderBounds = activeCollider.bounds;
        Vector2 boxCenter = new Vector2(colliderBounds.center.x, colliderBounds.min.y);
        RaycastHit2D hit = Physics2D.BoxCast(boxCenter, new Vector2(colliderBounds.size.x * GROUND_CHECK_WIDTH_REDUCING_FACTOR, groundCheckHeight), 0, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;

        animator.SetBool("isGrounded", isGrounded);

        if (isGrounded && !wasGrounded)
        {
            isJumping = false;
            jumpKeyDownDuration = 0;
        }

        wasGrounded = isGrounded;

        if (isJumping)
        {
            if (jumpKeyDownDuration < maxJumpKeyDownDuration)
            {
                rigidBody.linearVelocityY = jumpForce;
                jumpKeyDownDuration += Time.deltaTime;

                if (jumpKeyDownDuration >= somersaultThreshold)
                {
                    animator.SetBool("doSomersault", true);
                }
            }
            else
            {
                isJumping = false;
            }
        }
    }

    public void FinishSomersault()
    {
        animator.SetBool("doSomersault", false);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded && !isJumping && !isAttacking)
        {
            isJumping = true;
            rigidBody.linearVelocityY = jumpForce;

            animator.SetTrigger("jump");
        }

        if (context.canceled)
        {
            isJumping = false;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !isAttacking)
        {
            isAttacking = true;

            if (moveInput.y < 0 && !isGrounded && Mathf.Abs(rigidBody.linearVelocityY) > AIR_SLASH_Y_VELOCITY_THRESHOLD)
            {
                animator.SetTrigger("pogo");
            }
            else if (moveInput.y > 0 && !isGrounded && Mathf.Abs(rigidBody.linearVelocityY) > AIR_SLASH_Y_VELOCITY_THRESHOLD)
            {
                animator.SetTrigger("upAir");
            }
            else
            {
                animator.SetTrigger("fSlashes");
            }
        }
    }

    public void OnCast(InputAction.CallbackContext context)
    {
        if (context.performed && !isAttacking && Time.time >= prevCastTime + castCooldown)
        {
            prevCastTime = Time.time;
            animator.SetTrigger("cast");
        }
    }

    public void StartCast()
    {
        isAttacking = true;
        isCasting = true;
        isJumping = false;
        rigidBody.gravityScale = CAST_GRAVITY_SCALE;
        transform.position += new Vector3(0, CAST_Y_OFFSET, 0);
    }

    public void CastSpell()
    {
        GameObject spell = Instantiate(spellPrefab, firePoint.position, firePoint.rotation);
        float direction = transform.localScale.x > 0 ? 1f : -1f;
        spell.GetComponent<Spell>().Cast(direction);
    }

    public void FinishAttack()
    {
        isAttacking = false;
        isCasting = false;
        rigidBody.gravityScale = DEFAULT_GRAVITY_SCALE;
        animator.ResetTrigger("fSlashes");
        animator.ResetTrigger("pogo");
        animator.ResetTrigger("upAir");
        animator.ResetTrigger("cast");
    }
}
