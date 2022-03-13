using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    const string SUPPLY_ITEM_TPYE = "Supply";
    const string USABLE_ITEM_TPYE = "Usable";
    const string REST_ITEM_TPYE = "Rest";

    [SerializeField] private string[] squadUnitNames;
    [SerializeField] private string leaderUnitName;
    private Dictionary<string, UnitSaveData> unitsSaveData;

    [SerializeField] private string[] squadItemNames;
    [SerializeField] private int[] squadItemNumbers;
    private List<Item> squadItemList;
    private Dictionary<Item, int> squadItemDictionary;

    [SerializeField] string[] supplyItemNames = { };
    [SerializeField] int[] supplyItemNumbers = { };
    private Dictionary<Item, int> supplyItemDictionary;

    [SerializeField] string[] usableItemNames = { };
    [SerializeField] int[] usableItemNumbers = { };
    private Dictionary<Item, int> usableItemDictionary;

    [SerializeField] string[] restItemNames = { };
    [SerializeField] int[] restItemNumbers = { };
    private Dictionary<Item, int> restItemDictionary;
    [SerializeField] private int balance;

    public void Init()
    {
        LoadSquadUnitSaveData();
        LoadPlayerItemList();
    }
    private void LoadSquadUnitSaveData()
    {
        unitsSaveData = new Dictionary<string, UnitSaveData>();

        for (int i = 0; i < squadUnitNames.Length; ++i)
        {
            unitsSaveData.Add(squadUnitNames[i], LoadUnitStatus(squadUnitNames[i]));
        }
    }

    private void LoadPlayerItemList()
    {
        squadItemList = new List<Item>();
        squadItemDictionary = new Dictionary<Item, int>();

        for (int i = 0; i < squadItemNames.Length; ++i)
        {
            squadItemList.Add(LoadItemFromJson(squadItemNames[i], SUPPLY_ITEM_TPYE));
            squadItemDictionary.Add(squadItemList[i], squadItemNumbers[i]);
        }

        LoadEachItemType(supplyItemDictionary,supplyItemNames,supplyItemNumbers, SUPPLY_ITEM_TPYE);
        LoadEachItemType(usableItemDictionary, usableItemNames, usableItemNumbers, USABLE_ITEM_TPYE);
        LoadEachItemType(restItemDictionary, restItemNames, restItemNumbers, REST_ITEM_TPYE);

    }

    private void LoadEachItemType(Dictionary<Item, int> itemDictionary, string[] itemNames, int[] itemNumbers, string itemType)
    {
        itemDictionary = new Dictionary<Item, int>();

        for (int i = 0; i < itemNames.Length; ++i)
            itemDictionary.Add(LoadItemFromJson(itemNames[i], itemType), itemNumbers[i]);
    }

    private Item LoadItemFromJson(string itemName, string itemType)
    {
        string itemDataPath = Path.Combine("DataBase", "Item" , itemType);
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

    public void SaveSquadData(List<GameObject> squadList, List<UnitSaveData> unitSaveDataList, Dictionary<Item, int> itemDictionary)
    {
        SaveRemainItem(itemDictionary);
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
        for (int i = 0; i < squadItemList.Count; ++i)
        {
            if (squadItemDictionary[squadItemList[i]] > 0)
                ++itemTypeCnt;
        }

        //size 재조정
        SetNewItemList(itemTypeCnt, squadItemList, squadItemDictionary);
    }

    private void SaveRemainItem(Dictionary<Item, int> remainItemDictionary)
    {
        List<Item> remainItemList = new List<Item>();

        var dictionaryItems = remainItemDictionary.Keys;
        remainItemList.AddRange(dictionaryItems);

        SetNewItemList(remainItemList.Count, remainItemList, remainItemDictionary);
    }

    private void SetNewItemList(int itemTypeCnt, List<Item> _itemList, Dictionary<Item, int> _itemDictionary)
    {
        squadItemNames = new string[itemTypeCnt];
        squadItemNumbers = new int[itemTypeCnt];

        int idx = 0;
        for (int i = 0; i < _itemList.Count; ++i)  //개수가 0이 아닌값만 저장
        {
            if (_itemDictionary[_itemList[idx]] <= 0)
                continue;

            squadItemNames[idx] = _itemList[i].GetIconName();
            squadItemNumbers[idx++] = _itemDictionary[_itemList[i]];
        }
    }

    private void SaveSquadUnitSaveDataList(List<GameObject> squadList, List<UnitSaveData> unitSaveDataList)
    {
        string dataPath = Path.Combine("DataBase", "SaveData", "UnitData");

        for (int i=0; i<squadList.Count ;++i )
        {
            string dataFileName = squadList[i].GetComponent<UnitInterface>().GetUnitData().GetUnitIconName() + "SaveData.json";
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
        string path = Path.Combine(Application.dataPath, saveDataPath, unitName + "SaveData.json");
        string jsonData = File.ReadAllText(path);

        return JsonUtility.FromJson<UnitSaveData>(jsonData);
    }

    public void ApplyItemUse(Item item)
    {
        int remainNum = --squadItemDictionary[item];
    }

    public UnitSaveData GetUnitSaveDataByName(string unitName)
    {
        return unitsSaveData[unitName];
    }
    public string[] GetSquadUnitsName() { return this.squadUnitNames; }

    public Dictionary<Item,int> GetItemDictionary() { return this.squadItemDictionary; }

    public int GetBalance() { return this.balance; }
    public void SetBalance(int _balance) { this.balance = _balance; }
}