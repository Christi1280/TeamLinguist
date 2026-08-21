using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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

        Vector2 direction =
            (target.position - transform.position).normalized;

        if (direction.sqrMagnitude > 0.001f)
        {
            lastInputX = direction.x;
            lastInputY = direction.y;
        }

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

        // Only fires when waypoint 0 is reached.
        if (reachedWaypointIndex == 0)
        {
            onFirstWaypointReached?.Invoke();
        }

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
        animator.SetFloat("LastInputX", lastInputX);
        animator.SetFloat("LastInputY", lastInputY);
    }
}