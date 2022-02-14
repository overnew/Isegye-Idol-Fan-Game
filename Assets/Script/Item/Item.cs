using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    private const string ITEM_DESC_SETTING = "";

    private string name;
    [SerializeField] private string effectedStatus = "";
    [SerializeField] private float effectValue;
    [SerializeField] private int effectedRound;

    [SerializeField] private bool isTargetedEnemy;
    [SerializeField] private int[] attackRange;
    [SerializeField] private bool isSplash;

    private string iconName;

    public string GetIconName() { return this.iconName; }
}
