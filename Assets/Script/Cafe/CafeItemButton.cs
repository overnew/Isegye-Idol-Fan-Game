using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CafeItemButton : MonoBehaviour
{
    private Item item;
    public Text remainNum;

    internal void SetItemToButton(Item _item) 
    { 
        this.item = _item;
        remainNum.text = item.GetSellingNumber().ToString();
    }

    public void OnClickButton()
    {
        Debug.Log("비싸네");
    }


}
