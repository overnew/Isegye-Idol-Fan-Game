using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanelManager : MonoBehaviour
{
    private Button endButton;
    private GameObject iconGroup;

    private BattleManager battleManager;
    private SaveDataManager saveData;
    private SquadData squadData;

    private void Init()
    {
        iconGroup = GameObject.Find("IconGroup");
        endButton = gameObject.GetComponentInChildren<Button>();
        battleManager = GameObject.Find("BattleController").GetComponent<BattleManager>();
    }

    public void ClickEndButton()
    {
        battleManager.ReturnToPrevScene();
    }

    internal void DisplayBattleResult(SaveDataManager _saveData)
    {
        Init();

        this.saveData = _saveData;

        LoadAllUnitIconInPanel(battleManager.GetSquadList());
    }

    private void LoadAllUnitIconInPanel(List<GameObject> squadList)
    {
        this.squadData = saveData.GetSquadData();
        string[] nameList = squadData.GetSquadUnitsName();

        for (int i = 0; i < squadList.Count; ++i)
        {
            GameObject icon = iconGroup.transform.Find("Icon" + (i+1)).gameObject;
            icon.GetComponentInChildren<Image>().sprite = squadList[i].GetComponent<UnitInterface>().GetUnitIcon();

            UnitSaveData unitSaveData = squadData.GetUnitSaveDataByName(nameList[i]);
            icon.GetComponentInChildren<Text>().text = "Lv: " + unitSaveData.GetLevel().ToString();
            
            Image expBar = icon.transform.Find("expBarBack").Find("expBar").GetComponent<Image>();
            expBar.fillAmount = unitSaveData.GetExp() / unitSaveData.GetMaxExp();
            Debug.Log(expBar);
        }
    }

}