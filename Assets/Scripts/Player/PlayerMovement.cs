using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// PlayerMovement receives movement input from Unity's Input System, applies that movement to the
/// player's Rigidbody2D, updates the Animator with the player's movement/facing direction,
/// stops movement while paused, and controls the player's repeating footstep sounds.
/// </summary>

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private bool playingFootsteps = false;
    public float footstepSpeed = 0.5f;

  
    void Start()
    {
        // Get the player's movement and animation components.
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

  
    void Update()
    {
        if (PauseController.IsGamePaused)
        {
            // Prevent movement, walking animations, and footsteps while paused.
            rb.linearVelocity = Vector2.zero; 
            animator.SetBool("IsWalking", false);
            StopFootsteps();
            return;
        }
        // Move the player using the direction received from the Input System.
        rb.linearVelocity = moveInput * moveSpeed;
        animator.SetBool("IsWalking", rb.linearVelocity.magnitude > 0);

        //Determine when to play footsteps
        if (rb.linearVelocity.magnitude > 0 && !playingFootsteps)
        {
            StartFootsteps();
        } else if (rb.linearVelocity.magnitude == 0)
        {
            StopFootsteps();
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        // Save the last movement direction so the idle animation
        // continues facing the direction the player stopped in.
        if (context.canceled)
        {
            animator.SetBool("IsWalking", false);
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
        }
        moveInput = context.ReadValue<Vector2>();
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
    }

    void StartFootsteps()
    {
        playingFootsteps = true;
        // Continue playing footsteps at set intervals until movement stops.
        InvokeRepeating(nameof(PlayFootstep), 0, footstepSpeed);
    }

    void PlayFootstep()
    {
        SoundEffectManager.Play("Footstep", true);
    }
    void StopFootsteps()
    {
        playingFootsteps = false;
        CancelInvoke(nameof(PlayFootstep));
    }
}
