using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CafeManager : MonoBehaviour
{
    public GameObject cafeButton;

    private ShoppingPanel shoppingPanel;
    private SquadItemPanel itemPanel;
    private SquadPanel squadPanel;

    private SaveDataManager saveDataManager;
    private SquadData squadData;

    private List<GameObject> squadList;
    private List<UnitSaveData> unitSaveDataList;

    void Awake()
    {
        saveDataManager = new SaveDataManager();
        squadData = saveDataManager.GetSquadData();

        shoppingPanel = GameObject.Find("CafePanel").GetComponent<ShoppingPanel>();
        shoppingPanel.Init(saveDataManager);

        squadPanel = GameObject.Find("SquadPanel").GetComponent<SquadPanel>();
        squadPanel.Init(this);  //자신과 연결

        itemPanel = GameObject.Find("SquadItemPanel").GetComponent<SquadItemPanel>();
        itemPanel.Init(saveDataManager, shoppingPanel, squadPanel);

        shoppingPanel.SetSquadItemPanel(itemPanel.GetComponent<SquadItemPanel>());

        InstantSquadUnits();
    }

    private int cnt = 0;
    public void CafeOpen()
    {
        cnt++;
        if (cnt % 2 == 0)
        {
            shoppingPanel.SetShoppingMode(false);
            itemPanel.BlockUnusableItem();
        }
        else
        {
            shoppingPanel.SetShoppingMode(true);
            itemPanel.UnblockAllButton();
        }
            
    }

    private void InstantSquadUnits()
    {
        List<GameObject> squadPrefabs = squadData.GetSquadUnitPrefabs();
        string[] squadUnitsName = squadData.GetSquadUnitsName();
        Vector3 instantPosition = new Vector3(transform.position.x - 2f, transform.position.y - 1.35f, 0);
        squadList = new List<GameObject>();

        for (int i = 0; i < squadPrefabs.Count; ++i)   //아군 유닛은 180 방향 전환
        {
            squadList.Add(Instantiate(squadPrefabs[i], instantPosition + (Vector3.left * (i * 2 + 1)), Quaternion.Euler(0, 180.0f, 0)));
            squadList[i].GetComponent<UnitInterface>().SetUnitSaveData(squadData.GetUnitSaveDataByName(squadUnitsName[i]));
        }
    }

    internal void TurnUnitButtonOn()
    {
        for (int i=0; i<squadList.Count ; ++i)
        {
            squadList[i].GetComponent<UnitInterface>().SetTargetBar(true);
        }
    }

    public void SaveChanges()   // cafe 종료시 사용
    {
        SaveSquadUnitSaveData(squadList);
        squadData.SaveSquadData(squadList, unitSaveDataList, itemPanel.GetRemainItem());
    }

    private void SaveSquadUnitSaveData(List<GameObject> squadList)
    {
        unitSaveDataList = new List<UnitSaveData>();

        for (int i = 0; i < squadList.Count; ++i)
        {
            UnitSaveData unitSaveData = squadList[i].GetComponent<UnitInterface>().GetUnitSaveData();
            unitSaveData.SetHp(squadList[i].GetComponent<UnitInterface>().GetHp());
            unitSaveDataList.Add(unitSaveData);
        }

    }

    internal SaveDataManager GetSaveDataManager() { return this.saveDataManager; }
    internal SquadItemPanel GetSquadItemPanel()
    {
        return itemPanel.GetComponent<SquadItemPanel>();
    }

    internal SquadPanel GetSquadPanel() { return squadPanel.GetComponent<SquadPanel>(); ; }

}