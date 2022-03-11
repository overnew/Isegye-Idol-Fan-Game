using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemButton : MonoBehaviour, ButtonInterface
{
    private ShoppingPanel cafePanel;
    private Item item;
    private int itemPrice;
    private int remainNum;
    private float saleProbability =0;

    private bool isShoppingButton = true;

    private Outline outline;
    public Image itemImage;
    public Image saleMark;
    public Text remainText;

    void Awake()
    {
        outline = gameObject.GetComponent<Outline>();
        outline.enabled = false;
        saleMark.enabled = false;
    }

    internal void SetItemToButton(ShoppingPanel _cafePanel, Item _item) 
    {
        this.cafePanel = _cafePanel;
        this.item = _item;
        this.saleProbability = cafePanel.GetSaleProbability();

        itemImage.sprite = Utils.GetItemIconByIconName(item.GetIconName());

        remainNum = item.GetSellingNumber();
        remainText.text = remainNum.ToString();

        SetItemPrice();
    }
    internal void SetItemToButton(Item _item)
    {
        this.item = _item;
        //this.saleProbability = cafePanel.GetSaleProbability();

        itemImage.sprite = Utils.GetItemIconByIconName(item.GetIconName());

        remainNum = item.GetSellingNumber();
        remainText.text = remainNum.ToString();

        SetItemPrice();
    }

    private void SetItemPrice()
    {
        int salePrice = item.GetPrice();

        if (Random.Range(0,100) <= saleProbability)//반값 할인
        {
            salePrice /= 2;
            saleMark.enabled = true;
        }
            
        itemPrice = salePrice;
    }

    public void OnClickButton()
    {
        cafePanel.SetSelectdItem(this.item, this);
        outline.enabled = true;
    }

    public void DealExecute()
    {
        --remainNum;
        remainText.text = remainNum.ToString();

        if (remainNum <= 0)
            gameObject.GetComponent<Button>().interactable = false;
    }

    public int GetRemainNumber() { return this.remainNum; }
    public int GetPrice() { return this.itemPrice; }

    public bool GetIsShoppingButton() { return this.isShoppingButton; }

    public void SetOutline(bool setting) { outline.enabled = setting; }
}
