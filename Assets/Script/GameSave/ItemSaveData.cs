using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;

[System.Serializable]
public class ItemSaveData
{
    [SerializeField] string[] names;
    private Dictionary<string, Item> allItemDictionary;

    internal void Init()
    {
        LoadAllItem();
    }

    private void LoadAllItem()
    {
        allItemDictionary = new Dictionary<string, Item>();

        for (int i=0; i<names.Length ;++i ) 
        {
            allItemDictionary.Add(names[i], LoadItemFromJson(names[i]));
        }

    }
    private Item LoadItemFromJson(string itemName)
    {
        string itemDataPath = Path.Combine("DataBase", "Item");
        string path = Path.Combine(Application.dataPath, itemDataPath, itemName + ".json");
        string jsonData = File.ReadAllText(path);
        return JsonUtility.FromJson<Item>(jsonData);
    }

    internal Dictionary<string, Item> GetAllItemDictionary() { return this.allItemDictionary; }
    internal List<string> GetAllItemName() 
    {
        List<string> temp = new List<string>();
        temp.AddRange(names);
        return temp; 
    }
}