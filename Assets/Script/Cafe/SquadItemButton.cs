using UnityEngine;
using UnityEngine.UI;

public class SquadItemButton : MonoBehaviour, ButtonInterface
{
    public Text remainText;

    private ShoppingPanel shoppingPanel;
    private SquadPanel squadPanel;
    private SquadItemPanel itemPenel;
    private Item item;
    private Outline outline;

    private bool isShoppingButton = false;
    private int buttonIndex;
    private int remainNum;

    public void OnClickItem()
    {
        outline.enabled = true;
        if (shoppingPanel.GetIsShoppingMode())
        {
            shoppingPanel.SetSelectdItem(item, this);
            return;
        }

        squadPanel.SetUsingItem(item, this);

    }

    internal void Init(ShoppingPanel _shoppingPanel, SquadPanel _squadPanel ,SquadItemPanel _itemPanel, int _buttonIndex)
    {
        isShoppingButton = false; 

        this.shoppingPanel = _shoppingPanel;
        this.squadPanel = _squadPanel;
        this.itemPenel = _itemPanel;

        this.buttonIndex = _buttonIndex;
        this.outline = gameObject.GetComponent<Outline>();

        SetItemToButton(null, 0);
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

        itemPenel.ApplyChangeInItemList(this.buttonIndex, -1);

        if(remainNum <= 0)
            gameObject.GetComponent<Button>().interactable = false;
    }

    public void SetOutline(bool setting)
    {
        outline.enabled = setting;
    }
}