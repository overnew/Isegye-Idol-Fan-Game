using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;

[System.Serializable]
public class ItemSaveData
{
    [SerializeField] string[] supplyItemNames = { "" };
    [SerializeField] string[] usableItemNames = { "" };
    [SerializeField] string[] restItemNames = { "" };
    private Dictionary<string, Item> allItemDictionary;
    private Dictionary<string, Item> supplyItemDictionary = new Dictionary<string, Item>();
    private Dictionary<string, Item> usableItemDictionary = new Dictionary<string, Item>();
    private Dictionary<string, Item> restItemDictionary = new Dictionary<string, Item>();

    internal void Init()
    {
        LoadAllItem();
    }

    private void LoadAllItem()
    {
        allItemDictionary = new Dictionary<string, Item>();

        for (int i=0; i<supplyItemNames.Length ;++i )
        {
            allItemDictionary.Add(supplyItemNames[i], LoadItemFromJson(supplyItemNames[i], "Supply"));
            supplyItemDictionary.Add(supplyItemNames[i], allItemDictionary[supplyItemNames[i]]);
        }

        for (int i = 0; i < usableItemNames.Length; ++i)
        {
            allItemDictionary.Add(usableItemNames[i], LoadItemFromJson(usableItemNames[i], "Usable"));
            usableItemDictionary.Add(usableItemNames[i], allItemDictionary[usableItemNames[i]]);
        }

        for (int i = 0; i < restItemNames.Length; ++i)
        {
            allItemDictionary.Add(restItemNames[i], LoadItemFromJson(restItemNames[i], "Rest"));
            restItemDictionary.Add(restItemNames[i], allItemDictionary[restItemNames[i]]);
        }

    }
    private Item LoadItemFromJson(string itemName ,string itemType)
    {
        string itemDataPath = Path.Combine("DataBase", "Item", itemType);
        string path = Path.Combine(Application.dataPath, itemDataPath, itemName + ".json");
        string jsonData = File.ReadAllText(path);
        return JsonUtility.FromJson<Item>(jsonData);
    }

    internal List<string> GetAllItemName()
    {
        List<string> temp = new List<string>();
        temp.AddRange(supplyItemNames);
        return temp;
    }

    internal Dictionary<string, Item> GetAllItemDictionary() { return this.allItemDictionary; }

    internal Dictionary<string, Item> GetSupplyItemDictionary() { return this.supplyItemDictionary; }
    internal Dictionary<string, Item> GetUsableItemDictionary() { return this.usableItemDictionary; }
    internal Dictionary<string, Item> GetRestItemDictionary() { return this.restItemDictionary; }

}

public enum ItemTypeToIndex
{
    supplyItem =0,
    usableItem = 1,
    restItem = 2
}