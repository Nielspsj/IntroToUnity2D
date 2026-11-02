using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Coyote Time")]
    public float coyoteTime = 0.15f;
    private float coyoteTimeCountDown;

    private Rigidbody2D rigidBody;
    private float horizontalInput;
    private bool jumpPressed;
    private bool isGrounded;

    private bool isFacingRight = true;
    private Animator animator;
    private string currentState;
    private bool isDead = false;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        //Are we dead? Then the rest of Update doesn't run and we can't move.
        if(isDead == true)
        {
            return;
        }

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

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            horizontalInput = -1f;
        }
            
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
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
        //Are we dead? Then the rest of FixedUpdate doesn't run and we stop physics working on us.
        if (isDead == true)
        {
            return;
        }

        // Control horizontal movement.
        rigidBody.linearVelocity = new Vector2(horizontalInput * moveSpeed, rigidBody.linearVelocity.y);

        // Check whether the player is on the ground.
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        //Debug.Log("grounded?: " + isGrounded);

        // Refresh coyote time while grounded
        if (isGrounded)
        {
            coyoteTimeCountDown = coyoteTime;
        }
        else
        {
            // Count down while in the air
            coyoteTimeCountDown -= Time.fixedDeltaTime;
        }

        // Jump if we press jump and some coyote time remains
        if (jumpPressed && coyoteTimeCountDown > 0f)
        {
            rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, jumpForce);

            // Consume coyote time so we cannot jump again
            coyoteTimeCountDown = 0f;
        }

        // Consume the jump press after checking it.
        jumpPressed = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Are we dead? Can't collide with stuff if dead.
        if (isDead == true)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("collision to enemy");

            StartCoroutine(PlayerDeath());
        }
    }

    private void Flip()
    {
        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    
    //Play death animation
    //Restart level?
    private IEnumerator PlayerDeath()
    {
        //Are we dead? Clear stored inputs and stop the players movement.
        if (isDead == true)
        {
            yield break;
        }

        //Debug.Log("Death");
        isDead = true;

        // Clear any stored input.
        horizontalInput = 0f;
        jumpPressed = false;

        // Stop the player's movement.
        rigidBody.linearVelocity = Vector2.zero;
        // gravity to 0 if you don't want the player to fall while dying.
        //rigidBody.gravityScale = 0f;

        //Death animation
        ChangeAnimation("Player_Death");

        //animator.Play("Player_Death");
        yield return new WaitForSeconds(1);
        
        //Restart level if player death. Destroy object if not player
       
       
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
        if (!isGrounded)
        {
            if (rigidBody.linearVelocityY > 0f)
            {
                ChangeAnimation("Player_Jump");
            }
            else if (rigidBody.linearVelocityY < 0f)
            {
                ChangeAnimation("Player_Fall");
            }
        }
        else if (horizontalInput != 0)
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
