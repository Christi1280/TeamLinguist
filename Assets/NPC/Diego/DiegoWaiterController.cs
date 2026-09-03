using UnityEngine;

public class DiegoWaiterController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NPC npc;
    [SerializeField] private GameObject menuPanel;

    [Header("Dialogue")]
    [SerializeField] private NPCDialogue diegoDialogue1;
    [SerializeField] private NPCDialogue diegoDialogue2;

    private bool menuHasBeenShown;

    private void Start()
    {
        if (npc == null)
        {
            npc = GetComponent<NPC>();
        }

        if (npc != null && diegoDialogue1 != null)
        {
            npc.dialogueData = diegoDialogue1;
        }

        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
    }

    public void HandleDialogueEnded()
    {
        if (menuHasBeenShown)
        {
            return;
        }

        menuHasBeenShown = true;

        if (npc != null && diegoDialogue2 != null)
        {
            npc.dialogueData = diegoDialogue2;
        }

        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
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
}