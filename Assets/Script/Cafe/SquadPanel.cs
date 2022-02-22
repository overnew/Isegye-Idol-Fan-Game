using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SquadPanel : MonoBehaviour, PanelInterface
{
    private CafeManager cafeManager;
    private SquadItemPanel itemPanel;

    private Item selectedItem;

    public Image unitIcon;
    public Text unitInfoText;
    public Text unitStatusText;
    internal void Init(CafeManager _cafeManager)
    {
        unitIcon.enabled = false;
        itemPanel = GameObject.Find("SquadItemPanel").GetComponent<SquadItemPanel>();
        this.cafeManager = _cafeManager;
    }

    internal void SetUsingItem(Item _item)
    {
        this.selectedItem = _item;
        cafeManager.TurnUnitButtonOn();
    }

    public void LoadUnitStatusText(GameObject unit, bool isEnter)
    {
        unitIcon.enabled = true;
        unitIcon.sprite = unit.GetComponent<UnitInterface>().GetUnitIcon();

        UnitData unitData = unit.GetComponent<UnitInterface>().GetUnitData();
        unitInfoText.text = unitData.GetUnitInfo();
        unitStatusText.text = unitData.GetUnitStatus(unit);
        return;
    }

    internal Item GetSelectedItem() { return this.selectedItem; }
}