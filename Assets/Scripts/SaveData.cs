using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    // Scene the player was in when the game was saved
    public string sceneName;

    // Player data
    public Vector3 playerPosition;

    // Name of the active Cinemachine map boundary
    public string mapBoundary;

    // Inventory
    public List<InventorySaveData> inventorySaveData;

    // Hotbar
    public List<InventorySaveData> hotbarSaveData;

    // Chest states
    public List<ChestSaveData> chestSaveData;
}

[System.Serializable]
public class ChestSaveData
{
    public string chestID;
    public bool IsOpened;
}