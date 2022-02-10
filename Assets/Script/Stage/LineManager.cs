using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    const int StageNameLength = 5;
    private int cafeCount = 0, randomCount = 0;

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
            int stageIdx;

            do
            {
                stageIdx = Random.Range(1, StageNameLength - 1);
            } while (!CheckUnderMaxPoints(stageIdx));
            
            stageList.Add((StageName)stageIdx);
            points[i].sprite = pointImages[stageIdx];

            if (stageIdx == (int)StageName.cafe)
                ++cafeCount;
            else if (stageIdx == (int)StageName.random)
                ++randomCount;
        }

        points[points.Length -1].sprite = pointImages[(int)StageName.treasure];
        stageList.Add(StageName.treasure);
    }
    private bool CheckUnderMaxPoints(int stageIdx)
    {
        if (stageIdx == (int)StageName.cafe && (int)MaxPoint.cafe <= cafeCount)
            return false;

        if (stageIdx == (int)StageName.random && (int)MaxPoint.random <= randomCount)
            return false;

        return true;
    }

}

enum StageName
{
    start, 
    battle,
    cafe,
    random,
    treasure
}

enum MaxPoint
{
    cafe = 2,
    random = 3
}
