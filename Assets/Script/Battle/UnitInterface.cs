using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface UnitInterface
{
    public void SetUnitSaveData(UnitSaveData unitSaveData);
    public UnitSaveData GetUnitSaveData();
    public void AIBattleExecute();
    public void BuffSkillExcute(AbilityInterface buffSkill, int roundNum);
    public void CheckEndedEffect(int roundNum);


    public void DisplayCondition(string text, string textColor);
    public void UndisplayCondition();
    public float GetStepSpeed();
    public float GetHp();
    public void GetDamage();

    public UnitData GetUnitData();
    public Sprite GetUnitIcon();
    public Animator GetAnimator();

    public List<SkillData> GetUnitSkills();

    public GameObject GetTauntUnit();
    public void SetTauntIcon(bool setting);
    public bool GetIsTaunt();
    public void SetTargetBar(bool setting);
    public void SetChangeBar(bool setting);
    public void SetTurnBar(bool Setting);
    public void SetUnitUIPosition();
}
