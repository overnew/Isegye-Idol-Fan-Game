using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanelManager : MonoBehaviour
{
    private const string LEVEL = "Lv: ";
    private GameObject iconGroup;
    private Button endButton;
    private Text resultText;
    private Text rewardText;

    private BattleManager battleManager;
    private SaveDataManager saveData;
    private SquadData squadData;

    private List<UnitSaveData> prevUnitSaveDataList = new List<UnitSaveData>();
    private List<UnitSaveData> unitSaveDataList = new List<UnitSaveData>();
    private List<Text> levelTextList = new List<Text>();
    private List<Image> expBarList = new List<Image>();

    private void Init()
    {
        iconGroup = GameObject.Find("IconGroup");
        endButton = gameObject.GetComponentInChildren<Button>();

        Text[] texts = gameObject.GetComponentsInChildren<Text>();
        rewardText = texts[0];
        resultText = texts[1];

        battleManager = GameObject.Find("BattleController").GetComponent<BattleManager>();
    }

    public void ClickEndButton()
    {
        battleManager.ReturnToPrevScene();
    }

    internal void DisplayBattleResult(SaveDataManager _saveData, EnemySquadData enemyData, List<GameObject> squadList)
    {
        Init();
        this.saveData = _saveData;
        this.squadData = saveData.GetSquadData();
        saveData.AddBalance(enemyData.GetTotalRewardGold());

        float dividedExp = (float)System.Math.Ceiling((double)(enemyData.GetTotalRewardExp() / squadList.Count));

        SaveBattleResult(squadList, dividedExp);

        LoadAllUnitIconInPanel(squadList);

        rewardText.text = "보상:   +" + enemyData.GetTotalRewardGold().ToString() + "G"
            + "\n" + _saveData.GetBalance() + "G";

        StartCoroutine(ResultAnimation(dividedExp));
    }

    private void SaveBattleResult(List<GameObject> squadList, float dividedExp)
    {
        SaveSquadUnitSaveData(squadList,dividedExp);

        squadData.SaveSquadData(squadList, unitSaveDataList);
    }

    private void SaveSquadUnitSaveData(List<GameObject> squadList, float dividedExp)
    {
        unitSaveDataList = new List<UnitSaveData>();

        for (int i = 0; i < squadList.Count; ++i)
        {
            UnitSaveData prevSaveData = squadList[i].GetComponent<UnitInterface>().GetUnitSaveData();
            prevUnitSaveDataList.Add(prevSaveData);
            
            KeyValuePair<int, float> resultPair = GetResultLevelAndExpPair(prevSaveData, dividedExp);

            UnitSaveData resultSaveData = new UnitSaveData(squadList[i].GetComponent<UnitInterface>().GetHp(), resultPair.Key, resultPair.Value);

            unitSaveDataList.Add(resultSaveData);
        }

    }

    private KeyValuePair<int, float> GetResultLevelAndExpPair(UnitSaveData saveData, float addedExp)
    {
        float currnetExp = saveData.GetExp();
        float totalExp = currnetExp + addedExp;

        int currentLevel = saveData.GetLevel();
        int finalLevel = currentLevel;
        float maxExpSum = Utils.LEVEL_MAX_EXP[finalLevel];

        while (maxExpSum <= totalExp)
        {
            maxExpSum += Utils.LEVEL_MAX_EXP[++finalLevel];
        }

        float finalExp = totalExp - (maxExpSum - Utils.LEVEL_MAX_EXP[finalLevel]);

        return new KeyValuePair<int, float>(finalLevel, finalExp);
    }

    private void LoadAllUnitIconInPanel(List<GameObject> squadList)
    {
        string[] nameList = squadData.GetSquadUnitsName();

        for (int i = 0; i < squadList.Count; ++i)
        {
            GameObject icon = iconGroup.transform.Find("Icon" + (i+1)).gameObject;
            icon.GetComponentInChildren<Image>().sprite = squadList[i].GetComponent<UnitInterface>().GetUnitIcon();

            UnitSaveData unitSaveData = unitSaveDataList[i];

            Text levelText = icon.GetComponentInChildren<Text>();
            levelText.text = "Lv: " + unitSaveData.GetLevel().ToString();
            levelTextList.Add(levelText);

            float maxExp = Utils.LEVEL_MAX_EXP[unitSaveData.GetLevel()];
            Image expBar = icon.transform.Find("expBarBack").Find("expBar").GetComponent<Image>();
            expBar.fillAmount = unitSaveData.GetExp() / maxExp;
            expBarList.Add(expBar);
        }
    }
    
    private IEnumerator ResultAnimation(float dividedExp)
    {
        yield return new WaitForSeconds(1f);    //잠시 대기

        for (int i=0; i< prevUnitSaveDataList.Count ; ++i)
        {
            StartCoroutine(ExpBarAnimation(prevUnitSaveDataList[i], expBarList[i],levelTextList[i] ,dividedExp));
        }
    }
    
    private IEnumerator ExpBarAnimation(UnitSaveData saveData,Image expBar ,Text lvText ,float addedExp)
    {
        float currnetExp = saveData.GetExp();
        float totalExp = currnetExp + addedExp;

        int currentLevel = saveData.GetLevel();
        int finalLevel = currentLevel;
        float maxExpSum = Utils.LEVEL_MAX_EXP[finalLevel];

        while (maxExpSum <= totalExp)
        {
            maxExpSum += Utils.LEVEL_MAX_EXP[++finalLevel];
        }

        float finalExp = totalExp - (maxExpSum - Utils.LEVEL_MAX_EXP[finalLevel]);

        const float div = 50f;
        float speed = addedExp / div;
        float currentLevelMaxExp = Utils.LEVEL_MAX_EXP[currentLevel];
        int addedCnt = 0;

        while (true)
        {
            while (currnetExp < currentLevelMaxExp && addedCnt < div)
            {
                expBar.fillAmount = currnetExp / currentLevelMaxExp;
                currnetExp += speed;
                ++addedCnt;
                yield return null;
            }

            if (currentLevel == finalLevel)
                break;

            currnetExp = currnetExp - currentLevelMaxExp;
            currentLevelMaxExp = Utils.LEVEL_MAX_EXP[++currentLevel];
            lvText.text = LEVEL + currentLevel.ToString();
            expBar.fillAmount = currnetExp / currentLevelMaxExp;
        }
        
    }

}