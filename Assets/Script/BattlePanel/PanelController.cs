using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : PanelInterface
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

    private SquadData squadData;
    private GameObject itemPanel;
    private Button[] itemButtons;

    internal PanelController(SquadData _squadData)
    {
        Initialize(_squadData);
    }

    internal void Initialize(SquadData _squadData)
    {
        turnUnitIcon = GameObject.Find("unitIcon").GetComponent<Image>();
        unitStatusText = GameObject.Find("unitStatus").GetComponent<Text>();
        unitInfoText = GameObject.Find("unitInfo").GetComponent<Text>();
        enemyStatusText = GameObject.Find("enemyStatus").GetComponent<Text>();
        enemyInfoText = GameObject.Find("enemyInfo").GetComponent<Text>();

        skillPanel = GameObject.Find("skillPanel");
        skillButtons = skillPanel.GetComponentsInChildren<Button>();

        this.squadData = _squadData;
        itemPanel = GameObject.Find("itemPanel");
        itemButtons = itemPanel.GetComponentsInChildren<Button>();
        ItemPanelLoad();
    }

    internal void LoadTurnUnitStatus(GameObject turnUnit,UnitData turnUnitData)
    {
        turnUnitIcon.sprite = turnUnit.GetComponent<UnitInterface>().GetUnitIcon();
        unitInfoText.text = turnUnitData.GetUnitInfo();
        unitStatusText.text = turnUnitData.GetUnitStatus(turnUnit);

        SkillPanelLoad(turnUnit);
        SetItemButtonInteractable();
    }

    private void SkillPanelLoad(GameObject turnUnit)
    {
        skills = turnUnit.GetComponent<UnitInterface>().GetUnitSkills();
        const string skillIconPath = "SkillsIcon/";

        for (int i = 0; i < POS_CHANGER_IDX; ++i)    // execept posChangerButton in last
        {
            skillButtons[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(skillIconPath + skills[i].GetIconName());
            skillButtons[i].GetComponent<SkillButton>().SetSkillToButton(skills[i]);
        }
    }

    private void SetItemButtonInteractable()
    {
        for (int i = 0; i < itemButtons.Length; ++i)
        {
            if (itemButtons[i].GetComponent<ItemButton>().GetRemainNum() <= 0)
                itemButtons[i].interactable = false;
            else
                itemButtons[i].interactable = true;
        }
    }

    private void ItemPanelLoad()
    {
        const string itemIconPath = "ItemIcon/";
        Dictionary<Item, int> itemDictionary = squadData.GetItemDictionary();

        int buttonIdx = 0;
        foreach (KeyValuePair<Item, int> itemPair in itemDictionary)
        {
            itemButtons[buttonIdx].GetComponent<Image>().sprite = Resources.Load<Sprite>(itemIconPath + itemPair.Key.GetIconName());
            itemButtons[buttonIdx++].GetComponent<ItemButton>().SetItemToButton(itemPair.Key, itemPair.Value);
        }
        
        for (int i = buttonIdx; i< itemButtons.Length ; ++buttonIdx )
        {
            itemButtons[buttonIdx].enabled = false;
        }
    }

    internal void ApplyItemUse(Item item)
    {
        squadData.ApplyItemUse(item);

        Dictionary<Item, int> itemDictionary = squadData.GetItemDictionary();
        int buttonIdx = 0;

        foreach (KeyValuePair<Item, int> itemPair in itemDictionary)
        {
            if (itemPair.Key.Equals(item))  //사용한 아이템의 개수 줄여주기 적용
            {
                itemButtons[buttonIdx].GetComponent<ItemButton>().SetRemainNum(itemPair.Value.ToString());
                break;
            }
            ++buttonIdx;
        }
    }

    public void LoadUnitStatusText(GameObject unit, bool isEnter)
    {
        if (isEnter)
        {
            UnitData enemyData = unit.GetComponent<UnitInterface>().GetUnitData();
            enemyInfoText.text = enemyData.GetUnitInfo();
            enemyStatusText.text = enemyData.GetUnitStatus(unit);
            return;
        }

        enemyInfoText.text = "";
        enemyStatusText.text = "";
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

        for (int i = 0; i < itemButtons.Length; ++i)
        {
            itemButtons[i].interactable = false;
        }
    }
    internal void OffAllButtonOutLine()
    {
        for (int i = 0; i < skillButtons.Length - 1; ++i)
        {
            skillButtons[i].GetComponent<SkillButton>().SetOutline(false);
        }
        skillButtons[skillButtons.Length - 1].GetComponent<PositionChanger>().SetOutline(false);

        for (int i = 0; i < itemButtons.Length; ++i)
        {
            itemButtons[i].GetComponent<ItemButton>().SetOutline(false);
        }
    }

    internal void ActivePosChangerButton() { skillButtons[POS_CHANGER_IDX].interactable = true; }
}