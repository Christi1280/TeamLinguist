using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Moves an NPC through a sequence of waypoints.
/// Handles movement and facing animations, player obstacle detection,
/// waypoint arrival events, optional waiting, and reusable path controls.
/// </summary>

public class WaypointMover : MonoBehaviour
{
    [Header("Waypoint Settings")]
    public Transform waypointParent;
    public float moveSpeed = 2f;
    public float waitTime = 0f;

    [Header("Movement Behaviour")]
    public bool startAutomatically = true;
    public bool loopWaypoints = true;

    [Header("Obstacle Avoidance")]
    public float obstacleCheckDistance = 0.5f;
    public float obstacleCheckRadius = 0.25f;

    [Header("Movement Events")]
    public UnityEvent onFirstWaypointReached;
    public UnityEvent onWaypointReached;
    public UnityEvent onPathCompleted;

    private Transform[] waypoints;
    private int currentWaypointIndex;

    private bool isWaiting;
    private bool isMoving;

    private Animator animator;

    private float lastInputX;
    private float lastInputY;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (waypointParent == null)
        {
            Debug.LogWarning(
                $"{gameObject.name} has no waypoint parent assigned."
            );

            return;
        }

        // Build the waypoint path from the children of the assigned parent.
        waypoints = new Transform[waypointParent.childCount];

        for (int i = 0; i < waypointParent.childCount; i++)
        {
            waypoints[i] = waypointParent.GetChild(i);
        }

        isMoving = startAutomatically;
    }

    private void Update()
    {
        if (!isMoving ||
            isWaiting ||
            PauseController.IsGamePaused)
        {
            StopWalkingAnimation();
            return;
        }

        if (waypoints == null || waypoints.Length == 0)
        {
            return;
        }

        MoveTowardsCurrentWaypoint();
    }

    private void MoveTowardsCurrentWaypoint()
    {
        Transform target = waypoints[currentWaypointIndex];

        // Get a normalized direction from the NPC toward the waypoint.
        Vector2 direction =
            (target.position - transform.position).normalized;

        // Remember the last movement direction for the idle facing animation.
        if (direction.sqrMagnitude > 0.001f)
        {
            lastInputX = direction.x;
            lastInputY = direction.y;
        }

        // Wait for player to move out of the way
        if (IsPlayerAhead(direction))
        {
            StopWalkingAnimation();
            return;
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        if (animator != null)
        {
            animator.SetFloat("InputX", direction.x);
            animator.SetFloat("InputY", direction.y);

            animator.SetBool(
                "IsWalking",
                direction.sqrMagnitude > 0.001f
            );
        }


        // Close enough to count as reaching the waypoint.
        if (Vector2.Distance(
                transform.position,
                target.position) < 0.1f)
        {
            StartCoroutine(HandleWaypointReached());
        }
    }

    private bool IsPlayerAhead(Vector2 direction)
    {
        if (direction.sqrMagnitude <= 0.001f)
        {
            return false;
        }

        // Check a short area ahead of the NPC for the player.
        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            transform.position,
            obstacleCheckRadius,
            direction,
            obstacleCheckDistance
        );

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null &&
                hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator HandleWaypointReached()
    {
        // Prevent Update from handling the same waypoint again
        // while the arrival coroutine is still running.
        isWaiting = true;

        int reachedWaypointIndex = currentWaypointIndex;

        ApplyWaypointFacing(
            waypoints[reachedWaypointIndex]
        );

        StopWalkingAnimation();

        if (waitTime > 0f)
        {
            yield return new WaitForSeconds(waitTime);
        }

        isMoving = false;

        // Allow Inspector events to respond specifically to waypoint 0.
        if (reachedWaypointIndex == 0)
        {
            onFirstWaypointReached?.Invoke();
        }

        // Fires whenever any waypoint is reached.
        onWaypointReached?.Invoke();

        bool isLastWaypoint =
            reachedWaypointIndex >= waypoints.Length - 1;

        if (isLastWaypoint)
        {
            onPathCompleted?.Invoke();

            if (loopWaypoints)
            {
                currentWaypointIndex = 0;
            }
        }
        else
        {
            currentWaypointIndex++;
        }

        isWaiting = false;
    }

    private void ApplyWaypointFacing(Transform waypoint)
    {
        WaypointFacing waypointFacing =
            waypoint.GetComponent<WaypointFacing>();

        if (waypointFacing == null)
        {
            return;
        }

        // Use the direction assigned to this waypoint as the NPC's
        // facing direction after arriving.
        Vector2 facingDirection =
            waypointFacing.GetDirection();

        lastInputX = facingDirection.x;
        lastInputY = facingDirection.y;

        if (animator != null)
        {
            animator.SetFloat(
                "InputX",
                facingDirection.x
            );

            animator.SetFloat(
                "InputY",
                facingDirection.y
            );

            animator.SetFloat(
                "LastInputX",
                facingDirection.x
            );

            animator.SetFloat(
                "LastInputY",
                facingDirection.y
            );
        }
    }

    public void StartMoving()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning(
                $"{gameObject.name} has no waypoints."
            );

            return;
        }

        isMoving = true;
    }

    public void StopMoving()
    {
        isMoving = false;
        StopWalkingAnimation();
    }

    public void RestartPath()
    {
        currentWaypointIndex = 0;
        isMoving = true;
    }

    public void MoveToWaypoint(int waypointIndex)
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning(
                $"{gameObject.name} has no waypoints."
            );

            return;
        }
        // Prevent an invalid array index from being used as the target.
        if (waypointIndex < 0 ||
            waypointIndex >= waypoints.Length)
        {
            Debug.LogWarning(
                $"Waypoint index {waypointIndex} is invalid."
            );

            return;
        }

        currentWaypointIndex = waypointIndex;
        isMoving = true;
    }

    private void StopWalkingAnimation()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetBool("IsWalking", false);

        // Keep the NPC facing its last movement direction while idle.
        animator.SetFloat("LastInputX", lastInputX);
        animator.SetFloat("LastInputY", lastInputY);
    }
}