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
            // --------------------------------
            // ENTRANCE CONVERSATION FINISHED
            // --------------------------------

            // Add Laura's first conversation phrases
            // to the player's permanent journal.
            if (GameProgressManager.Instance != null)
            {
                GameProgressManager.Instance.LearnPhrases(
                    entranceDialogue
                );
            }

            // Refresh the Journal UI.
            JournalManager journalManager =
                FindFirstObjectByType<JournalManager>();

            if (journalManager != null)
            {
                journalManager.RefreshJournal();
            }

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
                "Laura's entrance conversation completed. " +
                "Phrases learned and Laura is leading the player to the table."
            );
        }
        else
        {
            // --------------------------------
            // TABLE CONVERSATION FINISHED
            // --------------------------------

            if (GameProgressManager.Instance != null)
            {
                // Diagnostic information so we can confirm
                // which dialogue asset Unity is using.
                Debug.Log(
                    "TABLE DIALOGUE: " +
                    (tableDialogue != null
                        ? tableDialogue.name
                        : "NULL")
                );

                // Check how many key phrases Unity sees
                // on Laura's table dialogue.
                Debug.Log(
                    "TABLE KEY PHRASE COUNT: " +
                    (tableDialogue != null &&
                     tableDialogue.keyPhrases != null
                        ? tableDialogue.keyPhrases.Length
                        : -1)
                );

                // Print every key phrase Unity finds.
                if (tableDialogue != null &&
                    tableDialogue.keyPhrases != null)
                {
                    foreach (KeyPhrase phrase
                             in tableDialogue.keyPhrases)
                    {
                        Debug.Log(
                            "TABLE PHRASE FOUND: " +
                            phrase.spanish +
                            " = " +
                            phrase.english
                        );
                    }
                }

                // Add Laura's table phrase(s)
                // to the permanent journal.
                GameProgressManager.Instance.LearnPhrases(
                    tableDialogue
                );

                // Laura's full interaction is now complete.
                GameProgressManager.Instance.SetModuleProgress(60);
            }
            else
            {
                Debug.LogWarning(
                    "LauraHostessController: " +
                    "GameProgressManager was not found."
                );
            }

            // Refresh the Journal so the new
            // table phrase appears immediately.
            JournalManager journalManager =
                FindFirstObjectByType<JournalManager>();

            if (journalManager != null)
            {
                journalManager.RefreshJournal();
            }

            // The player's next objective is to sit down.
            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.SetTakeASeatObjective();
            }

            // Refresh the module progress UI.
            if (ModuleProgressManager.Instance != null)
            {
                ModuleProgressManager.Instance.UpdateProgressUI();
            }

            // Laura should no longer be interactable
            // while the summary is displayed.
            if (interactionRange != null)
            {
                interactionRange.enabled = false;
            }

            // The chair is now available.
            if (seatInteraction != null)
            {
                seatInteraction.EnableSitting();
            }

            // Show one summary containing phrases from
            // both of Laura's conversations.
            if (ConversationSummaryManager.Instance != null)
            {
                ConversationSummaryManager.Instance.ShowLauraSummary();
            }

            Debug.Log(
                "Laura's full interaction completed. " +
                "Module Progress: 60%. " +
                "New objective: TAKE A SEAT."
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

            Debug.Log(
                "Laura returned to the entrance."
            );
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