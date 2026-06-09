using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData 
{
    public Vector3 playerPosition;
    public string mapBoundary; //name for boundary of map
    public List<InventorySaveData> inventorySaveData;
}
