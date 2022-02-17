using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CafeManager : MonoBehaviour
{
    private GameObject cafePanel;

    private void AWake()
    {
        cafePanel = GameObject.Find("CafePanel").GetComponent<GameObject>();
    }

    private void OnMouseDown()
    {
        cafePanel.active = true;
    }
    private void OnMouseEnter()
    {

    }

}