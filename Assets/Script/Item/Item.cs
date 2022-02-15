using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[System.Serializable]
public class Item : AbilityInterface
{
    private const int RANGE_START_IDX = 0;
    private const int RANGE_END_IDX = 1;
    private const string ITEM_DESC_SETTING = "{0}\n{1}\nµ¥¹ÌÁö: {2}\n\n{3}";

    [SerializeField] private string name;
    [SerializeField] private float damage;

    [SerializeField] private bool isBuff;
    [SerializeField] private string effectedStatus;
    [SerializeField] private float effectValue;
    [SerializeField] private int effectedRound;

    [SerializeField] private bool isPosChanger;

    [SerializeField] private bool isTargetedEnemy;
    [SerializeField] private int[] availableRange;
    [SerializeField] private int[] attackRange;
    [SerializeField] private bool isSplash;

    [SerializeField] private string iconName;
    [SerializeField] private string itemDescription;

    public bool Equal(object obj)
    {
        if (((Item)obj).GetIconName().Equals(this.iconName))
            return true;
        return false;
    }

    private string RangeVisualToString()
    {
        StringBuilder rangeText = new StringBuilder();

        for (int i = 0; i < 4; ++i)
        {
            if (attackRange[RANGE_START_IDX] <= i && i <= attackRange[RANGE_END_IDX])
                rangeText.Append("<color=#FE4554>o</color>");
            else
                rangeText.Append("<color=#919191>o</color>");
        }

        return rangeText.ToString();
    }

    private string SetItemDescription() { return string.Format(ITEM_DESC_SETTING, RangeVisualToString(), name, damage, itemDescription); }

    public float GetSkillDamage() { return this.damage; }
    public int[] GetAvailableRange() { return this.availableRange; }
    public int[] GetAttackRange() { return attackRange; }
    public bool GetIsSplashSkill() { return isSplash; }

    public bool GetIsTargetedEnemy() { return isTargetedEnemy; }

    public bool GetIsBuff() { return this.isBuff; }
    public float GetEffectValue() { return effectValue; }
    public int GetEffectedRound() { return effectedRound; }
    public string GetBuffEffectedStatus() { return effectedStatus; }

    public bool GetIsPosChanger() { return this.isPosChanger; }
    public string GetAbilityDesc() { return SetItemDescription(); }

    public string GetIconName() { return this.iconName; }
}
