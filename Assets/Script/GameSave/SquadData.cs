using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SquadData
{
    [SerializeField] private string[] squadUnitNames;
    [SerializeField] private string leaderUnitName;
    private Dictionary<string, UnitSaveData> unitsSaveData;

    [SerializeField] private string[] itemNames;
    [SerializeField] private int[] itemNumbers;
    private Dictionary<Item, int> itemDictionary;

    [SerializeField] private int balance;



    public void Init()
    {
        LoadSquadUnitSaveData();
        LoadItemList();
    }
    private void LoadSquadUnitSaveData()
    {
        unitsSaveData = new Dictionary<string, UnitSaveData>();

        for (int i = 0; i < squadUnitNames.Length; ++i)
        {
            unitsSaveData.Add(squadUnitNames[i], LoadUnitStatus(squadUnitNames[i]));
        }
    }
    private void LoadItemList()
    {
        itemDictionary = new Dictionary<Item, int>();

        for (int i = 0; i < itemNames.Length; ++i)
        {
            itemDictionary.Add(LoadItemFromJson(itemNames[i]), itemNumbers[i]);
        }
    }

    private Item LoadItemFromJson(string itemName)
    {
        string itemDataPath = Path.Combine("DataBase", "Item");
        string path = Path.Combine(Application.dataPath, itemDataPath, itemName + ".json");
        string jsonData = File.ReadAllText(path);
        return JsonUtility.FromJson<Item>(jsonData);
    }

    public void SaveRemainSquadData(List<GameObject> squadList)
    {
        squadUnitNames = new string[squadList.Count];
        for(int idx =0; idx< squadList.Count ; ++idx)
        {
            this.squadUnitNames[idx] = squadList[idx].GetComponent<UnitInterface>().GetUnitData().GetUnitIconName();
        }

        if (!CheckLeaderUnitIsRemain())
            leaderUnitName = squadUnitNames[0];

        SaveSquadDataToJson();
    }

    private bool CheckLeaderUnitIsRemain()
    {
        for (int idx = 0; idx < squadUnitNames.Length; ++idx)
            if (squadUnitNames[idx].Equals(leaderUnitName))
                return true;

        return false;
    }

    private void SaveSquadDataToJson()
    {
        string dataPath = Path.Combine("DataBase" ,"SaveData");
        string dataFileName = "squadData.json";

        string jsonData = JsonUtility.ToJson(this, true);
        string path = Path.Combine(Application.dataPath, dataPath, dataFileName);
        File.WriteAllText(path, jsonData);
    }

    public List<GameObject> GetSquadUnitPrefabs()
    {
        List<GameObject> squadList = new List<GameObject>();

        for (int i=0; i<squadUnitNames.Length ; ++i)
        {
            squadList.Add(GetPrefabByName(squadUnitNames[i]));
        }
        return squadList;
    }

    internal GameObject GetLeaderUnitPrefab()
    {
        return GetPrefabByName(leaderUnitName);
    }

    private GameObject GetPrefabByName(string prefabName)
    {
        string prefabPath = Path.Combine("Prefab", "PlayerUnit", prefabName);
        return Resources.Load<GameObject>(prefabPath);
    }

    private UnitSaveData LoadUnitStatus(string unitName)
    {
        string saveDataPath = Path.Combine("DataBase", "SaveData", "UnitData");
        string path = Path.Combine(Application.dataPath, saveDataPath, unitName + "Data.json");
        string jsonData = File.ReadAllText(path);

        return JsonUtility.FromJson<UnitSaveData>(jsonData);
    }

    public void ApplyItemUse(Item item)
    {
        int remainNum = --itemDictionary[item];
    }

    public UnitSaveData GetUnitSaveDataByName(string unitName)
    {
        return unitsSaveData[unitName];
    }
    public string[] GetSquadUnitsName() { return this.squadUnitNames; }

    public Dictionary<Item,int> GetItemDictionary() { return this.itemDictionary; }

    public int GetBalance() { return this.balance; }
    public void SetBalance(int _balance) { this.balance = _balance; }
}