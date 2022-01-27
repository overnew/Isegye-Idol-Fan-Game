using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Reflection;

[System.Serializable]
public class UnitData
{
    [SerializeField] private string unitName;
    [SerializeField] private bool isEnemyUnit;
    [SerializeField] private float maxHp;
    [SerializeField] private float stepSpeed;
    [SerializeField] private int[] attackPowerRange;
    [SerializeField] private float bonusPower;
    [SerializeField] private float defense;
    [SerializeField] private float accuracy;
    [SerializeField] private float critical;
    [SerializeField] private float avoidability;

    [SerializeField] private string[] skillNames;
    [SerializeField] private string unitIconName;
    private OriginStatus originStatus;

    private const string skillDataPath = "DataBase/Skills";
    private const string statusDescriptionSetting = "{0}\nü��: {1}\n���� �ӵ�: {2}\n������: {3} - {4}\n����: {5}\n���߷�: {6}%\nȸ����: {7}%\nġ��Ÿ Ȯ��: {8}%";
    private const string colorStatusSetting = "<color={0}>{1}</color>";
    private const string BUFF_COLOR = "#4BE198";
    private const string DEBUFF_COLOR = "#FE4554";
    

    private struct OriginStatus
    {
        float stepSpeed;
        float bonusPower;
        float defense;
        float accuracy;
        float critical;
        float avoidability;

        public OriginStatus(UnitData unitData)
        {
            this.stepSpeed = unitData.stepSpeed;
            this.bonusPower = unitData.bonusPower;
            this.defense = unitData.defense;
            this.accuracy = unitData.accuracy;
            this.critical = unitData.critical;
            this.avoidability = unitData.avoidability;
        }

        public float GetStatusValueByName(string statusName)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            return (float)this.GetType().GetField(statusName, bindingFlags).GetValue(this);
        }
    }

    public List<SkillData> LoadSkillData(UnitData unitData)
    {
        originStatus = new OriginStatus(unitData);
        List<SkillData> skillsData = new List<SkillData>();
        for (int i = 0; i <skillNames.Length; ++i)
        {
            skillsData.Add(LoadSkillDataFromJson(skillNames[i]));
        }
        return skillsData;
    }
    private SkillData LoadSkillDataFromJson(string skillName)
    {
        string path = Path.Combine(Application.dataPath, skillDataPath, skillName + ".json");
        string jsonData = File.ReadAllText(path);
        return JsonUtility.FromJson<SkillData>(jsonData);
    }

    public void ApplyBuffEffect(SkillData skillData, bool isBuffCancle)
    {
        var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
        string effectedStatus = skillData.GetBuffEffectedStatus();
        float statusValue = (float)this.GetType().GetField(effectedStatus, bindingFlags).GetValue(this);
        float buffValue = skillData.GetBuffBonus();

        if (isBuffCancle)
            buffValue *= -1;

        this.GetType().GetField(effectedStatus, bindingFlags).SetValue(this, statusValue + buffValue);
    }

    public string GetName() { return unitName; }
    public string GetUnitIconName() { return unitIconName; }
    public float GetMaxHp() { return maxHp; }
    public bool GetIsEnemy() { return isEnemyUnit; }
    public float GetStepSpeed() { return stepSpeed; }
    public int[] GetAttackPower() { return attackPowerRange; }
    public float GetBonusPower() { return bonusPower; }

    public float GetDefense() { return defense; }
    public float GetAccuracy() { return accuracy; }
    public float GetCritical() { return critical; }
    public float GetAvoidability() { return avoidability; }

    private string ApplyColorToStatus(string statusName)
    {
        var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
        float statusValue = (float)this.GetType().GetField(statusName, bindingFlags).GetValue(this);
        float oringValue = originStatus.GetStatusValueByName(statusName);

        if (statusValue < oringValue)
            return string.Format(colorStatusSetting, DEBUFF_COLOR, statusValue);
        else if (statusValue == oringValue)
            return statusValue.ToString();
        else 
            return string.Format(colorStatusSetting, BUFF_COLOR, statusValue);
    }

    public string GetUnitStatus() { return string.Format(statusDescriptionSetting,unitName, maxHp,
        ApplyColorToStatus("stepSpeed"),
        attackPowerRange[0], attackPowerRange[1],
        ApplyColorToStatus("defense"),
        ApplyColorToStatus("accuracy"),
        ApplyColorToStatus("avoidability"),
        ApplyColorToStatus("critical")); 
    }

}
