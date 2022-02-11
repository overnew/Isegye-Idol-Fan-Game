using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController
{
    private const int POS_CHANGER_IDX = 4;

    private Image turnUnitIcon;
    private Text unitStatusText;
    private Text unitInfoText;
    private Text enemyStatusText;
    private Text enemyInfoText;

    private GameObject skillPanel;
    private List<SkillData> skills;
    private Button[] skillButtons;

    private GameObject itemPanel;
    private Button[] itemButtons;


    internal void Initialize()
    {
        turnUnitIcon = GameObject.Find("unitIcon").GetComponent<Image>();
        unitStatusText = GameObject.Find("unitStatus").GetComponent<Text>();
        unitInfoText = GameObject.Find("unitInfo").GetComponent<Text>();
        enemyStatusText = GameObject.Find("enemyStatus").GetComponent<Text>();
        enemyInfoText = GameObject.Find("enemyInfo").GetComponent<Text>();

        skillPanel = GameObject.Find("skillPanel");
        skillButtons = skillPanel.GetComponentsInChildren<Button>();

        itemPanel = GameObject.Find("itemPanel");
        itemButtons = itemPanel.GetComponentsInChildren<Button>();
    }

    internal void LoadTurnUnitStatus(GameObject turnUnit,UnitData turnUnitData)
    {
        turnUnitIcon.sprite = turnUnit.GetComponent<UnitInterface>().GetUnitIcon();
        unitInfoText.text = turnUnitData.GetUnitInfo();
        unitStatusText.text = turnUnitData.GetUnitStatus(turnUnit);

        skills = turnUnit.GetComponent<UnitInterface>().GetUnitSkills();
        const string path = "SkillsIcon/";

        for (int i = 0; i < POS_CHANGER_IDX; ++i)    // execept posChangerButton in last
        {
            skillButtons[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(path + skills[i].GetIconName());
            skillButtons[i].GetComponent<SkillButton>().SetSkillToButton(skills[i]);
        }

        for (int i = 0; i < itemButtons.Length; ++i)
        {
            
        }
    }

    internal void LoadEnemyStatus(GameObject enemyUnit, bool isEnter)
    {
        if (isEnter)
        {
            UnitData enemyData = enemyUnit.GetComponent<UnitInterface>().GetUnitData();
            enemyInfoText.text = enemyData.GetUnitInfo();
            enemyStatusText.text = enemyData.GetUnitStatus(enemyUnit);
            return;
        }

        enemyInfoText.text = "";
        enemyStatusText.text = "";
    }
    internal void BlockAllButton()
    {
        for (int i = 0; i < skillButtons.Length; ++i)
        {
            skillButtons[i].interactable = false;
        }
    }
    internal void OffAllSkillOutLine()
    {
        for (int i = 0; i < skillButtons.Length - 1; ++i)
        {
            skillButtons[i].GetComponent<SkillButton>().SetOutline(false);
        }
        skillButtons[skillButtons.Length - 1].GetComponent<PositionChanger>().SetOutline(false);
    }

    internal void ActivePosChangerButton() { skillButtons[POS_CHANGER_IDX].interactable = true; }
}