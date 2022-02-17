using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CafePanel : MonoBehaviour
{
    private CafeManager cafeManager;
    private SaveDataManager saveDataManager;
    private Button[] itemButtons;
    private void Awake()
    {
        //gameObject.active = false;
    }

    internal void Init(SaveDataManager _saveDataManager)
    {
        this.saveDataManager = _saveDataManager;
        itemButtons = gameObject.GetComponentsInChildren<Button>();
        cafeManager = GameObject.Find("CafeManager").GetComponent<CafeManager>();
        LoadPanelItemRandomly();
    }

    private void LoadPanelItemRandomly()
    {
        ItemSaveData itemSaveData = saveDataManager.GetItemSaveData();
        Dictionary<string, Item> allItemDictionary = itemSaveData.GetAllItemDictionary();
        List<string> allitemNames = itemSaveData.GetAllItemName();

        List<int> selectedItemIdx = new List<int>();
        for (int i=0; i<itemButtons.Length ; ++i)
        {
            selectedItemIdx.Add(GetUnreapedIdxInList(selectedItemIdx));
            Debug.Log(selectedItemIdx);
        }

        for (int i = 0; i < itemButtons.Length; ++i)
        {
            LoadItemToButton(i,allItemDictionary[allitemNames[selectedItemIdx[i]]]);
        }
    }

    private void LoadItemToButton(int buttomIdx , Item item)
    {
        const string iconPath = "ItemIcon/";
        itemButtons[buttomIdx].GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + item.GetIconName());
        itemButtons[buttomIdx].GetComponent<CafeItemButton>().SetItemToButton(item);
    }

    private int GetUnreapedIdxInList(List<int> selectedItemIdx) //리스트 내의 중복된 index는 제외함
    {
        int nextIdx;
        do
        {
            nextIdx = Random.Range(0, itemButtons.Length);
        } while (selectedItemIdx.Contains(nextIdx));

        return nextIdx;
    }
}