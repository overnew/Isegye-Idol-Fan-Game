using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static float[] LEVEL_MAX_EXP = {0,100,150,200,300,500 };
    public static void Swap<T>(ref T lhs, ref T rhs)
    {
        T temp;
        temp = lhs;
        lhs = rhs;
        rhs = temp;
    }
    public static void ChangeLayersRecursively(Transform trans, string name)
    {
        trans.gameObject.layer = LayerMask.NameToLayer(name);
        foreach (Transform child in trans)
        {
            ChangeLayersRecursively(child, name);
        }
    }

    public static IEnumerator WaitThenCallBack(float waitTime, Action callBackFunc) 
    {
        yield return new WaitForSeconds(waitTime);
        callBackFunc();
    }

    public static Sprite GetItemIconByIconName(string iconName)
    {
        const string iconPath = "ItemIcon/";
        return Resources.Load<Sprite>(iconPath + iconName);
    }
}
