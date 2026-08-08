using UnityEngine;

public class LauraHostessController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NPC npc;
    [SerializeField] private WaypointMover waypointMover;
    [SerializeField] private SeatInteraction seatInteraction;

    // Laura's dedicated circular interaction trigger.
    [SerializeField] private CircleCollider2D interactionRange;

    [Header("Interaction Range Sizes")]
    [SerializeField] private float entranceInteractionRadius = 1.5f;
    [SerializeField] private float tableInteractionRadius = 0.4f;

    [Header("Dialogue")]
    [SerializeField] private NPCDialogue entranceDialogue;
    [SerializeField] private NPCDialogue tableDialogue;

    private bool isAtTable;
    private bool playerHasSatDown;

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

        // Laura begins at the hostess table with the larger range.
        if (interactionRange != null)
        {
            interactionRange.radius = entranceInteractionRadius;
            interactionRange.enabled = true;
        }

        // The player cannot use the chair yet.
        if (seatInteraction != null)
        {
            seatInteraction.DisableSitting();
        }
    }

    public void HandleDialogueEnded()
    {
        if (!isAtTable)
        {
            // DOOR CONVERSATION FINISHED.

            // The player should now follow Laura.
            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.SetFollowLauraObjective();
            }

            // Disable Laura's interaction area while she walks.
            if (interactionRange != null)
            {
                interactionRange.enabled = false;
            }

            // Laura begins walking to the table.
            if (waypointMover != null)
            {
                waypointMover.StartMoving();
            }

            Debug.Log(
                "Laura is leading the player to the table."
            );
        }
        else
        {
            // TABLE CONVERSATION FINISHED.

            // The player's next objective is to sit down.
            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.SetTakeASeatObjective();
            }

            // Laura waits here while the player sits.
            if (interactionRange != null)
            {
                interactionRange.enabled = false;
            }

            if (seatInteraction != null)
            {
                seatInteraction.EnableSitting();
            }

            Debug.Log(
                "Laura is waiting for the player to sit down."
            );
        }
    }

    public void HandleWaypointReached()
    {
        if (!isAtTable)
        {
            // Laura reached the player's table.
            isAtTable = true;

            // Switch Laura to her table conversation.
            if (npc != null && tableDialogue != null)
            {
                npc.dialogueData = tableDialogue;
            }

            /*
             * Use the smaller interaction range at the table so it
             * does not overlap too much with the seat interaction.
             */
            if (interactionRange != null)
            {
                interactionRange.radius = tableInteractionRadius;
                interactionRange.enabled = true;
            }

            Debug.Log(
                "Laura reached the table. Table dialogue is active."
            );
        }
        else
        {
            // Laura returned to the hostess table.
            isAtTable = false;

            if (npc != null && entranceDialogue != null)
            {
                npc.dialogueData = entranceDialogue;
            }

            // Restore Laura's larger entrance interaction range.
            if (interactionRange != null)
            {
                interactionRange.radius = entranceInteractionRadius;
                interactionRange.enabled = true;
            }

            Debug.Log("Laura returned to the entrance.");
        }
    }

    public void NotifyPlayerSatDown()
    {
        playerHasSatDown = true;

        if (seatInteraction != null)
        {
            seatInteraction.DisableSitting();
        }

        Debug.Log(
            "The player sat down. Loading the seated scene."
        );
    }
}