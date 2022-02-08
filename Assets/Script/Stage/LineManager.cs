using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    const int StageNameLength = 2;
    public Sprite[] pointImages;
    private SpriteRenderer[] points;

    private List<StageName> stageList; 

    void Start()
    {
        stageList = new List<StageName>();
        MakeLinePoints();
    }

    private void MakeLinePoints()
    {
        points = gameObject.GetComponentsInChildren<SpriteRenderer>();

        points[1].sprite = pointImages[(int)StageName.start];
        stageList.Add(StageName.start);

        for (int i=2; i<points.Length-1 ; ++i)
        {
            int stageIdx = Random.Range(0, StageNameLength);
            stageList.Add((StageName)stageIdx);
            points[i].sprite = pointImages[stageIdx];
        }

    }
}

public enum StageName
{
    start, 
    cafe
}
