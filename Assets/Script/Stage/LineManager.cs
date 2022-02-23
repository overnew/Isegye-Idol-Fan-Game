using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LineManager : MonoBehaviour
{
    const int START_POINT_IDX = 0;
    const int StageNameLength = 5;
    private int cafeCount = 0, randomCount = 0;

    public string lineType;
    public Sprite[] pointImages;

    private List<SpriteRenderer> pointSprites = new List<SpriteRenderer>();
    private List<StagePoint> stagePoints;
    private List<StagePoint> currentVisitablePoints;

    private List<StageName> stageList;
    private List<Node> graph;

    private SaveDataManager saveData;
    private SquadData squadData;
    private GameObject leaderUnit;

    private void Awake()
    {
        saveData = new SaveDataManager();
        squadData = saveData.GetSquadData();
    }

    void Start()
    {
        stageList = new List<StageName>();
        MakeLinePoints();
        graph = new GraphMaker().GetLineGraph(lineType);

        SetNodeToPoints();
        //stagePoints[START_POINT_IDX].SetCanVisitable();
        stagePoints[START_POINT_IDX].SetNextNodeEnabled();

        InstantLeaderUnit();
    }

    private void InstantLeaderUnit()
    {
        leaderUnit = Instantiate(squadData.GetLeaderUnitPrefab(), stagePoints[START_POINT_IDX].GetPointPosition(), Quaternion.Euler(0, 180.0f, 0));
        leaderUnit.transform.localScale /= 1.5f; 
    }

    internal void MoveUnitToPoint(int pointIndex,Vector3 pointPosition)
    {
        BlockAllOtherPoint();
        StartCoroutine(MoveToPointCoroutine(pointIndex, pointPosition));
    }

    private void BlockAllOtherPoint()
    {
        for (int i=0; i< currentVisitablePoints.Count; ++i)
        {
            currentVisitablePoints[i].StopPointCoroutine();
            currentVisitablePoints[i].SetCanVisitable(false);
        }
    }

    private IEnumerator MoveToPointCoroutine(int pointIndex, Vector3 destPosition)
    {
        const float div = 60f;
        Vector3 moveVector = (destPosition - leaderUnit.transform.position)/ div;

        leaderUnit.GetComponent<UnitControlloer>().SetWalkingAnimation(true);

        yield return new WaitForSeconds(0.5f);  //잠시 대기 후 출발
        while (leaderUnit.transform.position.x <destPosition.x)
        {
            leaderUnit.transform.position += moveVector;
            yield return new WaitForSeconds(0.02f);
        }

        leaderUnit.transform.position = destPosition;
        leaderUnit.GetComponent<UnitControlloer>().SetWalkingAnimation(false);

        yield return new WaitForSeconds(0.5f);  //잠시 대기 후 로드
        
        LoadPointScene(pointIndex);
    }

    private void LoadPointScene(int pointIndex)
    {
        SceneManager.LoadScene((int)stageList[pointIndex]);
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

            stagePoints[i].Init(i, nextStagePoints);
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
                stageIdx = Random.Range(0, StageNameLength - 2);    //treasure과 start는 제외
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

    internal void SetCurrentVisitablePoints(List<StagePoint> points) { this.currentVisitablePoints = points;}
}

enum StageName
{
    battle,
    cafe,
    random,
    treasure,
    start
}

enum MaxPoint
{
    cafe = 2,
    random = 3
}
