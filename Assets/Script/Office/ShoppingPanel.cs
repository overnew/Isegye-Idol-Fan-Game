using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.Office
{
    public class ShoppingPanel : MonoBehaviour
    {
        public GameObject itemPanel;
        public GameObject selectPanel;
        public GameObject itemButtonPrefab;

        private OfficeManager officeManager;
        private SaveDataManager saveDataManager;
        private SquadData squadData;
        private ItemSaveData itemSaveData;

        private void Start()
        {
            officeManager = GameObject.Find("OfficeManager").GetComponent<OfficeManager>();
            saveDataManager = officeManager.GetSaveDataManager();
            squadData = saveDataManager.GetSquadData();
            itemSaveData = saveDataManager.GetItemSaveData();

            InstantItemButtons();
        }

        private void InstantItemButtons()
        {
            Dictionary<string, Item> itemDictionary = itemSaveData.GetAllItemDictionary();
            List<string> allItemNames = itemSaveData.GetAllItemName();

            var itemNames = itemDictionary.Keys;
            foreach (var itemName in itemNames)
            {
                Instantiate(itemButtonPrefab, itemPanel.transform);

                itemButtonPrefab.GetComponent<ShopItemButton>().SetItemToButton(itemDictionary[itemName]);
                
            }

        }
    }
}