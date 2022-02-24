using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanelManager : MonoBehaviour
{
    private Button endButton;
    private Image[] unitIcons;

    private BattleManager battleManager;
    private SaveDataManager saveData;

    private void Init()
    {
        unitIcons = GameObject.Find("IconGroup").GetComponentsInChildren<Image>();
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
        for (int i=0; i<squadList.Count ;++i )
        {
            unitIcons[i].sprite = squadList[i].GetComponent<UnitInterface>().GetUnitIcon();
        }
    }

}