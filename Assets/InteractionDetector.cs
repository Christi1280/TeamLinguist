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
         * Save the interactable that received this E press.
         *
         * During Interact(), Laura may finish her dialogue and
         * SeatInteraction may replace her as the current
         * interactable.
         */
        IInteractable interactedWith = interactableInRange;

        interactedWith.Interact();

        /*
         * IMPORTANT:
         *
         * Only hide the prompt if the object that received the
         * interaction is STILL the current interactable.
         *
         * If Laura's final dialogue changed the current
         * interactable to the seat, we must NOT hide the
         * newly-created [E] Sit prompt.
         */
        if (interactableInRange == interactedWith &&
            !interactedWith.CanInteract())
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

        /*
         * If this collider belongs to an OLD interactable,
         * ignore it.
         *
         * This is important when Laura's collider gets disabled
         * immediately after the seat becomes active.
         */
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

    public void ForceInteractable(IInteractable interactable)
    {
        if (interactable == null)
        {
            Debug.LogError(
                "InteractionDetector: Cannot force a null interactable."
            );

            return;
        }

        if (!interactable.CanInteract())
        {
            Debug.LogError(
                "InteractionDetector: Forced interactable cannot interact."
            );

            return;
        }

        /*
         * Completely forget the previous interaction.
         *
         * We intentionally do NOT add the seat collider to
         * activeColliders here.
         *
         * This is a direct interaction handoff.
         */
        activeColliders.Clear();

        interactableInRange = interactable;

        ShowPromptFor(interactable);

        Debug.Log(
            $"Interaction switched directly to {interactable}."
        );
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