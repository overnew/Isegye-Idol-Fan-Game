using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[System.Serializable]
public class SquadData
{
    private string[] squadUnitNames;
    private List<ContinueUnitStatus> unitSaveData = new List<ContinueUnitStatus>();
   
    /*
    public GameObject[] GetSquadUnitPrefabByName()
    {

    }*/

    public void LoadSquadUnit()
    {
        unitSaveData = new List<ContinueUnitStatus>();

        for (int i=0; i<squadUnitNames.Length ;++i )
        {
            unitSaveData.Add(LoadUnitStatus(squadUnitNames[i]));
        }
    }

    private ContinueUnitStatus LoadUnitStatus(string unitName)
    {
        string saveDataPath = Path.Combine("DataBase", "SaveData", "UnitData");
        string path = Path.Combine(Application.dataPath, saveDataPath, unitName + ".json");
        string jsonData = File.ReadAllText(path);

        return JsonUtility.FromJson<ContinueUnitStatus>(jsonData);
    }
}