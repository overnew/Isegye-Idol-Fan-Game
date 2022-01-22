using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PositionChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private BattleController battleController;
    public SkillData skillData;

    //스킬 설명란
    public GameObject descFrame;
    public Text desc;
    private const string descInfo = "유닛의 위치를 바꿉니다.";

    void Start()
    {
        desc.text = descInfo;
        descFrame.SetActive(false);
    }

    public void OnClickPositionChanger()
    {
        battleController = GameObject.Find("BattleController").GetComponent<BattleController>();
        battleController.OffAllTargerBar();
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
}
