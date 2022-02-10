using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    const int START_POINT_IDX = 0;
    const int StageNameLength = 5;
    private int cafeCount = 0, randomCount = 0;

    public string lineType;
    public Sprite[] pointImages;

    private List<SpriteRenderer> pointSprites = new List<SpriteRenderer>();
    private List<StagePoint> stagePoints;

    private List<StageName> stageList;
    private List<Node> graph;

    void Start()
    {
        stageList = new List<StageName>();
        MakeLinePoints();
        graph = new GraphMaker().GetLineGraph(lineType);

        SetNodeToPoints();
        stagePoints[START_POINT_IDX].SetCanVisitable();
        stagePoints[START_POINT_IDX].SetNextNodeEnabled();
    }
    private void SetNodeToPoints()
    {
        StagePoint[] stagePointInChildren = gameObject.GetComponentsInChildren<StagePoint>();

        stagePoints = new List<StagePoint>();
        for (int i= START_POINT_IDX; i< stagePointInChildren.Length ; ++i)
        {
            stagePoints.Add(stagePointInChildren[i]);
        }

        for (int i = START_POINT_IDX; i < stagePointInChildren.Length; ++i)
        {
            List<StagePoint> nextStagePoints = new List<StagePoint>();
            List<int> nextNodeIdxs = graph[i].GetNextNodes();

            foreach (int nextIdx in nextNodeIdxs)
            {
                nextStagePoints.Add(stagePoints[nextIdx]);
            }

            stagePoints[i].SetNextPoints(nextStagePoints);
        }
    }

    private void MakeLinePoints()
    {
        SpriteRenderer[] spritesInChildren = gameObject.GetComponentsInChildren<SpriteRenderer>();

        for (int i= 1; i< spritesInChildren.Length ; ++i)
        {
            pointSprites.Add(spritesInChildren[i]);
        }

        pointSprites[START_POINT_IDX].sprite = pointImages[(int)StageName.start];
        stageList.Add(StageName.start);

        for (int i=1; i< pointSprites.Count-1 ; ++i)
        {
            int stageIdx;

            do
            {
                stageIdx = Random.Range(1, StageNameLength - 1);
            } while (!CheckUnderMaxPoints(stageIdx));
            
            stageList.Add((StageName)stageIdx);
            pointSprites[i].sprite = pointImages[stageIdx];

            if (stageIdx == (int)StageName.cafe)
                ++cafeCount;
            else if (stageIdx == (int)StageName.random)
                ++randomCount;
        }

        pointSprites[pointSprites.Count - 1].sprite = pointImages[(int)StageName.treasure];
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
