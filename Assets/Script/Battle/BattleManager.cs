using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    private const int RANGE_START_IDX = 0;
    private const int RANGE_END_IDX = 1;
    private const int STEP_BONUS_MAX = 8;
    private const int FRONT_IDX = 0;

    public GameObject[] enemys;
    private List<GameObject> squadList = new List<GameObject>();
    private List<GameObject> enemyList = new List<GameObject>();
    
    private GameObject turnUnit;
    private UnitData turnUnitData;
    private int turnUnitPosition;

    private AbilityInterface selectedAbility;
    private bool isItemSelected = false;

    private bool isTurnEnd = false;

    private bool posChangerActive = false;

    private List<KeyValuePair<float, GameObject>> turnList = new List<KeyValuePair<float, GameObject>>();
    private List<GameObject> destoryList = new List<GameObject>();

    private GameObject blurCamera;
    public PostProcessVolume postVolume;
    private int endedAnimationCount = 0;

    private PanelController panelController;
    private RoundManager roundManager;

    private SaveDataManager saveData;
    private SquadData squadData;

    private string prevSceneName;   //전투 종료 후 이전 씬으로 복귀

    void Awake()
    {
        saveData = new SaveDataManager();
        squadData = saveData.GetSquadData();

        panelController = new PanelController(squadData);
        roundManager = gameObject.GetComponent<RoundManager>();
    }

    void Start()
    {
        prevSceneName = SceneManager.GetActiveScene().name;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Battle"));

        InstantBattleUnits(); 
        blurCamera = GameObject.Find("BlurCamera");

        postVolume.enabled = false;
        BattleStart();
    }


    private void InstantBattleUnits()
    {
        List<GameObject> squadPrefabs = squadData.GetSquadUnitPrefabs();
        string[] squadUnitsName = squadData.GetSquadUnitsName();
        Vector3 instantPosition = new Vector3(transform.position.x, transform.position.y -1.35f, 0);

        for (int i=0; i< squadPrefabs.Count; ++i)   //아군 유닛은 180 방향 전환
        {
            squadList.Add(Instantiate(squadPrefabs[i], instantPosition + (Vector3.left * (i * 2 + 1)), Quaternion.Euler(0, 180.0f, 0)));
            squadList[i].GetComponent<UnitInterface>().SetUnitSaveData(squadData.GetUnitSaveDataByName(squadUnitsName[i]));   
        }

        for (int i = 0; i < enemys.Length; ++i)
            enemyList.Add(Instantiate(enemys[i], instantPosition + (Vector3.right * (i * 2 + 1)), Quaternion.identity)) ;
    }

    private void BattleStart()
    {
        StartCoroutine(WaitTurnEnding());
    }

    private IEnumerator WaitTurnEnding()
    {
        while (true)
        {
            if (turnList.Count == 0)
                EndRoundOperate();

            PlayTurn();
            yield return new WaitUntil(() => isTurnEnd);

            EndTurnOperate();
            yield return new WaitForSeconds(1.5f);  //잠시 대기

            CheckEndBattle();
            yield return new WaitForSeconds(1f);  //잠시 대기
        }
    }

    private void CheckEndBattle()
    {
        if (enemyList.Count <= 0)
            ReturnToPrevScene();
    }

    private void ReturnToPrevScene()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(prevSceneName));
        GameObject.Find("MapActivater").GetComponent<MapActivater>().SetActivate(true);
        SceneManager.UnloadScene(SceneManager.GetSceneByName("battle"));
    }

    private void EndRoundOperate()  //라운드 종료시 필요한 작업들
    {
        roundManager.ChangeRound();
        EndedBuffCheck();
        turnList = SetUnitsTurnOrder(); //새로운 턴 순서 주입
    }

    private void EndTurnOperate()   //턴 종료시 필요한 작업들
    {
        if (isItemSelected) //item 사용시 개수 감소 적용
            panelController.ApplyItemUse((Item)selectedAbility);

        panelController.OffAllButtonOutLine();
        DestoryReservedUnits();
    }

    private void PlayTurn()
    {
        isTurnEnd = false;

        for (int i = 0; i < squadList.Count; ++i)
            squadList[i].GetComponent<UnitInterface>().SetUnitUIPosition();

        for (int i = 0; i < enemyList.Count; ++i)
            enemyList[i].GetComponent<UnitInterface>().SetUnitUIPosition();

        turnUnit = turnList[FRONT_IDX].Value;
        turnList.RemoveAt(FRONT_IDX);

        turnUnit.GetComponent<UnitInterface>().SetTurnBar(true);
        turnUnitData = turnUnit.GetComponent<UnitInterface>().GetUnitData();

        if (turnUnitData.GetIsEnemy())
        {
            panelController.LoadEnemyStatus(turnUnit, true);
            panelController.BlockAllButton();
            
            turnUnitPosition = enemyList.IndexOf(turnUnit);
            turnUnit.GetComponent<UnitInterface>().AIBattleExecute();

        }else
        {
            panelController.LoadEnemyStatus(turnUnit, false);
            panelController.ActivePosChangerButton();
            turnUnitPosition = squadList.IndexOf(turnUnit);
            panelController.LoadTurnUnitStatus(turnUnit,turnUnitData);
        }
    }

    private List<KeyValuePair<float, GameObject>> SetUnitsTurnOrder()
    {
        List<KeyValuePair<float, GameObject>> turnOrderList = new List<KeyValuePair<float, GameObject>>();

        for (int i=0; i<squadList.Count ; ++i)
            turnOrderList.Add(new KeyValuePair<float, GameObject>
                (Random.Range(0,STEP_BONUS_MAX) + squadList[i].GetComponent<UnitInterface>().GetStepSpeed(),squadList[i]));

        for (int i = 0; i < enemyList.Count; ++i)
            turnOrderList.Add(new KeyValuePair<float, GameObject>
                (Random.Range(0, STEP_BONUS_MAX) + enemyList[i].GetComponent<UnitInterface>().GetStepSpeed(),enemyList[i]));

        turnOrderList.Sort((KeyValuePair<float, GameObject> pairA, KeyValuePair<float, GameObject> pairB)
            => (int)(pairB.Key -pairA.Key));  //내림차순 정렬

        return turnOrderList;
    }

    public void SwitchPositionWithTurnUnit(GameObject selectedUnit)
    {
        float turnUnitXpos = turnUnit.transform.position.x;
        float selectedUnitXpos = selectedUnit.transform.position.x;

        //list내 순서 변경
        int selectedUnitIndex = squadList.IndexOf(selectedUnit);
        int turnUnitIndex = squadList.IndexOf(turnUnit);
        squadList[selectedUnitIndex] = turnUnit;
        squadList[turnUnitIndex] = selectedUnit;

        StartCoroutine(PullUnitCoroutine(selectedUnit, turnUnitXpos, true));
        StartCoroutine(PullUnitCoroutine(turnUnit, selectedUnitXpos, true));

        StartCoroutine(WaitSwitchingEndCoroutine(selectedUnit,turnUnitXpos));
    }

    private IEnumerator WaitSwitchingEndCoroutine(GameObject selectedUnit, float destXpos)
    {
        while (selectedUnit.transform.position.x != destXpos)
        {
            yield return new WaitForSeconds(0.1f);
        }

        EndUnitTurn();
        isTurnEnd = true;
    }

    public void PullUnitToFront(GameObject selectedUnit)
    {
        List<GameObject> selectedList = squadList;
        bool isRightSide = selectedUnit.GetComponent<UnitInterface>().GetUnitData().GetIsEnemy();

        if (isRightSide)
            selectedList = enemyList;

        int selectedIndex = selectedList.IndexOf(selectedUnit);
        List<float> xPosList = new List<float>();

        for (int idx = 0; idx <= selectedIndex; ++idx)
            xPosList.Add(selectedList[idx].transform.position.x);
        
        for (int idx = selectedIndex ; idx>0 ; --idx)
        {
            StartCoroutine(PullUnitCoroutine(selectedList[idx-1], xPosList[idx], isRightSide));
            selectedList[idx] = selectedList[idx - 1];
        }

        StartCoroutine(PullUnitCoroutine(selectedUnit, xPosList[FRONT_IDX], isRightSide));
        selectedList[FRONT_IDX] = selectedUnit;
    }

    private IEnumerator PullUnitCoroutine(GameObject moveUnit, float destXpos, bool isRightSide)
    {
        const float SPEED_DIV = 10f;
        float moveSpeed = (moveUnit.transform.position.x - destXpos)/ SPEED_DIV;

        if (isRightSide)
            moveSpeed *= -1;

        int repeatCnt = 0;
        Vector3 movePostion = new Vector3(moveSpeed,0,0);
        while (++repeatCnt <= SPEED_DIV)
        {
            moveUnit.transform.position += movePostion;
            moveUnit.GetComponent<UnitInterface>().SetUnitUIPosition();
            yield return new WaitForSeconds(0.02f);
        }

        moveUnit.transform.position = new Vector3(destXpos, moveUnit.transform.position.y, 0);
    }

    private void EndedBuffCheck()
    {
        for (int i=0; i<squadList.Count ; ++i)
            squadList[i].GetComponent<UnitInterface>().CheckEndedEffect(roundManager.GetRoundCounter());

        for (int i = 0; i < enemyList.Count; ++i)
            enemyList[i].GetComponent<UnitInterface>().CheckEndedEffect(roundManager.GetRoundCounter());

    }

    public void SkillExcute(GameObject selectedUnit)
    {
        GameObject[] targeredUnits = null;

        blurCamera.GetComponent<BlurCamera>().CameraAction(true, turnUnitData.GetIsEnemy());

        if (selectedAbility.GetBuffEffectedStatus().Equals("taunt"))
            turnUnit.GetComponent<UnitInterface>().SetTauntIcon(true);

        if (selectedAbility.GetIsSplashSkill())
        {
            targeredUnits = GetTargetedEnemy(selectedAbility.GetAttackRange(), selectedAbility.GetIsTargetedEnemy());

            SkillAnimationStart(targeredUnits);
            for (int i = 0; i < targeredUnits.Length; ++i)
            {
                if (selectedAbility.GetIsBuff())
                    targeredUnits[i].GetComponent<UnitInterface>().BuffSkillExcute(selectedAbility, roundManager.GetRoundCounter());

                if(selectedAbility.GetSkillDamage() != 0)
                    targeredUnits[i].GetComponent<UnitInterface>().GetDamage();
            }
            return;
        }

        if(!selectedAbility.GetIsPosChanger())
            targeredUnits = new GameObject[] { selectedUnit };
        SkillAnimationStart(targeredUnits);

        if (selectedAbility.GetIsBuff())
            selectedUnit.GetComponent<UnitInterface>().BuffSkillExcute(selectedAbility, roundManager.GetRoundCounter());

        if (selectedAbility.GetSkillDamage() != 0)
            selectedUnit.GetComponent<UnitInterface>().GetDamage();
    }
    private void SkillAnimationStart(GameObject[] targetedUnits)
    {
        List<GameObject> squadUnits = new List<GameObject>();
        List<GameObject> enemyUnits = new List<GameObject>();
        string squadTrigger = "attack";
        string enemyTrigger = "damaged";

        endedAnimationCount = 0;

        if (turnUnitData.GetIsEnemy())
        {
            enemyUnits.Add(turnUnit);
            Utils.Swap<string>(ref squadTrigger, ref enemyTrigger);
        }
        else
        {
            squadUnits.Add(turnUnit);
        }

        if(selectedAbility.GetIsTargetedEnemy() && targetedUnits != null)
        {
            if (targetedUnits[0].GetComponent<UnitInterface>().GetUnitData().GetIsEnemy())
                enemyUnits.AddRange(targetedUnits);
            else
                squadUnits.AddRange(targetedUnits);
        }

        postVolume.enabled = true;
        SkillAnimationTrigger(squadUnits, false, squadTrigger); 
        SkillAnimationTrigger( enemyUnits, true, enemyTrigger);

        //애니메이션 끝난지 여부
        StartCoroutine(WaitAllAnimationEnd(squadUnits.Count + enemyUnits.Count));
    }

    private IEnumerator WaitAllAnimationEnd(int waitAnimeNum)
    {
        while (endedAnimationCount < waitAnimeNum)
        {
            yield return new WaitForSeconds(0.1f);
        }

        UndisplayAllUnitDamage();

        postVolume.enabled = false;

        blurCamera.GetComponent<BlurCamera>().CameraAction(false, turnUnitData.GetIsEnemy());
        while (blurCamera.transform.position.x !=0)
        {
            yield return new WaitForSeconds(0.5f);
        }

        isTurnEnd = true;
    }

    private void UndisplayAllUnitDamage()
    {
        for (int i=0; i<squadList.Count ;++i )
            squadList[i].GetComponent<UnitInterface>().UndisplayCondition();

        for (int i = 0; i < enemyList.Count; ++i)
            enemyList[i].GetComponent<UnitInterface>().UndisplayCondition();

    }

    private void SkillAnimationTrigger(List<GameObject> animationUnits, bool isEnemy, string triggerName)
    {
        if(animationUnits.Count ==0)
            return;

        Vector3 instantPosition = new Vector3(-2, -1, 0);
        float positionGap = -2f;

        if (isEnemy)    //적 유닛은 포자션을 다르게
        {
            instantPosition = new Vector3(2, -1, 0);
            positionGap *= -1;
        }

        for (int i = 0; i < animationUnits.Count; ++i)
            StartCoroutine(SkillAnimationCoroutine(animationUnits[i], instantPosition.x + (i* positionGap), animationUnits[i].transform.position.x, triggerName));
    }

    private IEnumerator SkillAnimationCoroutine(GameObject animeUnit, float aniXpos, float originXpos, string triggerName)
    {
        string originLayer = LayerMask.LayerToName(animeUnit.layer);
        Utils.ChangeLayersRecursively(animeUnit.transform, "UI");

        float div = 10;
        float moveSpeed = (aniXpos - originXpos)/div;
        Vector3 originScale = animeUnit.transform.localScale; 

        Vector3 movePosition = new Vector3( moveSpeed,0,0);
        Vector3 transScale = new Vector3((originScale.x) / div, (originScale.y) / div, 0);
        int cnt = 0;
        while (++cnt <div)
        {
            animeUnit.transform.position += movePosition;
            animeUnit.transform.localScale += transScale;
            yield return new WaitForSeconds(0.02f);
        }
        animeUnit.transform.position = new Vector3(aniXpos, animeUnit.transform.position.y, 0);

        Animator animator = animeUnit.GetComponent<UnitInterface>().GetAnimator();
        animator.SetBool(triggerName, true);

        yield return new WaitForSeconds(1.2f);  //잠시 대기 후 트리거 변경
        //while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) yield return null;
        animator.SetBool(triggerName, false);

        Utils.ChangeLayersRecursively(animeUnit.transform, originLayer);

        div = 20;
        moveSpeed = (originXpos - aniXpos) / div;
        movePosition = new Vector3(moveSpeed,0,0);
        transScale = new Vector3(-(originScale.x) / div, -(originScale.y) / div, 0);

        cnt = 0;
        while (++cnt < div)
        {
            animeUnit.transform.position += movePosition;
            animeUnit.transform.localScale += transScale;
            yield return new WaitForSeconds(0.02f);
        }
        animeUnit.transform.position = new Vector3(originXpos, animeUnit.transform.position.y, 0);
        animeUnit.transform.localScale = originScale;

        endedAnimationCount++;
    }

    public void EndUnitTurn()
    {
        if (!turnUnitData.GetIsEnemy())
            for (int i = 0; i < enemyList.Count; ++i)
                enemyList[i].GetComponent<UnitInterface>().SetTargetBar(false);

        turnUnit.GetComponent<UnitInterface>().SetTurnBar(false);
    }

    public void DestoryReservedUnits()
    {
        if (destoryList.Count == 0)
            return;

        List<GameObject> enemyDestroyList = new List<GameObject>();
        List<GameObject> squadDestroyList = new List<GameObject>();

        for (int i=0; i < destoryList.Count; ++i) 
        {
            if (destoryList[i].GetComponent<UnitInterface>().GetUnitData().GetIsEnemy())
                enemyDestroyList.Add(destoryList[i]);
            else
                squadDestroyList.Add(destoryList[i]);
        }
        destoryList.Clear();

        AlignUnitsInList(enemyDestroyList, enemyList);
        AlignUnitsInList(squadDestroyList, squadList);
    }

    public void ReserveUnitToDestory(GameObject reserveUnit)
    {
        for (int i = 0; i < turnList.Count; ++i)
            if (turnList[i].Value.Equals(reserveUnit))
            {
                turnList.RemoveAt(i);
                break;
            }
        destoryList.Add(reserveUnit);
    }

    private void AlignUnitsInList(List<GameObject> unitList ,List<GameObject> targetList)
    {
        if (unitList.Count == 0)
            return;

        for (int i=0; i<unitList.Count ;++i )
        {
            targetList.Remove(unitList[i]);
            Destroy(unitList[i]);
        }

        float destXpos;

        for (int i = 0; i < targetList.Count; ++i)
        {
            destXpos = 1 + (i * 2);
            if (targetList == squadList)
                destXpos *= -1;

            StartCoroutine(PullUnitCoroutine(targetList[i], destXpos, true));
        }
    }

    public void OffAllUnitsBar()
    {
        posChangerActive = false;
        for (int i = 0; i < squadList.Count; ++i)
        {
            squadList[i].GetComponent<UnitInterface>().SetTargetBar(false);
            squadList[i].GetComponent<UnitInterface>().SetChangeBar(false);
        }

        for (int i = 0; i < enemyList.Count; ++i)
        {
            enemyList[i].GetComponent<UnitInterface>().SetTargetBar(false);
            enemyList[i].GetComponent<UnitInterface>().SetChangeBar(false);
        }
    }

    public GameObject[] GetTargetedEnemy(int[] attackRange, bool isTargetedMyEnemy)
    {
        bool isTargetSquad;
        
        if (!turnUnitData.GetIsEnemy())
        {
            isTargetSquad = true;
            if (isTargetedMyEnemy)
                isTargetSquad = false;
        }
        else    //적인 경우
        {
            isTargetSquad = false;
            if (isTargetedMyEnemy)
                isTargetSquad = true;
        }

        if (!isTargetSquad) // 적 대상 스킬인 경우
            return GetUnitsByListRange(attackRange, enemyList, isTargetedMyEnemy);

        return GetUnitsByListRange(attackRange, squadList, isTargetedMyEnemy);
    }

    private GameObject[] GetUnitsByListRange(int[] attackRange, List<GameObject> targetList, bool isTargetedMyEnemy)
    {
        if (attackRange[RANGE_START_IDX] >= targetList.Count)
            return null;

        if (turnUnit.GetComponent<UnitInterface>().GetIsTaunt() && isTargetedMyEnemy && !selectedAbility.GetIsSplashSkill())
        {   // 도발에 걸린 경우 상대 선택이 가능한 스킬은 도발 상대만 반환
            int tauntIdx = targetList.IndexOf(turnUnit.GetComponent<UnitInterface>().GetTauntUnit());
            if (attackRange[RANGE_START_IDX] <= tauntIdx && tauntIdx <= attackRange[RANGE_END_IDX])
                return new GameObject[] { targetList[tauntIdx] };
        }

        int count = attackRange[RANGE_END_IDX] - attackRange[RANGE_START_IDX] +1;
        if (attackRange[RANGE_END_IDX] >= targetList.Count)
            count = targetList.Count - attackRange[RANGE_START_IDX];
        
        return targetList.GetRange(attackRange[RANGE_START_IDX], count).ToArray();
    }
    public GameObject[] GetOurSquad()
    {
        if (!turnUnitData.GetIsEnemy())
            return squadList.ToArray();

        return enemyList.ToArray();
    }

    public GameObject GetTurnUnit() { return this.turnUnit; }
    public int GetTurnUnitPosition() { return this.turnUnitPosition; }
    public void SetSelectedAbilityData(AbilityInterface data, bool setting) 
    { 
        this.selectedAbility = data;
        SetIsItemSelected(setting);
    }
    private void SetIsItemSelected(bool setting) { this.isItemSelected = setting; }

    public AbilityInterface GetSelectedAbilityData() { return this.selectedAbility; }

    public float GetTotalDamage() {
        System.Random random = new System.Random();
        int[] attackPowerRange = turnUnitData.GetAttackPower();

        if (Random.Range(0, 100f) > turnUnitData.GetAccuracy())
            return 0f;  //명중이 안되는 경우

        float totalDamage = random.Next(attackPowerRange[0], attackPowerRange[1] + 1) + (int)turnUnitData.GetBonusPower() + selectedAbility.GetSkillDamage();

        if (Random.Range(0, 100f) <= turnUnitData.GetCritical())
            totalDamage *= 2f;

        return totalDamage;
    }
    public void SetPosChanger(bool setting){this.posChangerActive = setting;}

    public bool GetPosChanger(){return this.posChangerActive;}
    internal PanelController GetPanelController() { return this.panelController; }
}