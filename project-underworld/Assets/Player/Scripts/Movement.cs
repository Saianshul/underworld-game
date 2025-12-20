using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rigidBody;
    private Animator animator;

    [Header("Horizontal Movement Settings")]
    [SerializeField] private float moveSpeed = 7f;
    private float horizontalMove;
    private bool isFacingRight;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 14f;
    private bool isJumping;

    [Header("Colliders")]
    [SerializeField] private Collider2D horizontalCapsuleCollider;
    [SerializeField] private Collider2D verticalCapsuleCollider;
    
    [Header("Ground Check Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckHeight = 0.1f;
    [SerializeField] private float groundCheckDistance = 0.05f;
    private bool isGrounded;
    private bool wasGrounded;

    private bool isAttacking;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        isFacingRight = true;
        isJumping = false;
        isGrounded = true;
        wasGrounded = true;
        isAttacking = false;
    }

    private void Update()
    {
        animator.SetFloat("speed", Mathf.Abs(horizontalMove));
        animator.SetFloat("yVelocity", rigidBody.linearVelocityY);
        
        if (horizontalMove < 0 && isFacingRight)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(-2, 2, 2);
        }
        else if (horizontalMove > 0 && !isFacingRight)
        {
            isFacingRight = true;
            transform.localScale = new Vector3(2, 2, 2);
        }
    }

    private void FixedUpdate()
    {
        rigidBody.linearVelocityX = horizontalMove * moveSpeed;

        Collider2D activeCollider = horizontalCapsuleCollider.gameObject.activeSelf ? horizontalCapsuleCollider : verticalCapsuleCollider;
        Bounds colliderBounds = activeCollider.bounds;
        Vector2 boxCenter = new Vector2(colliderBounds.center.x, colliderBounds.min.y);
        RaycastHit2D hit = Physics2D.BoxCast(boxCenter, new Vector2(colliderBounds.size.x * 0.9f, groundCheckHeight), 0f, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;
        animator.SetBool("isGrounded", isGrounded);

        if (isGrounded && !wasGrounded)
        {
            isJumping = false;
        }

        wasGrounded = isGrounded;
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        horizontalMove = context.ReadValue<float>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && !isJumping)
        {
            animator.SetTrigger("jump");
            isJumping = true;
        }
    }

    public void ApplyJumpForce()
    {
        rigidBody.linearVelocityY = jumpForce;
    }

    public void OnForwardSlash(InputAction.CallbackContext context)
    {
        if (context.performed && !isAttacking && isGrounded && !isJumping)
        {
            animator.SetTrigger("attack");
            isAttacking = true;
        }
    }

    public void FinishAttack()
    {
        isAttacking = false;
    }
}
