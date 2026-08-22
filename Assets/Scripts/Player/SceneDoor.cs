using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDoor : MonoBehaviour
{
    [Header("Destination")]
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string destinationSpawnPoint;

    [Header("Optional")]
    [SerializeField] private bool requireInteraction;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    [Header("Progress Requirement")]
    [SerializeField] private int minimumModuleProgress = 0;

    private bool playerInsideTrigger;
    private bool isLoading;

    private void Update()
    {
        if (!requireInteraction ||
            !playerInsideTrigger ||
            isLoading)
        {
            return;
        }

        if (Input.GetKeyDown(interactionKey))
        {
            EnterDoor();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInsideTrigger = true;

        if (!requireInteraction)
        {
            EnterDoor();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInsideTrigger = false;
    }

    private void EnterDoor()
    {
        if (isLoading)
        {
            return;
        }

        // Check whether the player has reached the
        // required point in the module.
        if (GameProgressManager.Instance == null)
        {
            Debug.LogWarning(
                "SceneDoor: GameProgressManager was not found."
            );

            return;
        }

        if (GameProgressManager.Instance.ModuleProgress <
            minimumModuleProgress)
        {
            Debug.Log(
                $"Door locked. Requires " +
                $"{minimumModuleProgress}% module progress."
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(sceneToLoad))
        {
            Debug.LogError(
                $"SceneDoor on '{gameObject.name}' " +
                $"has no scene assigned."
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(destinationSpawnPoint))
        {
            Debug.LogError(
                $"SceneDoor on '{gameObject.name}' " +
                $"has no destination spawn point assigned."
            );

            return;
        }

        isLoading = true;

        PlayerPrefs.SetString(
            "DestinationSpawnPoint",
            destinationSpawnPoint
        );

        SceneManager.LoadScene(sceneToLoad);
    }
}