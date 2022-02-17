using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataManager
{
    private SquadData squadData;
    private ItemSaveData itemSaveData;
    
    public SaveDataManager()   //객체 생성시 세이브 파일 로드
    {
        LoadSquadData();
        LoadItemSaveData();
    }
    private void LoadSquadData()
    {
        string saveDataPath = Path.Combine("DataBase", "SaveData");
        string path = Path.Combine(Application.dataPath, saveDataPath, "squadData" + ".json");
        string jsonData = File.ReadAllText(path);

        squadData = JsonUtility.FromJson<SquadData>(jsonData);
        squadData.Init();
    }

    private void LoadItemSaveData()
    {
        string saveDataPath = Path.Combine("DataBase", "SaveData");
        string path = Path.Combine(Application.dataPath, saveDataPath, "itemData" + ".json");
        string jsonData = File.ReadAllText(path);

        itemSaveData = JsonUtility.FromJson<ItemSaveData>(jsonData);
        itemSaveData.Init();
    }

    public SquadData GetSquadData() { return this.squadData; }
    public ItemSaveData GetItemSaveData() { return itemSaveData; }
}
