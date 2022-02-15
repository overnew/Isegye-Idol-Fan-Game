using UnityEditor;
using UnityEngine;

public interface AbilityInterface 
{
    public string GetIconName();
    public float GetSkillDamage();
    public int[] GetAvailableRange();
    public int[] GetAttackRange();
    public bool GetIsSplashSkill();

    public bool GetIsTargetedEnemy();

    public bool GetIsBuff();
    public float GetEffectValue();
    public int GetEffectedRound();
    public string GetBuffEffectedStatus();

    public bool GetIsPosChanger();
    public string GetAbilityDesc();

}