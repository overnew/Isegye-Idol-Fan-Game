using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private const int RANGE_START_IDX = 0;
    private const int RANGE_END_IDX = 1;

    private BattleManager battleController;
    private PanelController panelController;
    private AbilityInterface skillData;

    //스킬 설명란
    private Outline outline;
    public GameObject descFrame;    
    public Text desc;

    void Awake()
    {
        outline = GetComponent<Outline>(); 
        battleController = GameObject.Find("BattleController").GetComponent<BattleManager>();
    }

    void Start()
    {
        outline.enabled = false;
        descFrame.SetActive(false);
        panelController = battleController.GetPanelController();

    }

    public void OnClickSelect()
    {
        panelController.OffAllButtonOutLine();
        battleController.OffAllUnitsBar();
        outline.enabled = true;

        battleController.SetSelectedAbilityData(skillData, false);
        SetEnemyTargetBar(battleController.GetTargetedEnemy(skillData.GetAttackRange(), skillData.GetIsTargetedEnemy()), true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        descFrame.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descFrame.SetActive(false);
    }

    private void SetEnemyTargetBar(GameObject[] enemys, bool setting)
    {
        for (int i=0; i<enemys.Length ; ++i)
        {
           enemys[i].GetComponent<UnitInterface>().SetTargetBar(setting);
        }
    }

    public void SetSkillToButton(AbilityInterface data)
    {
        skillData = data;
        desc.text = skillData.GetAbilityDesc();
        gameObject.GetComponent<Button>().interactable = true;

        if (!CheckUnitIsAvailableRange(battleController.GetTurnUnitPosition()))
            gameObject.GetComponent<Button>().interactable = false;
    }

    private bool CheckUnitIsAvailableRange(int unitPosition)
    {
        int[] range = skillData.GetAvailableRange();

        if (unitPosition < range[RANGE_START_IDX] || range[RANGE_END_IDX] < unitPosition)
            return false;

        return true;
    }

    public void SetOutline(bool setting)
    {
        outline.enabled = setting;
    }
}
