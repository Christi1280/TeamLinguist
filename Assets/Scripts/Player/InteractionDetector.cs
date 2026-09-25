using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// InteractionDetector detects nearby interactable objects, shows the appropriate interaction prompt,
/// and calls their Interact() method when the player presses E. It also supports directly handing
/// interaction from one object to another, such as from Laura to the restaurant seat.
/// </summary>

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange;

    // Tracks colliders belonging to the current interactable.
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

        // Save what received the interaction because Interact()
        // may immediately switch to a different interactable.
        IInteractable interactedWith = interactableInRange;

        interactedWith.Interact();


        // Don't hide a prompt belonging to a newly activated interactable.
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

        /// Ignore colliders belonging to an interactable that is no longer active.
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

        // Used when interaction needs to pass directly from one object
        // to another without waiting for trigger detection.
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