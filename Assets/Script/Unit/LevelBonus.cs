using System.Collections;
using UnityEngine;

[System.Serializable]
public class LevelBonus
{
    [SerializeField] public float hp;
    [SerializeField] public float stepSpeed;
    [SerializeField] public int bonusPower;
    [SerializeField] public float defense;
    [SerializeField] public float accuracy;
    [SerializeField] public float critical;
    [SerializeField] public float avoidability;

    public LevelBonus GetLevelBonus() { return this; }
}