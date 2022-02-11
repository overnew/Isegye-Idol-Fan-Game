using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;

[System.Serializable]
public class ContinueUnitStatus
{
    private float hp;
    private string[] itemNames;
    private int[] itemNumbers;

    public List<KeyValuePair<Item, int>> LoadItemData()
    {
        List<KeyValuePair<Item, int>> itemAndNumbers = new List<KeyValuePair<Item, int>>();

        for (int i=0; i< itemNames.Length ; ++i)
        {
            itemAndNumbers.Add(new KeyValuePair<Item, int>(LoadItemFromJson(itemNames[i]), itemNumbers[i]));
        }

        return itemAndNumbers;
    }

    private Item LoadItemFromJson(string itemName)
    {
        string itemDataPath = Path.Combine("DataBase", "Item");
        string path = Path.Combine(Application.dataPath, itemDataPath, itemName + ".json");
        string jsonData = File.ReadAllText(path);
        return JsonUtility.FromJson<Item>(jsonData);
    }

    public float GetHp() { return this.hp; }
}