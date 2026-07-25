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
    public UnityEvent onWaypointReached;
    public UnityEvent onPathCompleted;

    private Transform[] waypoints;
    private int currentWaypointIndex;

    private bool isWaiting;
    private bool isMoving;

    private Animator animator;

    private float lastInputX;
    private float lastInputY;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (waypointParent == null)
        {
            Debug.LogWarning($"{gameObject.name} has no waypoint parent assigned.");
            return;
        }

        waypoints = new Transform[waypointParent.childCount];

        for (int i = 0; i < waypointParent.childCount; i++)
        {
            waypoints[i] = waypointParent.GetChild(i);
        }

        isMoving = startAutomatically;
    }

    void Update()
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

        MoveToWaypoint();
    }

    private void MoveToWaypoint()
    {
        Transform target = waypoints[currentWaypointIndex];

        Vector2 direction =
            (target.position - transform.position).normalized;

        if (direction.sqrMagnitude > 0.001f)
        {
            lastInputX = direction.x;
            lastInputY = direction.y;
        }

        // Stop if the player is directly in front of this NPC.
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

        StopWalkingAnimation();

        if (waitTime > 0f)
        {
            yield return new WaitForSeconds(waitTime);
        }

        // Stop whenever a waypoint is reached.
        isMoving = false;

        onWaypointReached?.Invoke();

        bool isLastWaypoint =
            currentWaypointIndex >= waypoints.Length - 1;

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
            // Prepare the next waypoint,
            // but don't start moving until StartMoving() is called again.
            currentWaypointIndex++;
        }

        isWaiting = false;
    }

    public void StartMoving()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning($"{gameObject.name} has no waypoints.");
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