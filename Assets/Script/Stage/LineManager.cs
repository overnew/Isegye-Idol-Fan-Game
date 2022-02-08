using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    const int StageNameLength = 5;
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
            int stageIdx = Random.Range(0, StageNameLength-1);
            stageList.Add((StageName)stageIdx);
            points[i].sprite = pointImages[stageIdx];
        }

        points[points.Length -1].sprite = pointImages[(int)StageName.treasure];
        stageList.Add(StageName.treasure);
    }
}

public enum StageName
{
    start, 
    battle,
    cafe,
    question,
    treasure
}
