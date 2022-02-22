﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CafeItemButton : MonoBehaviour, ButtonInterface
{
    const string MONEY_UNIT = "G";
    private ShoppingPanel cafePanel;
    private Item item;
    private int itemPrice;
    private int remainNum;
    private float saleProbability;

    private bool isShoppingButton = true;

    private Outline outline;
    public Text nameText;
    public Text remainText;
    public Text prictText;

    void Awake()
    {
        outline = gameObject.GetComponent<Outline>();
        outline.enabled = false;
    }

    internal void SetItemToButton(ShoppingPanel _cafePanel, Item _item) 
    {
        this.cafePanel = _cafePanel;
        this.item = _item;
        this.saleProbability = cafePanel.GetSaleProbability();


        remainNum = item.GetSellingNumber();
        remainText.text = remainNum.ToString();

        nameText.text = item.GetName().ToString();
        SetItemPrice();
    }

    private void SetItemPrice()
    {
        int salePrice = item.GetPrice();
        string addedInfo = "50% 할인 특가 ";

        if (Random.Range(0,100) <= saleProbability)//반값 할인
        {
            salePrice /= 2;
            prictText.text = addedInfo + salePrice.ToString() + MONEY_UNIT;
        }
        else
        {
            prictText.text = salePrice.ToString() + MONEY_UNIT;
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
