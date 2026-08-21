using System.Collections;
using UnityEngine;

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
        if (waypointMover != null)
        {
            waypointMover.StartMoving();
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
        // Diego has returned from the kitchen.
        if (returningWithFood)
        {
            if (soupOnTable != null)
            {
                soupOnTable.SetActive(true);
            }

            StartCoroutine(FinishRestaurantSequence());

            return;
        }

        // Diego is arriving at the table for the first time.
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
        // Give the player time to see the soup.
        yield return new WaitForSeconds(soupDisplayTime);

        if (screenFader != null)
        {
            screenFader.FadeToBlack();
        }
    }
}