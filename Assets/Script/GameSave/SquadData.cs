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
    private List<Item> itemList;
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
        itemList = new List<Item>();
        itemDictionary = new Dictionary<Item, int>();

        for (int i = 0; i < itemNames.Length; ++i)
        {
            itemList.Add(LoadItemFromJson(itemNames[i]));
            itemDictionary.Add(itemList[i], itemNumbers[i]);
        }
    }

    private Item LoadItemFromJson(string itemName)
    {
        string itemDataPath = Path.Combine("DataBase", "Item");
        string path = Path.Combine(Application.dataPath, itemDataPath, itemName + ".json");
        string jsonData = File.ReadAllText(path);
        return JsonUtility.FromJson<Item>(jsonData);
    }

    public void SaveSquadData(List<GameObject> squadList, List<UnitSaveData> unitSaveDataList)
    {
        SaveRemainSquadData(squadList);
        SaveRemainItem();

        SaveSquadUnitSaveDataList(squadList, unitSaveDataList);

        SaveSquadDataToJson();
    }

    private void SaveRemainSquadData(List<GameObject> squadList)
    {
        squadUnitNames = new string[squadList.Count];
        for (int idx = 0; idx < squadList.Count; ++idx)
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

    private void SaveRemainItem()
    {
        int itemTypeCnt = 0;
        for (int i = 0; i < itemList.Count; ++i)
        {
            if (itemDictionary[itemList[i]] > 0)
                ++itemTypeCnt;
        }

        //size 재조정
        SetNewItemList(itemTypeCnt, itemList, itemDictionary);
    }

    public void SaveRemainItem(Dictionary<Item, int> remainItemDictionary)
    {
        List<Item> remainItemList = new List<Item>();

        var dictionaryItems = remainItemDictionary.Keys;
        remainItemList.AddRange(dictionaryItems);

        SetNewItemList(remainItemList.Count, remainItemList, remainItemDictionary);

        SaveSquadDataToJson();
    }

    private void SetNewItemList(int itemTypeCnt, List<Item> _itemList, Dictionary<Item, int> _itemDictionary)
    {
        itemNames = new string[itemTypeCnt];
        itemNumbers = new int[itemTypeCnt];

        int idx = 0;
        for (int i = 0; i < _itemList.Count; ++i)  //개수가 0이 아닌값만 저장
        {
            if (_itemDictionary[_itemList[idx]] <= 0)
                continue;

            itemNames[idx] = _itemList[i].GetIconName();
            itemNumbers[idx++] = _itemDictionary[_itemList[i]];
        }
    }

    private void SaveSquadUnitSaveDataList(List<GameObject> squadList, List<UnitSaveData> unitSaveDataList)
    {
        string dataPath = Path.Combine("DataBase", "SaveData", "UnitData");

        for (int i=0; i<squadList.Count ;++i )
        {
            string dataFileName = squadList[i].GetComponent<UnitInterface>().GetUnitData().GetUnitIconName() + "Data.json";
            string jsonData = JsonUtility.ToJson(unitSaveDataList[i], true);
            string path = Path.Combine(Application.dataPath, dataPath, dataFileName);
            File.WriteAllText(path, jsonData);
        }
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