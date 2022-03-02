using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;

[System.Serializable]
public class EnemySquadData
{
    [SerializeField] private string[] enemyNames;
    private float totalExp =0;
    private int totalGold = 0;

    public List<GameObject> GetSquadUnitPrefabs()
    {
        List<GameObject> squadList = new List<GameObject>();

        for (int i = 0; i < enemyNames.Length; ++i)
        {
            squadList.Add(GetPrefabByName(enemyNames[i]));
        }

        return squadList;
    }

    private GameObject GetPrefabByName(string prefabName)
    {
        string prefabPath = Path.Combine("Prefab", "EnemyUnit",prefabName);
        return Resources.Load<GameObject>(prefabPath);
    }


    public void SetTotalReward(List<GameObject> enemyList) 
    {
        totalExp = totalGold = 0;
        foreach (GameObject unit in enemyList)
        {
            UnitData unitData = unit.GetComponent<UnitInterface>().GetUnitData();
            totalExp += unitData.GetRewardExp();
            totalGold += unitData.GetRewardGold();
        }
    }

    public float GetTotalRewardExp() { return this.totalExp; }
    public int GetTotalRewardGold() { return this.totalGold; }
}