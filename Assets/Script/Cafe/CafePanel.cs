using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CafePanel : MonoBehaviour
{
    private CafeManager cafeManager;
    private SaveDataManager saveDataManager;
    private Button[] itemButtons;

    private CafeItemButton selectedButton;
    private Item selectedItem;
    private float saleProbability = 80f;

    public Image itemImage;
    public Text itemInfo;
    public Button parchaseButton;

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
        for (int i=0; i<itemButtons.Length-1 ; ++i)
        {
            selectedItemIdx.Add(GetUnreapedIdxInList(selectedItemIdx));
        }

        for (int idx = 0; idx < itemButtons.Length-1; ++idx)
        {
            LoadItemToButton(idx,allItemDictionary[allitemNames[selectedItemIdx[idx]]]);
        }
    }

    private void LoadItemToButton(int buttomIdx , Item item)
    {
        itemButtons[buttomIdx].GetComponent<CafeItemButton>().SetItemToButton(this, item);
    }

    private int GetUnreapedIdxInList(List<int> selectedItemIdx) //리스트 내의 중복된 index는 제외함
    {
        int nextIdx;
        do
        {
            nextIdx = Random.Range(0, itemButtons.Length-1);
        } while (selectedItemIdx.Contains(nextIdx));

        return nextIdx;
    }

    public void ParchaseButtonClick()
    {
        selectedButton.ParchaseExcute();

        if(selectedButton.GetRemainNumber() <= 0)
            parchaseButton.enabled = false;
    }

    internal void SetSelectdItem(Item item, CafeItemButton cafeItemButton) 
    { 
        this.selectedItem = item;
        this.selectedButton = cafeItemButton;
        SetSelectedPanel();

        if (selectedButton.GetRemainNumber() <=0)
            parchaseButton.enabled = false;
    }
    private void SetSelectedPanel()
    {
        const string iconPath = "ItemIcon/";
        itemImage.sprite = Resources.Load<Sprite>(iconPath + selectedItem.GetIconName());
        itemInfo.text = selectedItem.GetAbilityDesc();
    }


    internal float GetSaleProbability() { return this.saleProbability; }
}