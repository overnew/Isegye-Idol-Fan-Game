﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.Office
{
    public class ShoppingPanel : MonoBehaviour
    {
        const string MONEY_UNIT = "G";

        public GameObject shopItemPanel;
        public GameObject playerItemPanel;
        public GameObject selectPanel;

        public GameObject itemButtonPrefab;
        public Image selectedImage;
        public Text selectedInfo;
        public Text selectedPrice;
        public Text balanceText;

        public Button dealButton;
        public Button shopModeChangerButton;

        private OfficeManager officeManager;
        private SaveDataManager saveDataManager;
        private SquadData squadData;
        private ItemSaveData itemSaveData;

        private float playerBalance;
        private Item selectedItem;
        private ShopItemButton selectedButton;
        private float saleProbability = 0;
        private bool isPurchaseMode = true;

        private void Start()
        {
            officeManager = GameObject.Find("OfficeManager").GetComponent<OfficeManager>();
            saveDataManager = officeManager.GetSaveDataManager();
            squadData = saveDataManager.GetSquadData();
            itemSaveData = saveDataManager.GetItemSaveData();

            playerBalance = saveDataManager.GetBalance();
            balanceText.text = playerBalance.ToString() + MONEY_UNIT;
            shopModeChangerButton.GetComponentInChildren<Text>().text = "Sale";

            InstantItemButtons();

            playerItemPanel.SetActive(false);
            selectPanel.active = false;
        }

        private void InstantItemButtons()
        {
            Dictionary<string, Item> shopItemDictionary = itemSaveData.GetAllItemDictionary();
            var itemNames = shopItemDictionary.Keys;

            foreach (var itemName in itemNames)
            {
                Instantiate(itemButtonPrefab, shopItemPanel.transform).GetComponent<ShopItemButton>().SetItemToButton(this, shopItemDictionary[itemName]);
            }

            Dictionary<Item, int> playerItemDictionary = squadData.GetItemDictionary();
            var playItems = playerItemDictionary.Keys;

            foreach (var item in playItems)
            {
                Instantiate(itemButtonPrefab, playerItemPanel.transform).GetComponent<ShopItemButton>().SetItemToButton(this,item, playerItemDictionary[item]);
            }
        }

        public void OnClickDealButton()
        {
            DealExecute();
        }

        private void DealExecute()
        {
            int dealCost = selectedItem.GetPrice();
            playerBalance -= dealCost;
            balanceText.text = playerBalance.ToString() + MONEY_UNIT;

            selectedButton.DealExecute();

            if (isPurchaseMode)
                dealButton.interactable = CheckPurchasePossible();
        }

        public void OnClickModeChanger()
        {
            selectPanel.active = false; // select 창은 일단 비워둠
            isPurchaseMode = !isPurchaseMode;

            if (isPurchaseMode)
                ChangeToPurchaseMode();
            else
                ChangeToSellMode();
        }

        private void ChangeToPurchaseMode()
        {
            playerItemPanel.SetActive(false);
            shopItemPanel.SetActive(true);
            shopModeChangerButton.GetComponentInChildren<Text>().text = "Sale";

        }

        private void ChangeToSellMode()
        {
            shopItemPanel.SetActive(false);
            playerItemPanel.SetActive(true);
            shopModeChangerButton.GetComponentInChildren<Text>().text = "Shop";
        }
        

        internal void SetSelectdItem(Item item, ShopItemButton itemButton)
        {
            selectPanel.active = true;  //아이템 선택시 등장하도록

            selectedItem = item;
            selectedButton = itemButton;

            selectedImage.sprite = Utils.GetItemIconByIconName(item.GetIconName());
            selectedInfo.text = item.GetAbilityDesc();
            selectedPrice.text = item.GetPrice().ToString() + MONEY_UNIT;

            if (isPurchaseMode)
                dealButton.interactable = CheckPurchasePossible();
        }

        private bool CheckPurchasePossible()
        {
            if (playerBalance < selectedItem.GetPrice())
                return false;
            return true;
        }

        internal float GetSaleProbability() { return this.saleProbability; }
    }
}