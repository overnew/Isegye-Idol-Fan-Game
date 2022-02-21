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

    private List<KeyValuePair<Item, int>> itemList;
    private int emptyStartIndex;

    internal void Init(SaveDataManager _saveData, CafePanel _cafePanel)
    {
        itemList = new List<KeyValuePair<Item, int>>();

        this.saveData = _saveData;
        squadData = saveData.GetSquadData();

        this.cafePanel = _cafePanel;
        itemButtons = gameObject.GetComponentsInChildren<Button>();

        InitAllButton();
        LoadAllSquadItem();
    }

    private void InitAllButton()
    {
        for (int i=0; i<itemButtons.Length ; ++i)
        {
            itemButtons[i].GetComponent<SquadItemButton>().Init(cafePanel, this, i);
        }
    }

    private void LoadAllSquadItem()
    {
        Dictionary<Item, int> itemDictionary = squadData.GetItemDictionary();
        var items = itemDictionary.Keys;

        int buttonIdx = 0;
        foreach (Item item in items)
        {
            itemList.Add(new KeyValuePair<Item, int>(item, itemDictionary[item]));
            LoadEachItemToButton(itemButtons[buttonIdx++], item, itemDictionary[item]);
        }

        emptyStartIndex = buttonIdx;    //빈 공간의 시작부를 저장
        while (buttonIdx<itemButtons.Length)
        {
            itemButtons[buttonIdx++].interactable = false;
        }
    }
    private void LoadEachItemToButton(Button button, Item item , int remainNum)
    {
        const string iconPath = "ItemIcon/";
        button.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + item.GetIconName());
        button.GetComponent<SquadItemButton>().SetItemToButton(item, remainNum);
    }

    internal bool GetHaveEmptySpace()
    {
        if (emptyStartIndex >= itemButtons.Length)
            return false;
        return true;
    }

    internal void SetItemToEmptySpace(Item item)
    {
        int insertIdx = FindRemainItemSpace(item);

        if (insertIdx == -1) // 같은 아이템창에 추가가 안되는 경우 끝에 추가
        {
            itemButtons[emptyStartIndex].interactable = true;
            LoadEachItemToButton(itemButtons[emptyStartIndex++], item, 1);
            return;
        }

        LoadEachItemToButton(itemButtons[insertIdx],item, 
            itemButtons[insertIdx].GetComponent<ButtonInterface>().GetRemainNumber() +1 );
        ApplyChangeInItemList(insertIdx, 1);
    }
    
    private int FindRemainItemSpace(Item item)
    {
        int index=-1;

        do
        {
            index = itemList.FindIndex(index + 1, (pair) => pair.Key.Equal(item));

            if (index == -1 || index >= itemList.Count ||index >= itemButtons.Length)
                break;


        } while (itemList[index].Value >= itemList[index].Key.GetMaxPossessiongNumber());
        
        return index;
    }

    internal void ApplyChangeInItemList(int idx, int changeNum) 
    {
        Item item = itemList[idx].Key;
        int remainNum = itemList[idx].Value + changeNum;
        itemList[idx] = new KeyValuePair<Item, int>(item, remainNum);
    }
}