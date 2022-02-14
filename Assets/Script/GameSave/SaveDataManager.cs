using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataManager
{
    private SquadData squadData;
    
    public SaveDataManager()   //��ü ������ ���̺� ���� �ε�
    {
        LoadSquadData();
    }
    private void LoadSquadData()
    {
        string saveDataPath = Path.Combine("DataBase", "SaveData");
        string path = Path.Combine(Application.dataPath, saveDataPath, "squadData" + ".json");
        string jsonData = File.ReadAllText(path);

        squadData = JsonUtility.FromJson<SquadData>(jsonData);
        squadData.Init();
    }

    public SquadData GetSquadData() { return this.squadData; }
}
