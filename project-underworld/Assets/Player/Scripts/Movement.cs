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
    
    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    private bool isGrounded;
    private bool wasGrounded;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        isFacingRight = true;
        wasGrounded = true;
    }

    private void Update()
    {
        animator.SetFloat("speed", Mathf.Abs(horizontalMove));
        animator.SetFloat("yVelocity", rigidBody.linearVelocityY);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        animator.SetBool("isGrounded", isGrounded);

        if (isGrounded && !wasGrounded)
        {
            isJumping = false;
        }

        wasGrounded = isGrounded;
        
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

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
