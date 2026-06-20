using System.Collections;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    public Transform waypointParent;
    public float moveSpeed = 2f;
    public float waitTime = 2f;
    public bool loopWaypoints = true;

    private Transform[] waypoints;
    private int currentWaypointIndex;
    private bool isWaiting;
    private Animator animator;

    private float lastInputX;
    private float lastInputY;
    
    void Start()
    {
        animator = GetComponent<Animator>();    
        waypoints = new Transform[waypointParent.childCount];

        for(int i = 0; i < waypointParent.childCount; i++)
        {
            waypoints[i] = waypointParent.GetChild(i);
        }
    }

    
    void Update()
    {
        if (PauseController.IsGamePaused || isWaiting)
        {
            animator.SetBool("IsWalking", false);
            animator.SetFloat("LastInputX", lastInputX);
            animator.SetFloat("LastInputY", lastInputY);
            return;
        }

        MoveToWaypoint();
    }
    
    void MoveToWaypoint()
    {
        Transform target = waypoints[currentWaypointIndex];
        Vector2 direction = (target.position - transform.position).normalized;

        if (direction.magnitude > 0f)
        {
            lastInputX = direction.x;
            lastInputY = direction.y;
        }

        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        animator.SetFloat("InputX", direction.x);
        animator.SetFloat("InputY", direction.y);
        animator.SetBool("IsWalking", direction.magnitude > 0f);

        if(Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        animator.SetBool("IsWalking", false);

        animator.SetFloat("LastInputX", lastInputX);
        animator.SetFloat("LastInputY", lastInputY);
        yield return new WaitForSeconds(waitTime);

        //if looping is enabled: increment currentWaypointIndex and wrap around if needed
        //if not looping: increment currentWaypointIndex but dont exceed last waypoint
        currentWaypointIndex = loopWaypoints ? (currentWaypointIndex + 1) % waypoints.Length : Mathf.Min(currentWaypointIndex + 1, waypoints.Length - 1);

        isWaiting = false;
    }
}
