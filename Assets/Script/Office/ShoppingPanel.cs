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
        private List<GameObject> shopItemPanelList;

        public GameObject playerPanelGroup;
        private List<GameObject> playerItemPanelList;

        public GameObject selectPanel;

        public GameObject itemButtonPrefab;
        public Image selectedImage;
        public Text selectedInfo;
        public Text selectedPrice;
        public Text balanceText;

        public Button dealButton;
        public Button shopModeChangerButton;
        public GameObject itemTypeButtonGroup;
        private Outline[] itemTypeBottonOutlines;

        private OfficeManager officeManager;
        private SaveDataManager saveDataManager;
        private PlayerData playerData;
        private ItemSaveData itemSaveData;

        private float playerBalance;
        private Item selectedItem;
        private ShopItemButton selectedButton = null;
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

            itemTypeBottonOutlines = itemTypeButtonGroup.GetComponentsInChildren<Outline>();

            playerPanelGroup.SetActive(false);
            OnClickChangeToSupplyPanel();
        }

        private void InitItemPanels()
        {
            InitShopItemPanel();

            InitPlayerItemPanel();
        }

        private void InitShopItemPanel()
        {
            shopItemPanelList = new List<GameObject>();
            EmptyMonoBehaviour[] monoBehaviours = shopPanelGroup.GetComponentsInChildren<EmptyMonoBehaviour>();

            for (int i=0; i<monoBehaviours.Length ; ++i)
            {
                shopItemPanelList.Add(monoBehaviours[i].gameObject);
            }

            List<Dictionary<string, Item>> itemDictionaryList = new List<Dictionary<string, Item>>();

            itemDictionaryList.Add(itemSaveData.GetSupplyItemDictionary());
            itemDictionaryList.Add(itemSaveData.GetUsableItemDictionary());
            itemDictionaryList.Add(itemSaveData.GetRestItemDictionary());

            for (int i = 0; i < shopItemPanelList.Count; ++i)
            {
                InstantShopItemToPanel(shopItemPanelList[i], itemDictionaryList[i]);
            }
        }

        private void InitPlayerItemPanel()
        {
            playerItemPanelList = new List<GameObject>();
            EmptyMonoBehaviour[] monoBehaviours = playerPanelGroup.GetComponentsInChildren<EmptyMonoBehaviour>();

            for (int i = 0; i < monoBehaviours.Length; ++i)
            {
                playerItemPanelList.Add(monoBehaviours[i].gameObject);
            }

            List<Dictionary<Item, int>> itemDictionaryList = new List<Dictionary<Item, int>>();

            itemDictionaryList.Add(playerData.GetSupplyItemDictionary());
            itemDictionaryList.Add(playerData.GetUsableItemDictionary());
            itemDictionaryList.Add(playerData.GetRestItemDictionary());

            for (int i = 0; i < playerItemPanelList.Count; ++i)
            {
                InstantPlayerItemToPanel(playerItemPanelList[i], itemDictionaryList[i]);
            }
        }

        private void InstantShopItemToPanel(GameObject panel, Dictionary<string, Item> itemDictionary)
        {
            var itemNames = itemDictionary.Keys;

            foreach (var itemName in itemNames)
            {
                Instantiate(itemButtonPrefab, panel.transform).GetComponent<ShopItemButton>().SetItemToButton(this, itemDictionary[itemName]);
            }
        }

        private void InstantPlayerItemToPanel(GameObject panel, Dictionary<Item, int> itemDictionary)
        {
            if (itemDictionary.Count <= 0)
                return; 

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
            SetSelectedThingsEnable(false);
            isPurchaseMode = !isPurchaseMode;

            if (isPurchaseMode)
                ChangeToPurchaseMode();
            else
                ChangeToSellMode();

            OnClickChangeToSupplyPanel();
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

            selectedItem = item;
            selectedButton = itemButton;
            SetSelectedThingsEnable(true);

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

        public void OnClickChangeToSupplyPanel()
        {
            SetPanelEnableByIndex(ItemTypeToIndex.supplyItem);
        }

        public void OnClickChangeToUsablePanel()
        {
            SetPanelEnableByIndex(ItemTypeToIndex.usableItem);
        }

        public void OnClickChangeToRestPanel()
        {
            SetPanelEnableByIndex(ItemTypeToIndex.restItem);
        }

        private void SetPanelEnableByIndex(ItemTypeToIndex idx)
        {
            SetSelectedThingsEnable(false);

            if (isPurchaseMode)
            {
                SetAllPanelEnableInList(shopItemPanelList);
                shopItemPanelList[(int)idx].SetActive(true);
                itemTypeBottonOutlines[(int)idx].enabled = true;
                return;
            }

            SetAllPanelEnableInList(playerItemPanelList);
            playerItemPanelList[(int)idx].SetActive(true);
            itemTypeBottonOutlines[(int)idx].enabled = true;
        }

        private void SetAllPanelEnableInList(List<GameObject> panels)
        {

            for (int i=0; i< itemTypeBottonOutlines.Length; ++i )
                itemTypeBottonOutlines[i].enabled = false;

            for (int i = 0; i < panels.Count; ++i)
                panels[i].SetActive(false);
        }

        private void SetSelectedThingsEnable(bool setting)
        {
            if (selectedButton != null)
                selectedButton.SetOutline(setting);

            selectPanel.active = setting;  //아이템 선택시 등장하도록 일단 false

        }

        internal float GetSaleProbability() { return this.saleProbability; }
    }
}
