using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShoppingPanel : MonoBehaviour
{
    const string PRICE = "가격 : ";
    const string MONEY_UNIT = "G";
    const string BALANCE_MARK = "잔돈: ";

    private CafeManager cafeManager;
    private SquadItemPanel squadItemPanel;
    private SaveDataManager saveDataManager;
    private Button[] itemButtons;

    private ButtonInterface selectedButton = null;
    private Item selectedItem;
    private float saleProbability = 80f;
    private int playerBalance;

    public GameObject itemPanel;
    public Image itemImage;
    public Text itemInfo;
    public Button dealButton;
    public Text itemPrice;
    public Text balance;
    public GameObject message;

    private bool isShoppingMode = false;

    private void Awake()
    {
        gameObject.active = false;
        message.active = false;
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

    public void OnClickDealButton()
    {
        DealExecute();
    }

    private void DealExecute()
    {
        if (selectedButton.GetIsShoppingButton() &&!squadItemPanel.GetHaveEmptySpace(selectedItem))
        {
            dealButton.interactable = false;
            message.active = true;
            StartCoroutine(MessageFadeAway());
            return;
        }

        int dealCost = selectedButton.GetPrice();
        selectedButton.DealExecute();

        if (selectedButton.GetIsShoppingButton()) //구매시 잔액 차감
        {
            dealCost *= -1;
            ParchaseExecute();
        }

        playerBalance += dealCost;
        balance.text = BALANCE_MARK + playerBalance.ToString() + MONEY_UNIT;

        dealButton.interactable = CheckPossibleToDeal();
    }

    private void ParchaseExecute() 
    {
        squadItemPanel.SetItemToEmptySpace(selectedItem);
    }

    private IEnumerator MessageFadeAway()
    {
        yield return new WaitForSeconds(2f);
        message.active = false;
    }

    internal void SetSelectdItem(Item item, ButtonInterface itemButton) 
    {
        if (selectedButton != null)
            selectedButton.SetOutline(false);

        this.selectedItem = item;
        this.selectedButton = itemButton;
        SetSelectedPanel();

        dealButton.interactable = CheckPossibleToDeal();
    }
    private bool CheckPossibleToDeal()
    {
        if (!selectedButton.GetIsShoppingButton())
        {
            if (selectedButton.GetRemainNumber() <= 0)
                return false;

            return true;
        }

        if (selectedButton.GetRemainNumber() <= 0 || playerBalance < selectedButton.GetPrice())   //구매의 경우
            return false;

        return true;
    }

    private void SetSelectedPanel()
    {
        itemImage.sprite = Utils.GetItemIconByIconName(selectedItem.GetIconName());
        itemInfo.text = selectedItem.GetAbilityDesc();

        string buttonType = "판매";
        if(selectedButton.GetIsShoppingButton())
            buttonType = "구매";

        dealButton.GetComponentInChildren<Text>().text = buttonType;

        itemPrice.text = PRICE+ selectedButton.GetPrice().ToString() + MONEY_UNIT;
    }

    internal void SetSquadItemPanel(SquadItemPanel _squadItemPanel) { this.squadItemPanel = _squadItemPanel; }
    internal float GetSaleProbability() { return this.saleProbability; }

    internal void SetShoppingMode(bool setting) 
    {
        isShoppingMode = setting;
        gameObject.active = setting; 
    }

    internal bool GetIsShoppingMode() { return this.isShoppingMode; }
}