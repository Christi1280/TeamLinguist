
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

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

        GameProgressManager.Instance.LearnPhrases(dialogueData);

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

            StartCoroutine(FinishRestaurantSequence());
            return;
        }

        if (npc != null)
        {
            npc.StartDialogueAutomatically();
        }
    }

    public void HandleKitchenReached()
    {
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
            waypointMover.MoveToWaypoint(0);
        }
    }

    private IEnumerator FinishRestaurantSequence()
    {
        yield return new WaitForSeconds(departureDelay);

        if (waypointMover != null)
        {
            waypointMover.MoveToWaypoint(1);
        }

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

    public void OpenCipherPuzzle()
    {
        SceneManager.LoadScene("Puzzle");
    }
}
