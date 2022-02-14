using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SquadData
{
    [SerializeField] private string[] squadUnitNames;
    private Dictionary<string, UnitSaveData> unitsSaveData;

    [SerializeField] private string[] itemNames;
    [SerializeField] private int[] itemNumbers;
    private Dictionary<Item, int> itemDictionary;

    public void Init()
    {
        LoadSquadUnitSaveData();
        LoadItemList();
    }
    public void LoadSquadUnitSaveData()
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

    public List<GameObject> GetSquadUnitPrefabs()
    {
        List<GameObject> squadList = new List<GameObject>();

        for (int i=0; i<squadUnitNames.Length ; ++i)
        {
            squadList.Add(GetPrefabByName(squadUnitNames[i]));
        }
        return squadList;
    }

    private GameObject GetPrefabByName(string prefabName)
    {
        string prefabPath = Path.Combine("Prefab", prefabName);
        return Resources.Load<GameObject>(prefabPath);
    }
    

    private UnitSaveData LoadUnitStatus(string unitName)
    {
        string saveDataPath = Path.Combine("DataBase", "SaveData", "UnitData");
        string path = Path.Combine(Application.dataPath, saveDataPath, unitName + "Data.json");
        string jsonData = File.ReadAllText(path);

        return JsonUtility.FromJson<UnitSaveData>(jsonData);
    }

    public UnitSaveData GetUnitSaveDataByName(string unitName)
    {
        return unitsSaveData[unitName];
    }
    public string[] GetSquadUnitsName() { return this.squadUnitNames; }

    public Dictionary<Item,int> GetItemDictionary() { return this.itemDictionary; }
}