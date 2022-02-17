using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CafePanel : MonoBehaviour
{
    private Button[] itemButtons;

    private void Awake()
    {
        gameObject.active = false;
        itemButtons = GetComponentsInChildren<Button>();

    }

    private void CafeItemLoad()
    {
        
    }


    
}

public enum ItemIndex
{
    americano,
    즙나기
}