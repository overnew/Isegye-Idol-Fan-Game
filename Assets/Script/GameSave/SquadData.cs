using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[System.Serializable]
public class SquadData
{
    [SerializeField] private string[] squadUnitNames;
    //private List<ContinueUnitStatus> unitSaveData = new List<ContinueUnitStatus>();
    
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
    /*
    public void LoadSquadUnit()
    {
        unitSaveData = new List<ContinueUnitStatus>();

        for (int i=0; i<squadUnitNames.Length ;++i )
        {
            unitSaveData.Add(LoadUnitStatus(squadUnitNames[i]));
        }
    }*/

    private ContinueUnitStatus LoadUnitStatus(string unitName)
    {
        string saveDataPath = Path.Combine("DataBase", "SaveData", "UnitData");
        string path = Path.Combine(Application.dataPath, saveDataPath, unitName + ".json");
        string jsonData = File.ReadAllText(path);

        return JsonUtility.FromJson<ContinueUnitStatus>(jsonData);
    }
}