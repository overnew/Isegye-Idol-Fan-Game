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
        totalExp = 0;

        for (int i = 0; i < enemyNames.Length; ++i)
        {
            squadList.Add(GetPrefabByName(enemyNames[i]));
            totalExp += squadList[i].GetComponent<UnitInterface>().GetUnitData().GetRewardExp();
        }
        return squadList;
    }

    private GameObject GetPrefabByName(string prefabName)
    {
        string prefabPath = Path.Combine("Prefab", "EnemyUnit",prefabName);
        return Resources.Load<GameObject>(prefabPath);
    }

    public float GetTotalRewardExp() { return this.totalExp; }
}