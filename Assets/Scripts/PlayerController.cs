using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rigidBody;
    private float horizontalInput;
    private bool jumpPressed;
    private bool isGrounded;

    private bool isFacingRight = true;
    private Animator animator;
    private string currentState;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        ReadInput();
        Flip();
        UpdateAnimations();
    }

    void ReadInput()
    {
        Keyboard keyboard = Keyboard.current;
        if(keyboard == null)
        {
            return;
        }

        // Read movement input.
        horizontalInput = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            horizontalInput = -1f;
        }
            
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            horizontalInput = +1f;
        }

        // Record jump input in Update so a quick press
        // is not missed between physics updates.
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            jumpPressed = true;
        }

    }

    void FixedUpdate()
    {
        // Control horizontal movement.
        rigidBody.linearVelocity = new Vector2(horizontalInput * moveSpeed, rigidBody.linearVelocity.y);

        // Check whether the player is on the ground.
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        //Debug.Log("grounded?: " + isGrounded);

        // Jump only when grounded.
        if (jumpPressed && isGrounded)
        {
            rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, jumpForce);
        }

        // Consume the jump press after checking it.
        jumpPressed = false;
    }

    private void Flip()
    {
        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    private void UpdateAnimations()
    {
        if (horizontalInput != 0)
        {
            //animator.Play("Player_Run");
            ChangeAnimation("Player_Run");
        }
        else
        {
            //animator.Play("Player_Idle");

            ChangeAnimation("Player_Idle");
        }
    }

    
    private void ChangeAnimation(string newState)
    {
        if(currentState  == newState)
        {
            return;
        }
        
        animator.Play(newState);
        currentState = newState;        
    }
    
}
