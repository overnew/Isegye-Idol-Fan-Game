using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class StagePoint : MonoBehaviour
{
    private Coroutine interactCoroutine = null;
    private int pointIndex;

    private Vector3 originScale;
    private Node pointNode;
    private List<StagePoint> nextStagePoints;

    private LineManager lineManager;
    private float expansionRatio = 1.5f;

    private void Start()
    {
        lineManager = GameObject.Find("stageLine").GetComponent<LineManager>();

        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        originScale = gameObject.transform.localScale;
    }

    internal void Init(int index, List<StagePoint> stagePoints)
    {
        this.pointIndex = index;
        SetNextPoints(stagePoints);
    }

    private void OnMouseEnter() 
    {
        StopPointCoroutine();
        gameObject.transform.localScale = originScale * expansionRatio;
    }

    private void OnMouseExit()
    {
        interactCoroutine = StartCoroutine(InteractCoroutine());
    }

    private void OnMouseDown()
    {
        StopPointCoroutine();
        gameObject.transform.localScale = originScale * expansionRatio;
        lineManager.MoveUnitToPoint(pointIndex, gameObject.transform.position);
    }

    internal void StopPointCoroutine()
    {
        if (interactCoroutine != null)
            StopCoroutine(interactCoroutine);

        gameObject.transform.localScale = originScale;
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

    internal Vector3 GetPointPosition() {return gameObject.transform.position;}

    internal void SetCanVisitable(bool setting) 
    { 
        gameObject.GetComponent<BoxCollider2D>().enabled = setting;
        originScale = gameObject.transform.localScale;

        if (setting)
        {
            interactCoroutine = StartCoroutine(InteractCoroutine());
            return;
        }
    }

    public void SetNextNodeEnabled()
    {
        lineManager.SetCurrentVisitablePoints(nextStagePoints);
        foreach (StagePoint nextPoint in  nextStagePoints)
        {
            nextPoint.SetCanVisitable(true);
        }
    }

    internal int GetPointIndex() { return this.pointIndex; }
    internal void SetNextPoints(List<StagePoint> stagePoints) { this.nextStagePoints = stagePoints; }
}