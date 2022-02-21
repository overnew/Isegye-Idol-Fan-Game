using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SquadItemButton : MonoBehaviour, ButtonInterface
{
    public Text remainText;

    private CafePanel cafePanel;
    private SquadItemPanel squadPanel;
    private Item item;
    private Outline outline;

    private bool isShoppingButton = false;
    private int buttonIndex;
    private int remainNum;

    public void OnClickItem()
    {
        outline.enabled = true;
        cafePanel.SetSelectdItem(item, this);
    }

    internal void Init(CafePanel _cafePanel, SquadItemPanel _squadPanel, int _buttonIndex)
    {
        this.cafePanel = _cafePanel;
        this.squadPanel = _squadPanel;

        this.buttonIndex = _buttonIndex;
        this.outline = gameObject.GetComponent<Outline>();
    }

    internal void SetItemToButton(Item _item,int _remainNum)
    {
        this.item = _item;
        this.remainNum = _remainNum;

        outline.enabled = false;
        if (remainNum == 0)
        {
            remainText.text = "";
            return;
        }

        remainText.text = this.remainNum.ToString();
    }

    public bool GetIsShoppingButton() { return this.isShoppingButton; }
    public int GetPrice() { return item.GetPrice() / 2; }
    public int GetRemainNumber() { return this.remainNum; }

    public void DealExecute()
    {
        --remainNum;
        remainText.text = remainNum.ToString();

        squadPanel.ApplyChangeInItemList(this.buttonIndex, -1);

        if(remainNum <= 0)
            gameObject.GetComponent<Button>().interactable = false;
    }

    public void SetOutline(bool setting)
    {
        outline.enabled = setting;
    }
}