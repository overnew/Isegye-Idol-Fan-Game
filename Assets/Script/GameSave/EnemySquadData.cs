using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;

[System.Serializable]
public class EnemySquadData
{
    [SerializeField] private string[] enemyNames;
    private float totalExp =0;

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


    public void SetTotalRewardExp(List<GameObject> enemyList) 
    {
        totalExp = 0;
        foreach (GameObject unit in enemyList)
        {
            totalExp += unit.GetComponent<UnitInterface>().GetUnitData().GetRewardExp();
        }
    }

    public float GetTotalRewardExp() { return this.totalExp; }
}