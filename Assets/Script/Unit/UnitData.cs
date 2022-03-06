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
    [SerializeField] private int rewardGold = 0;
    [SerializeField] private float rewardExp = 0;

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
    private const string statusDescriptionSetting = "{0}\n스텝 속도: {1}\n데미지: {2} - {3}\n방어력: {4}\n명중률: {5}%\n회피율: {6}%\n치명타 확률: {7}%";
    private const string colorStatusSetting = "<color={0}>{1}</color>";
    private const string BUFF_COLOR = "#4BE198";
    private const string DEBUFF_COLOR = "#FE4554";

    private const string dataBasePath = "DataBase";
    private const string saveDataPath = "SaveData\\UnitData";

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

    public List<SkillData> LoadSkillData()
    {
        Init();
        
        List<SkillData> skillsData = new List<SkillData>();
        for (int i = 0; i <skillNames.Length; ++i)
        {
            skillsData.Add(LoadSkillDataFromJson(skillNames[i]));
        }
        return skillsData;
    }

    private void Init()
    {
        originStatus = new OriginStatus(this);
    }

    public void ApplyLevelUpBonus(int upLevel)
    {
        for (; upLevel>0 ;--upLevel )
        {
            ApplyLevelBonus();
        }

        SaveUnitDataToJson();
    }
    private void ApplyLevelBonus()  //LevelUp 했을때 적용시키기
    {
        if (isEnemyUnit)
            return;

        LevelBonus lvBonus = GetLevelBonus();
        this.maxHp += lvBonus.hp;
        this.stepSpeed += lvBonus.stepSpeed;

        this.attackPowerRange[0] += lvBonus.bonusPower;
        this.attackPowerRange[1] += lvBonus.bonusPower;

        this.defense += lvBonus.defense;

        this.accuracy += lvBonus.accuracy;
        if (accuracy >= 100) accuracy = 100;

        this.critical += lvBonus.critical;
        if (critical >= 100) critical = 100;

        this.avoidability += lvBonus.avoidability;
        if (avoidability >= 100) avoidability = 100;
    }

    private LevelBonus GetLevelBonus()
    {
        string lvDataName = this.unitIconName + "LevelBonusData.json";
        string path = Path.Combine(Application.dataPath, dataBasePath, saveDataPath, lvDataName);
        string jsonData = File.ReadAllText(path);

        return JsonUtility.FromJson<LevelBonus>(jsonData);
    }

    private void SaveUnitDataToJson()
    {
        string dataPath = Path.Combine("DataBase", "SaveData", "UnitData");
        string dataFileName = this.unitIconName + "UnitData.json";

        string jsonData = JsonUtility.ToJson(this, true);
        string path = Path.Combine(Application.dataPath, dataPath, dataFileName);
        File.WriteAllText(path, jsonData);
    }

    private SkillData LoadSkillDataFromJson(string skillName)
    {
        string path = Path.Combine(Application.dataPath, skillDataPath, skillName + ".json");
        string jsonData = File.ReadAllText(path);
        return JsonUtility.FromJson<SkillData>(jsonData);
    }

    public void ApplyBuffEffect(AbilityInterface skillData, bool isBuffCancle)
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
    public bool GetIsEnemy() { return isEnemyUnit; }

    public float GetRewardExp() { return rewardExp; }

    public int GetRewardGold() { return rewardGold; }
    public float GetMaxHp() { return maxHp; }
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
        StringBuilder unitHp = new StringBuilder("<color=#FE4554>체력: ");
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
