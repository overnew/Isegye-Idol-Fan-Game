using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ElevatorManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
}