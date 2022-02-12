using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SquadData
{
    [SerializeField] private string[] squadUnitNames;
    private Dictionary<string, UnitSaveData> unitsSaveData; 
    
    public List<GameObject> GetSquadUnitPrefabByName()
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
    
    public void LoadSquadUnit()
    {
        unitsSaveData = new Dictionary<string, UnitSaveData>();

        for (int i=0; i<squadUnitNames.Length ;++i )
        {
            unitsSaveData.Add(squadUnitNames[i], LoadUnitStatus(squadUnitNames[i]));
        }
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
}