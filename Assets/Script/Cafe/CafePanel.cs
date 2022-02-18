using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CafePanel : MonoBehaviour
{
    const string PRICE = "가격 : ";
    const string MONEY_UNIT = "G";
    const string BALANCE_MARK = "잔돈: ";

    private CafeManager cafeManager;
    private SaveDataManager saveDataManager;
    private Button[] itemButtons;

    private CafeItemButton selectedButton;
    private Item selectedItem;
    private float saleProbability = 80f;
    private int playerBalance;

    public GameObject itemPanel;
    public Image itemImage;
    public Text itemInfo;
    public Button parchaseButton;
    public Text itemPrice;
    public Text balance;

    private void Awake()
    {
        //gameObject.active = false;
    }

    internal void Init(SaveDataManager _saveDataManager)
    {
        this.saveDataManager = _saveDataManager;
        itemButtons = itemPanel.GetComponentsInChildren<Button>();
        cafeManager = GameObject.Find("CafeManager").GetComponent<CafeManager>();
        playerBalance = saveDataManager.GetBalance();
        balance.text = BALANCE_MARK + playerBalance.ToString() + MONEY_UNIT;

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
        }

        for (int idx = 0; idx < itemButtons.Length; ++idx)
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
            nextIdx = Random.Range(0, itemButtons.Length);
        } while (selectedItemIdx.Contains(nextIdx));

        return nextIdx;
    }

    public void ParchaseButtonClick()
    {
        selectedButton.ParchaseExcute();
        playerBalance -= selectedButton.GetPrice(); //할인가 적용
        balance.text = BALANCE_MARK + playerBalance.ToString() + MONEY_UNIT;

        parchaseButton.interactable = CheckPossibleToDeal();
    }

    internal void SetSelectdItem(Item item, CafeItemButton cafeItemButton) 
    { 
        this.selectedItem = item;
        this.selectedButton = cafeItemButton;
        SetSelectedPanel();

        parchaseButton.interactable = CheckPossibleToDeal();
    }
    private bool CheckPossibleToDeal()
    {
        if (selectedButton.GetRemainNumber() <= 0 || playerBalance < selectedButton.GetPrice())
            return false;
        return true;
    }

    private void SetSelectedPanel()
    {
        const string iconPath = "ItemIcon/";
        itemImage.sprite = Resources.Load<Sprite>(iconPath + selectedItem.GetIconName());
        itemInfo.text = selectedItem.GetAbilityDesc();

        itemPrice.text = PRICE+ selectedButton.GetPrice().ToString() + MONEY_UNIT;
    }

    internal float GetSaleProbability() { return this.saleProbability; }
}