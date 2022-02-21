using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CafeManager : MonoBehaviour
{
    private GameObject cafePanel;
    private GameObject squadItemPanel;
    private SaveDataManager saveDataManager;

    void Awake()
    {
        saveDataManager = new SaveDataManager();
        cafePanel = GameObject.Find("CafePanel");
        cafePanel.GetComponent<CafePanel>().Init(saveDataManager);

        squadItemPanel = GameObject.Find("SquadItemPanel");
        squadItemPanel.GetComponent<SquadItemPanel>().Init(saveDataManager, cafePanel.GetComponent<CafePanel>());

        cafePanel.GetComponent<CafePanel>().SetSquadItemPanel(squadItemPanel.GetComponent<SquadItemPanel>());
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
    internal SquadItemPanel GetSquadItemPanel()
    {
        return squadItemPanel.GetComponent<SquadItemPanel>();
    }
}