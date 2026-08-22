using System.IO;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveController : MonoBehaviour
{
    private string saveLocation;

    private void Start()
    {
        InitializeSaveLocation();
        LoadGame();
    }

    private void InitializeSaveLocation()
    {
        saveLocation = Path.Combine(
            Application.persistentDataPath,
            "saveData.json"
        );
    }

    public void SaveGame()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError(
                "SaveController: No GameObject with the Player tag was found."
            );

            return;
        }

        string currentSceneName =
            SceneManager.GetActiveScene().name;

        SaveData saveData = new SaveData
        {
            sceneName = currentSceneName,

            playerPosition = player.transform.position,

            mapBoundary = GetCurrentMapBoundaryName()
        };

        string json =
            JsonUtility.ToJson(saveData, true);

        File.WriteAllText(
            saveLocation,
            json
        );

        Debug.Log(
            $"Game saved in scene '{currentSceneName}' " +
            $"at position {player.transform.position}."
        );
    }

    public void LoadGame()
    {
        if (!File.Exists(saveLocation))
        {
            Debug.Log(
                "No save file was found. " +
                "The player will use the position set in the scene."
            );

            return;
        }

        string json =
            File.ReadAllText(saveLocation);

        SaveData saveData =
            JsonUtility.FromJson<SaveData>(json);

        if (saveData == null)
        {
            Debug.LogError(
                "SaveController: The save file could not be read."
            );

            return;
        }

        string currentSceneName =
            SceneManager.GetActiveScene().name;

        bool saveBelongsToCurrentScene =
            saveData.sceneName == currentSceneName;

        if (saveBelongsToCurrentScene)
        {
            LoadPlayerPosition(saveData);
            LoadMapBoundary(saveData);
        }
        else
        {
            Debug.Log(
                $"The saved player position belongs to " +
                $"'{saveData.sceneName}', but the current scene is " +
                $"'{currentSceneName}'. The player will use the position " +
                $"set in the {currentSceneName} scene."
            );
        }
    }

    private void LoadPlayerPosition(
        SaveData saveData
    )
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning(
                "SaveController: Player was not found while loading."
            );

            return;
        }

        player.transform.position =
            saveData.playerPosition;

        Debug.Log(
            $"Loaded player position: {saveData.playerPosition}"
        );
    }

    private string GetCurrentMapBoundaryName()
    {
        CinemachineConfiner2D confiner =
            FindFirstObjectByType<CinemachineConfiner2D>();

        if (confiner == null)
        {
            Debug.LogWarning(
                "SaveController: CinemachineConfiner2D was not found."
            );

            return string.Empty;
        }

        if (confiner.BoundingShape2D == null)
        {
            Debug.LogWarning(
                "SaveController: The Cinemachine Confiner has no " +
                "Bounding Shape 2D assigned."
            );

            return string.Empty;
        }

        return confiner
            .BoundingShape2D
            .gameObject
            .name;
    }

    private void LoadMapBoundary(
        SaveData saveData
    )
    {
        if (string.IsNullOrEmpty(
                saveData.mapBoundary))
        {
            return;
        }

        CinemachineConfiner2D confiner =
            FindFirstObjectByType<CinemachineConfiner2D>();

        if (confiner == null)
        {
            Debug.LogWarning(
                "SaveController: CinemachineConfiner2D was not found."
            );

            return;
        }

        GameObject boundaryObject =
            GameObject.Find(
                saveData.mapBoundary
            );

        if (boundaryObject == null)
        {
            Debug.LogWarning(
                $"SaveController: Map boundary " +
                $"'{saveData.mapBoundary}' was not found in " +
                $"'{SceneManager.GetActiveScene().name}'."
            );

            return;
        }

        PolygonCollider2D boundaryCollider =
            boundaryObject
                .GetComponent<PolygonCollider2D>();

        if (boundaryCollider == null)
        {
            Debug.LogWarning(
                $"SaveController: '{saveData.mapBoundary}' does not " +
                "have a PolygonCollider2D."
            );

            return;
        }

        confiner.BoundingShape2D =
            boundaryCollider;

        confiner.InvalidateBoundingShapeCache();
    }

    [ContextMenu("Delete Save File")]
    public void DeleteSaveFile()
    {
        if (!string.IsNullOrEmpty(saveLocation) &&
            File.Exists(saveLocation))
        {
            File.Delete(saveLocation);

            Debug.Log(
                "Save file deleted."
            );
        }
        else
        {
            Debug.Log(
                "No save file was found to delete."
            );
        }
    }
}