using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class DiegoWaiterController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NPC npc;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private WaypointMover waypointMover;
    [SerializeField] private GameObject soupOnTable;
    [SerializeField] private ScreenFader screenFader;

    [Header("Dialogue")]
    [SerializeField] private NPCDialogue diegoDialogue1;
    [SerializeField] private NPCDialogue diegoDialogue2;

    [Header("Kitchen Return")]
    [SerializeField] private float kitchenWaitTime = 2f;

    [Header("Ending")]
    [SerializeField] private float soupDisplayTime = 2f;

    [Header("Cutscene")]
    [SerializeField] private PlayableDirector cutsceneDirector;

    [Header("After Soup Delivery")]
    [SerializeField] private float departureDelay = 0.5f;
    private bool cutsceneTriggered;
    private bool menuHasBeenShown;
    private bool returningWithFood;

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

        if (npc != null && diegoDialogue1 != null)
        {
            npc.dialogueData = diegoDialogue1;
        }

        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }

        if (soupOnTable != null)
        {
            soupOnTable.SetActive(false);
        }
    }

    public void HandleDialogueEnded()
    {
        // Dialogue 1 ended.
        if (!menuHasBeenShown)
        {
            // Save vocabulary from Diego's first conversation.
            LearnDiegoPhrases(diegoDialogue1);

            menuHasBeenShown = true;

            if (npc != null && diegoDialogue2 != null)
            {
                npc.dialogueData = diegoDialogue2;
            }

            if (menuPanel != null)
            {
                menuPanel.SetActive(true);
            }

            return;
        }

        // Dialogue 2 ended.
        LearnDiegoPhrases(diegoDialogue2);

        if (waypointMover != null)
        {
            waypointMover.StartMoving();
        }
    }

    private void LearnDiegoPhrases(NPCDialogue dialogueData)
    {
        if (dialogueData == null ||
            GameProgressManager.Instance == null)
        {
            return;
        }

        // Add the dialogue's key phrases to persistent progress.
        GameProgressManager.Instance.LearnPhrases(dialogueData);

        // Update the journal UI immediately.
        JournalManager journalManager =
            FindFirstObjectByType<JournalManager>();

        if (journalManager != null)
        {
            journalManager.RefreshJournal();
        }
    }

    public void CloseMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }

        if (npc != null)
        {
            npc.StartDialogueAutomatically();
        }
    }

    public void HandleTableReached()
    {
        // Diego has returned with Justin's soup.
        if (returningWithFood)
        {
            if (cutsceneTriggered)
            {
                return;
            }

            cutsceneTriggered = true;

            if (soupOnTable != null)
            {
                soupOnTable.SetActive(true);
            }

            // Start the cutscene and send Diego away.
            StartCoroutine(FinishRestaurantSequence());

            return;
        }

        // Diego's first arrival at Justin's table.
        if (npc != null)
        {
            npc.StartDialogueAutomatically();
        }
    }

    public void HandleKitchenReached()
    {
        // Prevent this sequence from accidentally running twice.
        if (returningWithFood)
        {
            return;
        }

        returningWithFood = true;

        StartCoroutine(ReturnToTable());
    }

    private IEnumerator ReturnToTable()
    {
        yield return new WaitForSeconds(kitchenWaitTime);

        if (waypointMover != null)
        {
            // Waypoint 0 = Diego's table waypoint.
            waypointMover.MoveToWaypoint(0);
        }
    }

    private IEnumerator FinishRestaurantSequence()
    {
        // Brief pause after placing the soup.
        yield return new WaitForSeconds(departureDelay);

        // Send Diego back toward the kitchen.
        if (waypointMover != null)
        {
            waypointMover.MoveToWaypoint(1);
        }

        // Wait until the cutscene should begin.
        yield return new WaitForSeconds(
            Mathf.Max(0f, soupDisplayTime - departureDelay)
        );

        if (cutsceneDirector != null)
        {
            cutsceneDirector.time = 0;
            cutsceneDirector.Play();
        }
        else
        {
            Debug.LogWarning(
                "DiegoWaiterController: Cutscene Director is not assigned."
            );
        }
    }
}