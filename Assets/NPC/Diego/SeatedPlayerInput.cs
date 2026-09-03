using UnityEngine;
using UnityEngine.InputSystem;

public class SeatedPlayerInput : MonoBehaviour
{
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        DialogueController dialogueController =
            DialogueController.Instance;

        if (dialogueController == null)
        {
            return;
        }

        NPC currentNPC =
            dialogueController.GetCurrentNPC();

        if (currentNPC != null)
        {
            currentNPC.Interact();
        }
    }
}