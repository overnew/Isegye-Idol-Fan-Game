using System.Collections;
using UnityEngine;

[System.Serializable]
public class LevelBonus : MonoBehaviour
{
    [SerializeField] private float hp;
    [SerializeField] private float stepSpeed;
    [SerializeField] private int[] attackPowerRange;
    [SerializeField] private float bonusPower;
    [SerializeField] private float defense;
    [SerializeField] private float accuracy;
    [SerializeField] private float critical;
    [SerializeField] private float avoidability;

    public LevelBonus GetLevelBonus() { return this; }
}