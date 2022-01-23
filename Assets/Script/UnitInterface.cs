using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface UnitInterface
{
    public void AIBattleExecute();
    public void BuffSkillExcute(SkillData buffSkill, int roundNum);
    public void EndBuffEffect(int roundNum);
    public float GetStepSpeed();
    public float GetHp();
    public void GetDamage();

    public UnitData GetUnitData();
    public Sprite GetUnitIcon();
    public Animator GetAnimator();

    public List<SkillData> GetUnitSkills();
    public void SetTargetBar(bool setting);
    public void SetChangeBar(bool setting);
    public void SetTurnBar(bool Setting);
    public void SetUnitUIPosition();
}
