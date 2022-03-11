using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.Office
{
    public class ShoppingPanel : MonoBehaviour
    {
        const string MONEY_UNIT = "G";

        public GameObject itemPanel;
        public GameObject selectPanel;
        public GameObject itemButtonPrefab;
        public Image selectedImage;
        public Text selectedInfo;
        public Text selectedPrice;
        public Text balanceText;

        private OfficeManager officeManager;
        private SaveDataManager saveDataManager;
        private SquadData squadData;
        private ItemSaveData itemSaveData;

        private ShopItemButton selectedButton;
        private float saleProbability = 0;

        private void Start()
        {
            officeManager = GameObject.Find("OfficeManager").GetComponent<OfficeManager>();
            saveDataManager = officeManager.GetSaveDataManager();
            squadData = saveDataManager.GetSquadData();
            itemSaveData = saveDataManager.GetItemSaveData();

            balanceText.text = saveDataManager.GetBalance().ToString() + MONEY_UNIT;

            InstantItemButtons();
        }

        private void InstantItemButtons()
        {
            Dictionary<string, Item> itemDictionary = itemSaveData.GetAllItemDictionary();
            var itemNames = itemDictionary.Keys;

            foreach (var itemName in itemNames)
            {

                Instantiate(itemButtonPrefab, itemPanel.transform).GetComponent<ShopItemButton>().SetItemToButton(this, itemDictionary[itemName]);
            }

        }

        internal void SetSelectdItem(Item item, ShopItemButton itemButton)
        {
            selectedButton = itemButton;

            selectedImage.sprite = Utils.GetItemIconByIconName(item.GetIconName());
            selectedInfo.text = item.GetAbilityDesc();
            selectedPrice.text = item.GetPrice().ToString() + MONEY_UNIT;
        }

        internal float GetSaleProbability() { return this.saleProbability; }
    }
}