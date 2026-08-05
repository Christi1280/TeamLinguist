using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange;

    private readonly HashSet<Collider2D> activeColliders = new();

    [Header("Interaction Prompts")]
    [SerializeField] private GameObject chatIcon;
    [SerializeField] private GameObject sitPrompt;

    private void Start()
    {
        HideAllPrompts();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed ||
            interactableInRange == null)
        {
            return;
        }

        /*
         * Do not check CanInteract here.
         * NPC.Interact() must continue receiving E presses
         * while its dialogue is active.
         */
        interactableInRange.Interact();

        if (!interactableInRange.CanInteract())
        {
            HideAllPrompts();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactable =
            collision.GetComponentInParent<IInteractable>();

        if (interactable == null ||
            !interactable.CanInteract())
        {
            return;
        }

        if (interactableInRange == null ||
            interactable == interactableInRange)
        {
            interactableInRange = interactable;
            activeColliders.Add(collision);

            ShowPromptFor(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactable =
            collision.GetComponentInParent<IInteractable>();

        if (interactable == null ||
            interactable != interactableInRange)
        {
            return;
        }

        activeColliders.Remove(collision);

        if (activeColliders.Count == 0)
        {
            interactableInRange = null;
            HideAllPrompts();
        }
    }

    private void ShowPromptFor(IInteractable interactable)
    {
        HideAllPrompts();

        if (interactable is NPC)
        {
            if (chatIcon != null)
            {
                chatIcon.SetActive(true);
            }
        }
        else if (interactable is SeatInteraction)
        {
            if (sitPrompt != null)
            {
                sitPrompt.SetActive(true);
            }
        }
    }

    private void HideAllPrompts()
    {
        if (chatIcon != null)
        {
            chatIcon.SetActive(false);
        }

        if (sitPrompt != null)
        {
            sitPrompt.SetActive(false);
        }
    }
}