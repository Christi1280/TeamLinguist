using UnityEngine;
using UnityEngine.SceneManagement;

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

        /*
         * Enable sitting FIRST.
         *
         * ForceInteractable() checks CanInteract(), so this
         * must be true before we hand interaction to the seat.
         */
        interactionEnabled = true;

        if (interactionRange != null)
        {
            interactionRange.enabled = true;
        }

        /*
         * Do not wait for OnTriggerEnter2D.
         *
         * The player may already be standing inside the
         * chair's interaction area.
         */
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