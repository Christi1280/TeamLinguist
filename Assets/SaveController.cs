using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Cinemachine;
using UnityEditor.Overlays;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    private string saveLocation;
    private InventoryController inventoryController;
    private HotBarController hotbarController;
    private Chest[] chests;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeComponents();
        LoadGame();
    }

    private void InitializeComponents()
    {
        //Define save location
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
        inventoryController = FindFirstObjectByType<InventoryController>();
        hotbarController = FindFirstObjectByType<HotBarController>();
        chests = FindObjectsByType<Chest>(FindObjectsSortMode.None);

        if (hotbarController == null)
        {
            Debug.LogWarning("SaveController: HotBarController not found in scene (may be disabled). Hotbar data will not be saved/loaded.");
        }
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
            mapBoundary = FindFirstObjectByType<CinemachineConfiner2D>().BoundingShape2D.gameObject.name,
            inventorySaveData = inventoryController.GetInventoryItems(),
            hotbarSaveData = hotbarController != null ? hotbarController.GetHotbarItems() : new List<InventorySaveData>(),
            chestSaveData = GetChestState()
        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
    }

    private List<ChestSaveData> GetChestState()
    {
        List<ChestSaveData> chestStates = new List<ChestSaveData>();
        foreach (Chest chest in chests)
        {
            ChestSaveData chestSaveData = new ChestSaveData
            {
                chestID = chest.ChestID,
                IsOpened = chest.IsOpened
            };
            chestStates.Add(chestSaveData);
        }
        return chestStates;
    }

    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playerPosition;
            FindFirstObjectByType<CinemachineConfiner2D>().BoundingShape2D = GameObject.Find(saveData.mapBoundary).GetComponent<PolygonCollider2D>();
            inventoryController.SetInventoryItems(saveData.inventorySaveData);

            if (hotbarController != null)
            {
                hotbarController.SetHotbarItems(saveData.hotbarSaveData);
            }

            LoadChestStates(saveData.chestSaveData);
        }
        else
        {
            SaveGame();
            inventoryController.SetInventoryItems(new List<InventorySaveData>());

            if (hotbarController != null)
            {
                hotbarController.SetHotbarItems(new List<InventorySaveData>());
            }
        }
    }

    private void LoadChestStates(List<ChestSaveData> chestStates)
    {
        foreach (Chest chest in chests)
        {
            ChestSaveData chestSaveData = chestStates.FirstOrDefault(c => c.chestID == chest.ChestID);
            if (chestSaveData != null)
            {
                chest.SetOpened(chestSaveData.IsOpened);
            }
        }
    }
}