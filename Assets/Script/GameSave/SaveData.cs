using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveData
{
    private SquadData squadData;

    public SaveData()   //객체 생성시 세이브 파일 로드
    {
        LoadSquadData();
    }
    private void LoadSquadData()
    {
        string saveDataPath = Path.Combine("DataBase", "SaveData");
        string path = Path.Combine(Application.dataPath, saveDataPath, "squadData" + ".json");
        string jsonData = File.ReadAllText(path);

        squadData = JsonUtility.FromJson<SquadData>(jsonData);
        //squadData.LoadSquadUnit();
    }

    public SquadData GetSquadData() { return this.squadData; }
}
