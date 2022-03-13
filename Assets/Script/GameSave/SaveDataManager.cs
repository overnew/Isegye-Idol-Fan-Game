using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataManager
{
    private PlayerData playerData;
    private int balance;
    private ItemSaveData itemSaveData;
    
    public SaveDataManager()   //객체 생성시 세이브 파일 로드
    {
        LoadPlayerData();
        LoadItemSaveData();

        this.balance = playerData.GetBalance();
    }
    private void LoadPlayerData()
    {
        string saveDataPath = Path.Combine("DataBase", "SaveData");
        string path = Path.Combine(Application.dataPath, saveDataPath, "PlayerData" + ".json");
        string jsonData = File.ReadAllText(path);

        playerData = JsonUtility.FromJson<PlayerData>(jsonData);
        playerData.Init();
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
        playerData.SetBalance(this.balance);
    }

    public PlayerData GetPlayerData() { return this.playerData; }
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
