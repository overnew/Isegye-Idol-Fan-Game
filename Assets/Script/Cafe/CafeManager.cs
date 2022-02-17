using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CafeManager : MonoBehaviour
{
    private GameObject cafePanel;
    private SaveDataManager saveDataManager;

    void Awake()
    {
        saveDataManager = new SaveDataManager();
        cafePanel = GameObject.Find("CafePanel");
        cafePanel.GetComponent<CafePanel>().Init(saveDataManager);
    }

    internal SaveDataManager GetSaveDataManager() { return this.saveDataManager; }

    /*
    private void OnMouseDown()
    {
        cafePanel.active = true;
    }

    private void OnMouseEnter()
    {

    }*/

}