﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class StagePoint : MonoBehaviour
{
    Coroutine interactCoroutine = null;
    private Vector3 originScale;
    private Node pointNode;
    private List<StagePoint> nextStagePoints;

    private void Start()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    private void OnMouseEnter() 
    {
        originScale = gameObject.transform.localScale;
        interactCoroutine = StartCoroutine(InteractCoroutine()); 
    }

    private void OnMouseExit()
    {
        if (interactCoroutine != null)
            StopCoroutine(interactCoroutine);

        gameObject.transform.localScale = originScale;
    }

    private void OnMouseDown()
    {
        Debug.Log("stage Start");
    }

    private IEnumerator InteractCoroutine()
    {
        float expandSize = 1.5f;
        Vector3 expandedScale = originScale * expandSize;

        float div = 10f;
        Vector3 addedScale = (expandedScale - originScale) / div;

        while (true)    //OnMouseExit() 호출 전까지 반복
        {
            while (gameObject.transform.localScale.x < expandedScale.x)
            {
                gameObject.transform.localScale += addedScale;
                yield return new WaitForSeconds(0.06f);
            }
            gameObject.transform.localScale = expandedScale;

            while (gameObject.transform.localScale.x > originScale.x)
            {
                gameObject.transform.localScale -= addedScale;
                yield return new WaitForSeconds(0.06f);
            }
            gameObject.transform.localScale = originScale;

        }

    }
    public void SetCanVisitable() 
    { 
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        
    }

    public void SetNextNodeEnabled()
    {
        foreach (StagePoint nextPoint in  nextStagePoints)
        {
            nextPoint.SetCanVisitable();
        }
    }
    public void SetNextPoints(List<StagePoint> stagePoints) { this.nextStagePoints = stagePoints; }
}