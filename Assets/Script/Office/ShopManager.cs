using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public GameObject shadow;

    private void Start()
    {
        shadow.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        shadow.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        shadow.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //shadow.SetActive(false);
    }
}