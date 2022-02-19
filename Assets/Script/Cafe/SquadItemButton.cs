using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SquadItemButton : MonoBehaviour, ButtonInterface
{
    public Text remainText;

    private CafePanel cafePanel;
    private Item item;
    private Outline outline;

    private bool isShoppingButton = false;
    private int remainNum;

    public void OnClickItem()
    {
        outline.enabled = true;
        cafePanel.SetSelectdItem(item, this);
    }

    internal void SetItemToButton(CafePanel _cafePanel ,Item _item,int _remainNum)
    {
        this.cafePanel = _cafePanel;
        this.item = _item;
        this.remainNum = _remainNum;
        this.outline = gameObject.GetComponent<Outline>();

        outline.enabled = false;
        remainText.text = this.remainNum.ToString();
    }

    public bool GetIsShoppingButton() { return this.isShoppingButton; }
    public int GetPrice() { return item.GetPrice() / 2; }
    public int GetRemainNumber() { return this.remainNum; }

    public void DealExecute()
    {
        --remainNum;
        remainText.text = remainNum.ToString();

        if(remainNum <= 0)
            gameObject.GetComponent<Button>().interactable = false;
    }

    public void SetOutline(bool setting)
    {
        outline.enabled = setting;
    }
}