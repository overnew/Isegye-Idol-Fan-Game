using System.Collections;
using UnityEngine;

public class MapActivater : MonoBehaviour
{
    public GameObject MapGroup;

    internal void SetActivate(bool setting) { MapGroup.active = setting; }
}