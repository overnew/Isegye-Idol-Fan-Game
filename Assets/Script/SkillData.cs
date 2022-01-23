using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SkillData 
{
    private const string skillDescriptionSetting = "{0}\nµ¥¹ÌÁö: {1}\n{2}";
    [SerializeField] private string name;
    [SerializeField] private int level;
    [SerializeField] private float damage;
    [SerializeField] private int upgradeDamage = 0;
    [SerializeField] private int cost;

    [SerializeField] private bool isBuff;
    [SerializeField] private string effectedStatus = "";
    [SerializeField] private float buffBonus;
    [SerializeField] private int effectedRound;
    [SerializeField] private float bonusDamage;

    [SerializeField] private bool isTargetedEnemy;
    [SerializeField] private int[] availableRange;
    [SerializeField] private int[] attackRange;
    [SerializeField] private bool isSplash;


    [SerializeField] private string skillIconName = "";
    [SerializeField] private string skillDescription = "";

    public void Upgrade()
    {
        damage += upgradeDamage;
        ++level;
    }

    private string SetSkillDescription(){ return string.Format(skillDescriptionSetting, name, damage, skillDescription);}

    public string GetIconName(){return skillIconName;}
    public float GetSkillDamage() { return damage;}
    public int[] GetAvailableRange() { return availableRange; }
    public int[] GetAttackRange() { return attackRange; }
    public bool GetIsSplashSkill() { return isSplash; }

    public bool GetIsTargetedEnemy() { return isTargetedEnemy; }

    public bool GetIsBuff() { return isBuff; }
    public float GetBuffBonus() { return buffBonus; }
    public int GetEffectedRound() { return effectedRound; }
    public string GetBuffEffectedStatus() { return effectedStatus; }
    public string GetSkillDescription() { return SetSkillDescription(); }
}
