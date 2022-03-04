using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SquadItemPanel : MonoBehaviour
{
    public GameObject sortButton;

    private Button[] itemButtons;
    private SaveDataManager saveData;
    private SquadData squadData;
    private ShoppingPanel shoppingPanel;
    private SquadPanel squadPanel;

    private List<KeyValuePair<Item, int>> itemList;

    internal void Init(SaveDataManager _saveData, ShoppingPanel _shoppingPanel, SquadPanel _squadPanel)
    {
        itemList = new List<KeyValuePair<Item, int>>();
        this.shoppingPanel = _shoppingPanel;
        this.squadPanel = _squadPanel;

        this.saveData = _saveData;
        squadData = saveData.GetSquadData();

        itemButtons = gameObject.GetComponentsInChildren<Button>();

        InitAllButton();
        LoadAllSquadItem();
        BlockUnusableItem();
    }

    private void InitAllButton()
    {
        for (int i=0; i<itemButtons.Length ; ++i)
        {
            itemButtons[i].GetComponent<SquadItemButton>().Init(shoppingPanel, squadPanel, this, i);
        }
    }

    private void LoadAllSquadItem()
    {
        Dictionary<Item, int> itemDictionary = squadData.GetItemDictionary();
        var items = itemDictionary.Keys;

        foreach (Item item in items)
        {
            itemList.Add(new KeyValuePair<Item, int>(item, itemDictionary[item]));
        }

        LoadItemListToPanel();
    }
    private void LoadEachItemToButton(Button button, Item item , int remainNum)
    {
        const string iconPath = "ItemIcon/";
        button.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + item.GetIconName());
        button.GetComponent<SquadItemButton>().SetItemToButton(item, remainNum);
    }

    internal bool GetHaveEmptySpace(Item item)      //아이템이 들어갈 장소가 있는지 확인
    {
        if (FindRemainItemSpace(item) == -1 &&itemList.Count >= itemButtons.Length)
            return false;
        return true;
    }

    internal void SetItemToEmptySpace(Item item)
    {
        int insertIdx = FindRemainItemSpace(item);

        if (insertIdx == -1) // 같은 아이템창에 추가가 안되는 경우 끝에 추가
        {
            itemList.Add(new KeyValuePair<Item, int>(item, 1));
            itemButtons[itemList.Count -1].interactable = true;
            LoadEachItemToButton(itemButtons[itemList.Count - 1], item, 1);
            
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

        } while (itemList[index].Value >= itemList[index].Key.GetMaxPossessionNumber());
        
        return index;
    }

    internal void ApplyChangeInItemList(int idx, int changeNum) 
    {
        Item item = itemList[idx].Key;
        int remainNum = itemList[idx].Value + changeNum;
        itemList[idx] = new KeyValuePair<Item, int>(item, remainNum);

        if(remainNum <=0)   //빈 칸이 생기면 리스트 정렬
            FillEmptySpaceOfPanel(idx);
    }

    private void FillEmptySpaceOfPanel(int emptyIdx)
    {
        int idx = emptyIdx;
        for (;  idx< itemList.Count-1 ; ++idx)
        {
            LoadEachItemToButton(itemButtons[idx], itemList[idx+1].Key, itemList[idx+1].Value);
            itemList[idx] = itemList[idx + 1];
        }

        RemoveItemInButton(idx);
        itemList.RemoveAt(idx); //마지막 요소 삭제
    }

    private void RemoveItemInButton(int blockidx)
    {
        itemButtons[blockidx].GetComponent<Image>().sprite = null;
        itemButtons[blockidx].GetComponent<SquadItemButton>().SetItemToButton(null, 0);

        itemButtons[blockidx].interactable = false;
    }

    public void SortItemPanel()
    {
        List<KeyValuePair<Item, int>> totalItemList = new List<KeyValuePair<Item, int>>();
        for (int idx=0; idx<itemList.Count ;++idx )
        {
            int itemIdx = totalItemList.FindIndex(pair => pair.Key.Equal(itemList[idx].Key));
            
            if(itemIdx == -1)   //totalList에 없는 아이템의 경우
            {
                totalItemList.Add(itemList[idx]);
                continue;
            }

            //아이템이 있는 경우
            Item item = itemList[idx].Key;
            int sumNum = itemList[idx].Value + totalItemList[itemIdx].Value;
            totalItemList[itemIdx] = new KeyValuePair<Item, int>(item, sumNum);
        }

        for(int idx = 0; idx < totalItemList.Count; ++idx )
        {
            int maxNumber = totalItemList[idx].Key.GetMaxPossessionNumber();
            if (totalItemList[idx].Value > maxNumber) //소지량 초과시
            {
                if (idx + 1 >= totalItemList.Count)
                    totalItemList.Add(new KeyValuePair<Item, int>(totalItemList[idx].Key, totalItemList[idx].Value- maxNumber));
                else
                    totalItemList.Insert(idx+1, new KeyValuePair<Item, int>(totalItemList[idx].Key, totalItemList[idx].Value - maxNumber));

                totalItemList[idx] = new KeyValuePair<Item, int>(totalItemList[idx].Key, maxNumber);
            }
        }

        itemList = totalItemList;
        LoadItemListToPanel();
    }
    private void LoadItemListToPanel()
    {
        int buttonIdx = 0;
        foreach (var pair in itemList)
        {
            LoadEachItemToButton(itemButtons[buttonIdx++], pair.Key, pair.Value);
        }

        while (buttonIdx < itemButtons.Length)
        {
            RemoveItemInButton(buttonIdx++);
        }
    }

    internal void BlockUnusableItem()
    {
        for (int idx = 0; idx<itemList.Count ; ++idx )
        {
            Item item = itemList[idx].Key;  //체력 회복 아이템만 사용가능
            if (item.GetIsTargetedEnemy() || !item.GetIsBuff()|| !item.GetBuffEffectedStatus().Equals("hp"))
                itemButtons[idx].interactable = false;
        }
    }

    internal void UnblockAllButton()
    {
        for (int idx = 0; idx < itemList.Count; ++idx)
        {
            itemButtons[idx].interactable = true;
        }
    }
}