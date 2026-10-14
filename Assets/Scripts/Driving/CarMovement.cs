
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private bool isDriving = true;

    [Header("Turning Waypoints")]
    [SerializeField] private Transform[] rightTurnWaypoints;
    [SerializeField] private Transform[] leftTurnWaypoints;

    [Header("Smooth Right Turn")]
    [SerializeField] private Transform rightTurnEnd;
    [SerializeField] private float turnDuration = 1.2f;

    private Vector2 currentDirection = Vector2.up;

    private bool isTurning = false;
    private bool isCurvingRight = false;
    private bool waitingForDirections = false;

    private Transform[] activeWaypoints;
    private int waypointIndex = 0;

    private Vector2 directionAfterTurn;

    private Vector3 curveStart;
    private Vector3 curveControl;
    private Vector3 curveEnd;
    private float turnProgress = 0f;

    private void Update()
    {
        if (isCurvingRight)
        {
            FollowRightTurnCurve();
            return;
        }

        if (isTurning)
        {
            FollowWaypoints();
            return;
        }

        if (!isDriving)
            return;

        transform.position +=
            (Vector3)currentDirection * moveSpeed * Time.deltaTime;
    }

    // Smooth right turn using a quadratic Bezier curve.
    private void FollowRightTurnCurve()
    {
        turnProgress += Time.deltaTime /
            Mathf.Max(0.01f, turnDuration);

        float t = Mathf.Clamp01(turnProgress);
        float u = 1f - t;

        Vector3 position =
            u * u * curveStart +
            2f * u * t * curveControl +
            t * t * curveEnd;

        transform.position = position;

        // Rotate the Uber along the curve's tangent.
        Vector3 tangent =
            2f * u * (curveControl - curveStart) +
            2f * t * (curveEnd - curveControl);

        if (tangent.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(
                tangent.y,
                tangent.x
            ) * Mathf.Rad2Deg - 90f;

            transform.rotation =
                Quaternion.Euler(0f, 0f, angle);
        }

        if (t >= 1f)
        {
            isCurvingRight = false;

            currentDirection = Vector2.right;

            transform.rotation =
                Quaternion.Euler(0f, 0f, -90f);

            isDriving = true;
        }
    }

    // Existing waypoint movement for left turns.
    private void FollowWaypoints()
    {
        if (activeWaypoints == null ||
            waypointIndex >= activeWaypoints.Length)
        {
            FinishTurn();
            return;
        }

        Transform target = activeWaypoints[waypointIndex];

        if (target == null)
            return;

        Vector3 targetPosition = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        Vector3 lookDirection =
            targetPosition - transform.position;

        if (waypointIndex + 1 < activeWaypoints.Length)
        {
            Transform nextWaypoint =
                activeWaypoints[waypointIndex + 1];

            if (nextWaypoint != null)
            {
                Vector3 nextDirection =
                    nextWaypoint.position - targetPosition;

                float distance = Vector3.Distance(
                    transform.position,
                    targetPosition
                );

                float blend = Mathf.Clamp01(
                    1f - distance / 0.35f
                );

                lookDirection = Vector3.Lerp(
                    lookDirection.normalized,
                    nextDirection.normalized,
                    blend
                );
            }
        }

        if (lookDirection.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(
                lookDirection.y,
                lookDirection.x
            ) * Mathf.Rad2Deg - 90f;

            Quaternion targetRotation =
                Quaternion.Euler(0f, 0f, angle);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        if (Vector3.Distance(
            transform.position,
            targetPosition
        ) < 0.01f)
        {
            waypointIndex++;
        }
    }

    public void TurnRight()
    {
        if (!waitingForDirections ||
            isTurning ||
            isCurvingRight ||
            rightTurnEnd == null)
            return;

        waitingForDirections = false;
        isDriving = true;

        // Begin at the Uber's actual position.
        curveStart = transform.position;

        // Finish at the assigned waypoint.
        curveEnd = rightTurnEnd.position;
        curveEnd.z = curveStart.z;

        // Control point creates a north-to-east curve.
        curveControl = new Vector3(
            curveStart.x,
            curveEnd.y,
            curveStart.z
        );

        turnProgress = 0f;
        isCurvingRight = true;
    }

    public void TurnLeft()
    {
        StartTurn(leftTurnWaypoints, Vector2.left);
    }

    private void StartTurn(
        Transform[] waypoints,
        Vector2 finalDirection
    )
    {
        if (isTurning ||
            isCurvingRight ||
            !waitingForDirections ||
            waypoints == null ||
            waypoints.Length == 0)
            return;

        activeWaypoints = waypoints;
        directionAfterTurn = finalDirection;

        waypointIndex = 0;
        isTurning = true;
        waitingForDirections = false;
        isDriving = true;
    }

    private void FinishTurn()
    {
        isTurning = false;
        activeWaypoints = null;

        currentDirection = directionAfterTurn;

        float angle = Mathf.Atan2(
            currentDirection.y,
            currentDirection.x
        ) * Mathf.Rad2Deg - 90f;

        transform.rotation =
            Quaternion.Euler(0f, 0f, angle);

        isDriving = true;
    }

    public void GoStraight()
    {
        if (!waitingForDirections)
            return;

        waitingForDirections = false;
        isDriving = true;
    }

    public void StopDriving()
    {
        if (isTurning || isCurvingRight)
            return;

        isDriving = false;
        waitingForDirections = true;
    }

    public void ResumeDriving()
    {
        waitingForDirections = false;
        isDriving = true;
    }

    // Temporary testing buttons.
    private void OnGUI()
    {
        if (!waitingForDirections)
            return;

        if (GUI.Button(
            new Rect(20, 20, 120, 45),
            "Turn Left"))
        {
            TurnLeft();
        }

        if (GUI.Button(
            new Rect(150, 20, 120, 45),
            "Go Straight"))
        {
            GoStraight();
        }

        if (GUI.Button(
            new Rect(280, 20, 120, 45),
            "Turn Right"))
        {
            TurnRight();
        }
    }
}
