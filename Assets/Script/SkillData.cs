using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

[System.Serializable]
public class SkillData 
{
    private const string skillDescriptionSetting = "{0}\n{1}\nµ¥¹ÌÁö: {2}\n{3}";
    [SerializeField] private string name;
    [SerializeField] private int level;
    [SerializeField] private float damage;
    [SerializeField] private int upgradeDamage = 0;
    [SerializeField] private int cost;

    [SerializeField] private bool isBuff;
    [SerializeField] private string effectedStatus = "";
    [SerializeField] private float buffBonus;
    [SerializeField] private int effectedRound;

    [SerializeField] private bool isPosChanger;

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

    private string RangeVisualToString()
    {
        StringBuilder rangeText = new StringBuilder();

        for (int i=3; i>=0 ;--i )
        {
            if (availableRange[0] <= i && i <= availableRange[1])
                rangeText.Append("<color=#F1C600>o</color>");
            else
                rangeText.Append("<color=#919191>o</color>");
        }

        rangeText.Append("   ");

        for (int i = 0; i <4; ++i)
        {
            if (attackRange[0] <= i && i <= attackRange[1])
                rangeText.Append("<color=#FE4554>o</color>");
            else
                rangeText.Append("<color=#919191>o</color>");
        }

        return rangeText.ToString();
    }

    private string SetSkillDescription(){ return string.Format(skillDescriptionSetting,RangeVisualToString() ,name, damage, skillDescription);}

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

    public bool GetIsPosChanger() { return isPosChanger; }
    public string GetSkillDescription() { return SetSkillDescription(); }
}
