using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataManager
{
    const int START_BALANCE = 10000;
    private SquadData squadData;
    private int balance;
    private ItemSaveData itemSaveData;
    
    public SaveDataManager()   //��ü ������ ���̺� ���� �ε�
    {
        balance = START_BALANCE;
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

    internal int GetBalance() { return this.balance; }
}
