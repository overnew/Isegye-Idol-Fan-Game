using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SquadPanel : MonoBehaviour, PanelInterface
{
    private CafeManager cafeManager;
    private SquadItemPanel itemPanel;

    private Item selectedItem;
    private SquadItemButton selectedButton;

    public Image unitIcon;
    public Text unitInfoText;
    public Text unitStatusText;

    internal void Init(CafeManager _cafeManager)
    {
        unitIcon.enabled = false;
        itemPanel = GameObject.Find("SquadItemPanel").GetComponent<SquadItemPanel>();
        this.cafeManager = _cafeManager;
    }

    internal void SetUsingItem(Item _item, SquadItemButton _button)
    {
        this.selectedButton = _button;
        this.selectedItem = _item;
        cafeManager.TurnUnitButtonOn();
    }

    internal void ItemUseExecute()
    {
        selectedButton.DealExecute();
        selectedButton.SetOutline(false);
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