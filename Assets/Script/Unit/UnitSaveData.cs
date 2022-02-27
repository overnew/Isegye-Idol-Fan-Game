using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;

[System.Serializable]
public class UnitSaveData
{
    const float EXP_DEFAULT = 100;

    [SerializeField] private float hp;
    [SerializeField] private int level;
    [SerializeField] private float exp;

    public float GetMaxExp() { return level * EXP_DEFAULT; }
    public float GetExp() { return this.exp; }
    public int GetLevel() { return this.level; }

    public float GetHp() { return this.hp; }
}

/*
[SerializeField] private string[] itemNames;
[SerializeField] private int[] itemNumbers;
private Dictionary<Item, int> itemList;

public void Init()
{
    LoadUnitItemList();
}

private void LoadUnitItemList()
{
    itemList = new Dictionary<Item, int>();

    for (int i=0; i< itemNames.Length ; ++i)
    {
        itemList.Add(LoadItemFromJson(itemNames[i]), itemNumbers[i]);
    }
}

private Item LoadItemFromJson(string itemName)
{
    string itemDataPath = Path.Combine("DataBase", "Item");
    string path = Path.Combine(Application.dataPath, itemDataPath, itemName + ".json");
    string jsonData = File.ReadAllText(path);
    return JsonUtility.FromJson<Item>(jsonData);
}
public Dictionary<Item, int> GetItemList() { return this.itemList; }*/