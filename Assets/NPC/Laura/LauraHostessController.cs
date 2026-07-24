using UnityEngine;

public class LauraHostessController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NPC npc;
    [SerializeField] private WaypointMover waypointMover;

    [Header("Dialogue")]
    [SerializeField] private NPCDialogue entranceDialogue;
    [SerializeField] private NPCDialogue tableDialogue;

    private bool isAtTable = false;

    private void Start()
    {
        if (npc == null)
        {
            npc = GetComponent<NPC>();
        }

        if (waypointMover == null)
        {
            waypointMover = GetComponent<WaypointMover>();
        }

        if (npc != null && entranceDialogue != null)
        {
            npc.dialogueData = entranceDialogue;
        }
    }

    public void HandleDialogueEnded()
    {
        if (!isAtTable)
        {
            // Entrance dialogue finished.
            // Walk to the table.
            waypointMover.StartMoving();
        }
        else
        {
            // Table dialogue finished.
            // Continue to the next waypoint: entrance.
            waypointMover.StartMoving();
        }
    }

    public void HandleWaypointReached()
    {
        if (!isAtTable)
        {
            // First waypoint is the table.
            isAtTable = true;

            npc.dialogueData = tableDialogue;

            Debug.Log("Laura reached the table. Table dialogue is now active.");
        }
        else
        {
            // Second waypoint is the entrance.
            isAtTable = false;

            Debug.Log("Laura returned to the entrance.");
        }
    }
}