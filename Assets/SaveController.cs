using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveController : MonoBehaviour
{
    private const string MainHubSceneName = "MainHub";
    private const string RestaurantSceneName = "Restaurant";

    private string saveLocation;

    private InventoryController inventoryController;
    private HotBarController hotbarController;
    private Chest[] chests;

    private void Start()
    {
        InitializeComponents();
        LoadGame();
    }

    private void InitializeComponents()
    {
        saveLocation = Path.Combine(
            Application.persistentDataPath,
            "saveData.json"
        );

        inventoryController =
            FindFirstObjectByType<InventoryController>();

        hotbarController =
            FindFirstObjectByType<HotBarController>();

        chests = FindObjectsByType<Chest>(
            FindObjectsSortMode.None
        );

        if (inventoryController == null)
        {
            Debug.LogWarning(
                "SaveController: InventoryController was not found."
            );
        }

        if (hotbarController == null)
        {
            Debug.LogWarning(
                "SaveController: HotBarController was not found. " +
                "Hotbar data will not be saved or loaded."
            );
        }
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

            mapBoundary = GetCurrentMapBoundaryName(),

            inventorySaveData =
                inventoryController != null
                    ? inventoryController.GetInventoryItems()
                    : new List<InventorySaveData>(),

            hotbarSaveData =
                hotbarController != null
                    ? hotbarController.GetHotbarItems()
                    : new List<InventorySaveData>(),

            chestSaveData = GetChestState()
        };

        string json = JsonUtility.ToJson(saveData, true);

        File.WriteAllText(saveLocation, json);

        Debug.Log(
            $"Game saved in scene '{currentSceneName}' " +
            $"at position {player.transform.position}."
        );
    }

    public void LoadGame()
    {
        if (!File.Exists(saveLocation))
        {
            SetEmptyInventoryData();

            Debug.Log(
                "No save file was found. " +
                "The player will use the position set in the scene."
            );

            return;
        }

        string json = File.ReadAllText(saveLocation);

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

        LoadInventory(saveData);
        LoadHotbar(saveData);
        LoadChestStates(saveData.chestSaveData);
    }

    private void LoadPlayerPosition(SaveData saveData)
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

        return confiner.BoundingShape2D.gameObject.name;
    }

    private void LoadMapBoundary(SaveData saveData)
    {
        if (string.IsNullOrEmpty(saveData.mapBoundary))
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
            GameObject.Find(saveData.mapBoundary);

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
            boundaryObject.GetComponent<PolygonCollider2D>();

        if (boundaryCollider == null)
        {
            Debug.LogWarning(
                $"SaveController: '{saveData.mapBoundary}' does not " +
                "have a PolygonCollider2D."
            );

            return;
        }

        confiner.BoundingShape2D = boundaryCollider;
        confiner.InvalidateBoundingShapeCache();
    }

    private void LoadInventory(SaveData saveData)
    {
        if (inventoryController == null)
        {
            return;
        }

        inventoryController.SetInventoryItems(
            saveData.inventorySaveData
            ?? new List<InventorySaveData>()
        );
    }

    private void LoadHotbar(SaveData saveData)
    {
        if (hotbarController == null)
        {
            return;
        }

        hotbarController.SetHotbarItems(
            saveData.hotbarSaveData
            ?? new List<InventorySaveData>()
        );
    }

    private void SetEmptyInventoryData()
    {
        if (inventoryController != null)
        {
            inventoryController.SetInventoryItems(
                new List<InventorySaveData>()
            );
        }

        if (hotbarController != null)
        {
            hotbarController.SetHotbarItems(
                new List<InventorySaveData>()
            );
        }
    }

    private List<ChestSaveData> GetChestState()
    {
        List<ChestSaveData> chestStates =
            new List<ChestSaveData>();

        if (chests == null)
        {
            return chestStates;
        }

        foreach (Chest chest in chests)
        {
            if (chest == null)
            {
                continue;
            }

            ChestSaveData chestSaveData =
                new ChestSaveData
                {
                    chestID = chest.ChestID,
                    IsOpened = chest.IsOpened
                };

            chestStates.Add(chestSaveData);
        }

        return chestStates;
    }

    private void LoadChestStates(
        List<ChestSaveData> chestStates
    )
    {
        if (chests == null || chestStates == null)
        {
            return;
        }

        foreach (Chest chest in chests)
        {
            if (chest == null)
            {
                continue;
            }

            ChestSaveData chestSaveData =
                chestStates.FirstOrDefault(
                    savedChest =>
                        savedChest.chestID == chest.ChestID
                );

            if (chestSaveData != null)
            {
                chest.SetOpened(chestSaveData.IsOpened);
            }
        }
    }

    [ContextMenu("Delete Save File")]
    public void DeleteSaveFile()
    {
        if (!string.IsNullOrEmpty(saveLocation) &&
            File.Exists(saveLocation))
        {
            File.Delete(saveLocation);

            Debug.Log("Save file deleted.");
        }
        else
        {
            Debug.Log("No save file was found to delete.");
        }
    }
}