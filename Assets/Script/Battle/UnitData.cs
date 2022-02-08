using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

[System.Serializable]
public class UnitData
{
    [SerializeField] private string unitName;
    [SerializeField] private string unitType;
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
    private const string statusDescriptionSetting = "{0}\n���� �ӵ�: {1}\n������: {2} - {3}\n����: {4}\n���߷�: {5}%\nȸ����: {6}%\nġ��Ÿ Ȯ��: {7}%";
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
        float buffValue = skillData.GetEffectValue();

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
    
    private string GetCurrnetHpWithMaxHp(float currentHp)
    {
        StringBuilder unitHp = new StringBuilder("<color=#FE4554>ü��: ");
        unitHp.Append(currentHp + " / " +maxHp);
        unitHp.Append("</color>");
        return unitHp.ToString();
    }

    public string GetUnitInfo()
    {
        StringBuilder unitInfo = new StringBuilder(unitName);
        unitInfo.Append("\n<color=");

        if (isEnemyUnit)
            unitInfo.Append(DEBUFF_COLOR);
        else
            unitInfo.Append(BUFF_COLOR);

        unitInfo.Append("><size=28>" +  unitType + "</size></color>");
        return unitInfo.ToString();
    }

    public string GetUnitStatus(GameObject unit) { return string.Format(statusDescriptionSetting,
        GetCurrnetHpWithMaxHp(unit.GetComponent<UnitInterface>().GetHp()),
        ApplyColorToStatus("stepSpeed"),
        attackPowerRange[0], attackPowerRange[1],
        ApplyColorToStatus("defense"),
        ApplyColorToStatus("accuracy"),
        ApplyColorToStatus("avoidability"),
        ApplyColorToStatus("critical")); 
    }

}