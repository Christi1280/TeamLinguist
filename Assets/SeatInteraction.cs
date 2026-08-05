using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SeatInteraction : MonoBehaviour, IInteractable
{
    [Header("Scene")]
    [SerializeField] private string seatedSceneName;

    [Header("References")]
    [SerializeField]
    private LauraHostessController lauraHostessController;

    [SerializeField]
    private Collider2D interactionRange;

    private bool interactionEnabled;
    private bool hasSatDown;

    private void Start()
    {
        // Sitting should initially be unavailable.
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

        StartCoroutine(EnableSittingAfterPhysicsUpdate());
    }

    private IEnumerator EnableSittingAfterPhysicsUpdate()
    {
        interactionEnabled = false;

        if (interactionRange != null)
        {
            interactionRange.enabled = false;
        }

        // Give Unity time to remove Laura from InteractionDetector.
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        interactionEnabled = true;

        if (interactionRange != null)
        {
            interactionRange.enabled = true;
        }

        Debug.Log("The player can now press E to sit down.");
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