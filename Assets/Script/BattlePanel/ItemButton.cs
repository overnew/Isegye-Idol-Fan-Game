using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private BattleManager battleController;
    private PanelController panelController;
    private AbilityInterface itemData;

    //스킬 설명란
    private Outline outline;
    public GameObject descFrame;
    public Text desc;
    public Text remainNum;

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

        battleController.SetSelectedAbilityData(itemData, true);
        SetEnemyTargetBar(battleController.GetTargetedEnemy(itemData.GetAttackRange(), itemData.GetIsTargetedEnemy()), true);
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
        for (int i = 0; i < enemys.Length; ++i)
        {
            enemys[i].GetComponent<UnitInterface>().SetTargetBar(setting);
        }
    }

    public void SetItemToButton(AbilityInterface data, int itemNum)
    {
        itemData = data;
        desc.text = itemData.GetAbilityDesc();
        remainNum.text = itemNum.ToString();
        gameObject.GetComponent<Button>().interactable = true;
    }

    public void SetOutline(bool setting)
    {
        outline.enabled = setting;
    }

    internal void SetRemainNum(string num) { remainNum.text = num; }
    internal int GetRemainNum() { return Int32.Parse(this.remainNum.text); }
}