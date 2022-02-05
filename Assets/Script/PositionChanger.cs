using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PositionChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private BattleController battleController;
    private PanelController panelController;
    public SkillData skillData;

    //스킬 설명란
    private Outline outline;
    public GameObject descFrame;
    public Text desc;
    private const string descInfo = "유닛의 위치를 바꿉니다.";
    void Awake()
    {
        outline = GetComponent<Outline>();
    }
    void Start()
    {
        outline.enabled = false;

        desc.text = descInfo;
        descFrame.SetActive(false);

        battleController = GameObject.Find("BattleController").GetComponent<BattleController>();
        panelController = battleController.GetPanelController();
    }

    public void OnClickPositionChanger()
    {
        panelController.OffAllSkillOutLine();
        outline.enabled = true;

        battleController.OffAllUnitsBar();
        battleController.SetPosChanger(true);

        GameObject turnUnit = battleController.GetTurnUnit();
        GameObject[] squad = battleController.GetOurSquad();

        for (int i=0; i<squad.Length ; ++i)
        {
            if (!squad[i].Equals(turnUnit))
                squad[i].GetComponent<UnitInterface>().SetChangeBar(true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        descFrame.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descFrame.SetActive(false);
    }
    public void SetOutline(bool setting)
    {
        outline.enabled = setting;
    }
}
