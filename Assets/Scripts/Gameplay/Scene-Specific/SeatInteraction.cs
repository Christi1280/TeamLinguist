using System.ComponentModel;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls when the restaurant seat can be interacted with.
/// After Laura enables the seat, the player can press E to sit
/// and transition to the seated restaurant scene.
/// </summary>

public class SeatInteraction : MonoBehaviour, IInteractable
{
    [Header("Scene")]
    [SerializeField] private string seatedSceneName;

    [Header("References")]
    [SerializeField]
    private LauraHostessController lauraHostessController;

    [SerializeField]
    private Collider2D interactionRange;

    [SerializeField]
    private InteractionDetector interactionDetector;

    private bool interactionEnabled;
    private bool hasSatDown;

    private void Start()
    {
        // Sitting is unavailable until Laura enables it.
        interactionEnabled = false;
        hasSatDown = false;

        if (interactionRange != null)
        {
            interactionRange.enabled = false;
        }
    }

    public bool CanInteract()
    {
        return interactionEnabled &&
               !hasSatDown &&
               !PauseController.IsGamePaused;
    }

    public void Interact()
    {
        if (!CanInteract())
        {
            return;
        }
        // Prevent the seat from being interacted with again.
        hasSatDown = true;
        interactionEnabled = false;

        if (interactionRange != null)
        {
            interactionRange.enabled = false;
        }

        if (lauraHostessController != null)
        {
            lauraHostessController.NotifyPlayerSatDown();
        }

        // Restore the interaction if the next scene was not assigned.
        if (string.IsNullOrWhiteSpace(seatedSceneName))
        {
            Debug.LogError(
                "No seated restaurant scene has been assigned."
            );

            hasSatDown = false;
            EnableSitting();

            return;
        }

        SceneManager.LoadScene(seatedSceneName);
    }

    public void EnableSitting()
    {
        if (hasSatDown)
        {
            return;
        }

        // Must be enabled before ForceInteractable() checks CanInteract().
        interactionEnabled = true;

        if (interactionRange != null)
        {
            interactionRange.enabled = true;
        }

        //  Hand interaction directly to the seat because the player
        //  may already be inside its interaction range.
        if (interactionDetector != null)
        {
            interactionDetector.ForceInteractable(this);

            Debug.Log(
                "SeatInteraction: [E] Sit should now be active."
            );
        }
        else
        {
            Debug.LogError(
                "SeatInteraction: InteractionDetector is not assigned!"
            );
        }
    }

    public void DisableSitting()
    {
        interactionEnabled = false;

        if (interactionRange != null)
        {
            interactionRange.enabled = false;
        }
    }
}