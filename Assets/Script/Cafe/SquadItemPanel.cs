using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SquadItemPanel : MonoBehaviour
{
    private Button[] itemButtons;
    private SaveDataManager saveData;
    private SquadData squadData;
    private CafePanel cafePanel;

    internal void Init(SaveDataManager _saveData, CafePanel _cafePanel)
    {
        this.saveData = _saveData;
        squadData = saveData.GetSquadData();

        this.cafePanel = _cafePanel;
        itemButtons = gameObject.GetComponentsInChildren<Button>();

        LoadAllSquadItem();
    }

    private void LoadAllSquadItem()
    {
        Dictionary<Item, int> itemDictionary = squadData.GetItemDictionary();
        var items = itemDictionary.Keys;

        int buttonIdx = 0;
        foreach (Item item in items)
        {
            LoadEachItemToButton(itemButtons[buttonIdx], item, itemDictionary[item]);
        }
    }
    private void LoadEachItemToButton(Button button, Item item , int remainNum)
    {
        const string iconPath = "ItemIcon/";
        button.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + item.GetIconName());
        button.GetComponent<SquadItemButton>().SetItemToButton(cafePanel,item, remainNum);
    }
    
}