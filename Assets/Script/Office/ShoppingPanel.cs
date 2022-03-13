using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.Office
{
    public class ShoppingPanel : MonoBehaviour
    {
        const string MONEY_UNIT = "G";

        public GameObject shopPanelGroup;
        private List<Image> shopItemPanelList;

        public GameObject playerPanelGroup;
        private List<Image> playerItemPanelList;

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
        private PlayerData playerData;
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
            playerData = saveDataManager.GetPlayerData();
            itemSaveData = saveDataManager.GetItemSaveData();

            playerBalance = saveDataManager.GetBalance();
            balanceText.text = playerBalance.ToString() + MONEY_UNIT;
            shopModeChangerButton.GetComponentInChildren<Text>().text = "Sale";

            InitItemPanels();

            playerPanelGroup.SetActive(false);
            selectPanel.active = false;
        }

        private void InitItemPanels()
        {
            InitShopItemPanel();

            InitPlayerItemPanel();
        }

        private void InitShopItemPanel()
        {
            shopItemPanelList = new List<Image>();
            Image[] shopPanels = shopPanelGroup.GetComponentsInChildren<Image>();
            List<Dictionary<string, Item>> itemDictionaryList = new List<Dictionary<string, Item>>();

            itemDictionaryList.Add(itemSaveData.GetSupplyItemDictionary());
            itemDictionaryList.Add(itemSaveData.GetUsableItemDictionary());
            itemDictionaryList.Add(itemSaveData.GetRestItemDictionary());

            for (int i = 0; i < shopPanels.Length; ++i)
            {
                shopItemPanelList.Add(shopPanels[i]);
                InitShopItemPanels(shopPanels[i], itemDictionaryList[i]);
            }
        }
        private void InitPlayerItemPanel()
        {
            playerItemPanelList = new List<Image>();
            Image[] playerPanels = playerPanelGroup.GetComponentsInChildren<Image>();

            List<Dictionary<Item, int>> itemDictionaryList = new List<Dictionary<Item, int>>();

            itemDictionaryList.Add(playerData.GetSupplyItemDictionary());
            itemDictionaryList.Add(playerData.GetUsableItemDictionary());
            itemDictionaryList.Add(playerData.GetRestItemDictionary());

            for (int i = 0; i < playerPanels.Length; ++i)
            {
                shopItemPanelList.Add(playerPanels[i]);
                InitPlayerItemPanels(playerPanels[i], itemDictionaryList[i]);
            }
        }

        private void InitShopItemPanels(Image panel, Dictionary<string, Item> itemDictionary)
        {
            var itemNames = itemDictionary.Keys;

            foreach (var itemName in itemNames)
            {
                Instantiate(itemButtonPrefab, panel.transform).GetComponent<ShopItemButton>().SetItemToButton(this, itemDictionary[itemName]);
            }
        }

        private void InitPlayerItemPanels(Image panel, Dictionary<Item, int> itemDictionary)
        {
            var items = itemDictionary.Keys;

            foreach (var item in items)
            {
                Instantiate(itemButtonPrefab, panel.transform).GetComponent<ShopItemButton>().SetItemToButton(this, item, itemDictionary[item]);
            }
        }

        public void OnClickDealButton()
        {
            DealExecute();
        }

        private void DealExecute()
        {
            int dealCost = selectedItem.GetPrice();
            selectedButton.DealExecute();

            if (isPurchaseMode)
                PurchaseExecute(dealCost);
            else
                SellExecute(dealCost);

            balanceText.text = playerBalance.ToString() + MONEY_UNIT;   //update apply
        }

        private void PurchaseExecute(int dealCost)
        {
            playerBalance -= dealCost;
            dealButton.interactable = CheckPurchasePossible();
        }

        private void SellExecute(int dealCost)
        {
            playerBalance += dealCost;
            dealButton.interactable = CheckSellPossible();
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
            playerPanelGroup.SetActive(false);
            shopPanelGroup.SetActive(true);

            shopModeChangerButton.GetComponentInChildren<Text>().text = "Sale";
            dealButton.GetComponentInChildren<Text>().text = "구매";
        }

        private void ChangeToSellMode()
        {
            shopPanelGroup.SetActive(false);
            playerPanelGroup.SetActive(true);

            shopModeChangerButton.GetComponentInChildren<Text>().text = "Shop";
            dealButton.GetComponentInChildren<Text>().text = "판매";
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
            else
                dealButton.interactable = CheckSellPossible();
        }

        private bool CheckPurchasePossible()
        {
            if (playerBalance < selectedItem.GetPrice())
                return false;
            return true;
        }

        private bool CheckSellPossible()
        {
            if (selectedButton.GetRemainNumber() <= 0)
                return false;
            return true;
        }

        internal float GetSaleProbability() { return this.saleProbability; }
    }
}