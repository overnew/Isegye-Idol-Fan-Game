using System.Collections;
using UnityEngine;

public interface ButtonInterface 
{
    public bool GetIsShoppingButton();
    public int GetPrice();
    public int GetRemainNumber();

    public void DealExecute();

    public void SetOutline(bool setting);
}