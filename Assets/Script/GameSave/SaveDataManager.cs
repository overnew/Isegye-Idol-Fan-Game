using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataManager
{
    private SquadData squadData;
    private int balance;
    private ItemSaveData itemSaveData;
    
    public SaveDataManager()   //객체 생성시 세이브 파일 로드
    {
        LoadSquadData();
        LoadItemSaveData();

        this.balance = squadData.GetBalance();
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
        string path = Path.Combine(Application.dataPath, saveDataPath, "AllItemData" + ".json");
        string jsonData = File.ReadAllText(path);

        itemSaveData = JsonUtility.FromJson<ItemSaveData>(jsonData);
        itemSaveData.Init();
    }

    public void SaveCurrentData()
    {
        squadData.SetBalance(this.balance);
    }

    public SquadData GetSquadData() { return this.squadData; }
    public ItemSaveData GetItemSaveData() { return itemSaveData; }

    internal int GetBalance() { return this.balance; }
    internal void AddBalance(int change) 
    { 
        this.balance += change; 
        if(this.balance < 0)
        {
            balance = 0;
            throw new System.Exception("잔액이 음수입니다.");
        } 
    }
}
