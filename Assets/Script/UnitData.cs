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

    private const string skillDataPath = "DataBase/Skills";
    private const string statusDescriptionSetting = "{0}\n체력: {1}\n스텝 속도: {2}\n데미지: {3} - {4}\n방어력: {5}\n명중률: {6}%\n회피율: {7}%\n치명타 확률: {8}%";

    public List<SkillData> LoadSkillData()
    {
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

    public string GetUnitStatus() { return string.Format(statusDescriptionSetting,unitName, maxHp,stepSpeed,attackPowerRange[0], attackPowerRange[1],
        defense, accuracy, avoidability, critical); }
}
