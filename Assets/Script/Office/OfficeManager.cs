using System.Collections;
using UnityEngine;

public class OfficeManager : MonoBehaviour
{
    private SaveDataManager saveDataManager;
    private SquadData squadData;
    private ItemSaveData itemSaveData;

    private void Awake()
    {
        saveDataManager = new SaveDataManager();
        squadData = saveDataManager.GetSquadData();
        itemSaveData = saveDataManager.GetItemSaveData();
    }

    internal SaveDataManager GetSaveDataManager() { return this.saveDataManager; }
    
}