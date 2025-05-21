using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int playerIndex = 0;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveInput;
    private bool facingRight = true;
    private Animator animator;
    private PowerupManager powerupManager;
    private bool isInAir = false;
    private Vector2 facingDirection = Vector2.right;

    private string horizontalAxis;
    private string jumpButton;
    private string forcePushButton;
    private string shieldButton;
    private string airJumpButton;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        powerupManager = FindObjectOfType<PowerupManager>();

        // Set up input based on player index
        SetupPlayerControls();
    }

    private void SetupPlayerControls()
    {
        // For player 1
        if (playerIndex == 0)
        {
            horizontalAxis = "Horizontal";
            jumpButton = "Jump";
            forcePushButton = "Fire1";
            shieldButton = "Fire2";
            airJumpButton = "Fire3";
        }
        // For player 2
        else if (playerIndex == 1)
        {
            horizontalAxis = "HorizontalP2";
            jumpButton = "JumpP2";
            forcePushButton = "Fire1P2";
            shieldButton = "Fire2P2";
            airJumpButton = "Fire3P2";
        }
        // Add more players as needed
    }

    private void Update()
    {
        // Check if grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Track if player is in air
        if (!isGrounded && rb.linearVelocity.y < 0)
        {
            isInAir = true;
        }
        else if (isGrounded)
        {
            isInAir = false;
        }

        // Get movement input
        moveInput = Input.GetAxis(horizontalAxis);

        // Update facing direction
        if (moveInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && facingRight)
        {
            Flip();
        }

        // Jump input
        if (Input.GetButtonDown(jumpButton) && isGrounded)
        {
            Jump();
        }

        // Air jump
        if (Input.GetButtonDown(airJumpButton) && isInAir)
        {
            TryAirJump();
        }

        // Powerup inputs
        if (Input.GetButtonDown(forcePushButton))
        {
            powerupManager.UsePowerup(playerIndex, PowerupType.ForcePush);
        }

        if (Input.GetButtonDown(shieldButton))
        {
            powerupManager.UsePowerup(playerIndex, PowerupType.Shield);
        }

        // Update animation states
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
        }
    }

    private void FixedUpdate()
    {
        // Move the player
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        if (animator != null)
        {
            animator.SetTrigger("Jump");
        }
    }

    private void TryAirJump()
    {
        if (powerupManager.UseAirJump(playerIndex))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 1.2f);
            if (animator != null)
            {
                animator.SetTrigger("AirJump");
            }
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;

        // Update facing direction
        facingDirection = facingRight ? Vector2.right : Vector2.left;
    }

    public int GetPlayerIndex()
    {
        return playerIndex;
    }

    public Vector2 GetFacingDirection()
    {
        return facingDirection;
    }

    public void AddForce(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public bool IsShieldActive()
    {
        return powerupManager.IsShieldActive(playerIndex);
    }
}