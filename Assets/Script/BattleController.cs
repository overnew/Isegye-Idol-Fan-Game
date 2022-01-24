using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    private const int RANGE_START_IDX = 0;
    private const int RANGE_END_IDX = 1;
    private const int POS_CHANGER_IDX = 4;
    private const int STEP_BONUS_MAX = 8; 

    public GameObject[] units;
    public GameObject[] enemys;
    private List<GameObject> squadList = new List<GameObject>();
    private List<GameObject> enemyList = new List<GameObject>();

    private const int FRONT_IDX = 0;

    public Image turnUnitIcon;
    private GameObject turnUnit;
    private UnitData turnUnitData;
    private int turnUnitPosition;

    private Text unitStatusText;
    private GameObject skillPanel;
    private List<SkillData> skills;
    private SkillData selectedSkill;
    private Button[] skillButtons;

    private int roundCounter = 0;
    private bool isTurnEnd = false;

    private bool posChangerActive = false;

    private List<KeyValuePair<float, GameObject>> turnList = new List<KeyValuePair<float, GameObject>>();
    
    private GameObject blurCamera;
    public PostProcessVolume postVolume;
    private int endedAnimationCount = 0;
    

    void Awake()
    {
        BattleInit();
    }

    void Start()
    {
        blurCamera = GameObject.Find("BlurCamera");
        unitStatusText = GameObject.Find("unitStatus").GetComponent<Text>();

        skillPanel = GameObject.Find("skillPanel");
        skillButtons = skillPanel.GetComponentsInChildren<Button>();

        roundCounter = 0;
        BattleStart();
        postVolume.enabled = false;
    }

    void Update() { }

    private void BattleInit()
    {   
        Vector3 instantPosition = new Vector3(transform.position.x, transform.position.y -1, 0);

        for (int i=0; i< units.Length; ++i)
            squadList.Add(Instantiate(units[i], instantPosition + (Vector3.left * (i * 2 + 1)), Quaternion.Euler(0, 180.0f, 0)));

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
            {
                ++roundCounter;
                EndedBuffCheck();
                turnList = SetUnitsTurnOrder();
            }

            PlayTurn();
            yield return new WaitUntil(() => isTurnEnd);
        }
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
            for (int i=0; i<skillButtons.Length ; ++i)  //적 턴에는 버튼 클릭 금지
                skillButtons[i].interactable = false;
            
            turnUnitPosition = enemyList.IndexOf(turnUnit);
            turnUnit.GetComponent<UnitInterface>().AIBattleExecute();

        }else
        {
            skillButtons[POS_CHANGER_IDX].interactable = true;
            turnUnitPosition = squadList.IndexOf(turnUnit);
            LoadTurnUnitStatus();
        }
    }

    private List<KeyValuePair<float, GameObject>> SetUnitsTurnOrder()
    {
        List<KeyValuePair<float, GameObject>> turnOrderList = new List<KeyValuePair<float, GameObject>>();

        for (int i=0; i<squadList.Count ; ++i)
            turnOrderList.Add(new KeyValuePair<float, GameObject>
                (UnityEngine.Random.Range(0,STEP_BONUS_MAX) + squadList[i].GetComponent<UnitInterface>().GetStepSpeed(),squadList[i]));

        for (int i = 0; i < enemyList.Count; ++i)
            turnOrderList.Add(new KeyValuePair<float, GameObject>
                (UnityEngine.Random.Range(0, STEP_BONUS_MAX) + enemyList[i].GetComponent<UnitInterface>().GetStepSpeed(),enemyList[i]));

        turnOrderList.Sort((KeyValuePair<float, GameObject> pairA, KeyValuePair<float, GameObject> pairB)
            => (int)(pairB.Key -pairA.Key));  //내림차순 정렬

        return turnOrderList;
    }

    private void LoadTurnUnitStatus()
    {
        turnUnitIcon.sprite = turnUnit.GetComponent<UnitInterface>().GetUnitIcon();
        unitStatusText.text = turnUnitData.GetUnitStatus();

        skills = turnUnit.GetComponent<UnitInterface>().GetUnitSkills();
        const string path = "SkillsIcon/";

        for (int i=0; i< POS_CHANGER_IDX; ++i )    // execept posChangerButton in last
        {
            skillButtons[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(path + skills[i].GetIconName());
            skillButtons[i].GetComponent<SkillButton>().SetSkillToButton(skills[i]);
        }
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
            squadList[i].GetComponent<UnitInterface>().EndBuffEffect(roundCounter);

        for (int i = 0; i < enemyList.Count; ++i)
            enemyList[i].GetComponent<UnitInterface>().EndBuffEffect(roundCounter);

    }

    public void SkillExcute(GameObject selectedUnit)
    {
        GameObject[] targeredUnits;

        blurCamera.GetComponent<BlurCamera>().CameraAction(true, turnUnitData.GetIsEnemy());
        if (selectedSkill.GetIsSplashSkill())
        {
            targeredUnits = GetTargetedEnemy(selectedSkill.GetAttackRange(), selectedSkill.GetIsTargetedEnemy());
            SkillAnimationStart(targeredUnits);
            for (int i = 0; i < targeredUnits.Length; ++i)
            {
                if (selectedSkill.GetIsBuff())
                    targeredUnits[i].GetComponent<UnitInterface>().BuffSkillExcute(selectedSkill, roundCounter);

                if(selectedSkill.GetSkillDamage() != 0)
                    targeredUnits[i].GetComponent<UnitInterface>().GetDamage();
            }
            return;
        }

        targeredUnits = new GameObject[] {selectedUnit};
        SkillAnimationStart(targeredUnits);

        if (selectedSkill.GetIsBuff())
            selectedUnit.GetComponent<UnitInterface>().BuffSkillExcute(selectedSkill, roundCounter);

        if (selectedSkill.GetSkillDamage() != 0)
            selectedUnit.GetComponent<UnitInterface>().GetDamage();
    }

    private void SkillAnimationStart(GameObject[] targetedUnits)
    {
        Vector3 squadPosition = new Vector3(-3,-1,0);
        Vector3 enmeyPosition = new Vector3(3, -1, 0);
        List<GameObject> squadUnits = new List<GameObject>();
        List<GameObject> enmeyUnits = new List<GameObject>();
        endedAnimationCount = 0;

        if (turnUnitData.GetIsEnemy())
            enmeyUnits.Add(turnUnit);
        else
            squadUnits.Add(turnUnit);

        if(targetedUnits != null )
        {
            if (targetedUnits[0].GetComponent<UnitInterface>().GetUnitData().GetIsEnemy())
                enmeyUnits.AddRange(targetedUnits);
            else
                squadUnits.AddRange(targetedUnits);
        }

        SkillAnimationTrigger(squadUnits, squadPosition, Quaternion.Euler(0,180f,0)); ;
        SkillAnimationTrigger( enmeyUnits, enmeyPosition, Quaternion.identity);

        //애니메이션 끝난지 여부
        StartCoroutine(WaitAllAnimationEnd());
    }

    private IEnumerator WaitAllAnimationEnd()
    {
        while (endedAnimationCount <2)
        {
            yield return new WaitForSeconds(0.1f);
        }

        blurCamera.GetComponent<BlurCamera>().CameraAction(false, turnUnitData.GetIsEnemy());
        while (blurCamera.transform.position.x !=0)
        {
            yield return new WaitForSeconds(0.5f);
        }

        isTurnEnd = true;
    }

    private void SkillAnimationTrigger(List<GameObject> animationUnits, Vector3 instantPosition, Quaternion quaternion)
    {
        if(animationUnits.Count ==0)
        {
            endedAnimationCount++;
            return;
        }

        for (int i = 0; i < animationUnits.Count; ++i)
        {
            GameObject instantUnit = Instantiate(animationUnits[i], instantPosition + Vector3.right * (i * 2), quaternion, GameObject.Find("Canvas").transform);
            ChangeLayersRecursively(instantUnit.transform, "UI");
            instantUnit.transform.localScale =
                new Vector3(instantUnit.transform.localScale.x * 2, instantUnit.transform.localScale.y * 2, 0);
            instantUnit.GetComponent<UnitInterface>().GetAnimator().SetBool("damaged", true);
            animationUnits[i] = instantUnit;
        }

        StartCoroutine(WaitAnimationEnd(animationUnits));
    }
    private static void ChangeLayersRecursively(Transform trans, string name)
    {
        trans.gameObject.layer = LayerMask.NameToLayer(name);
        foreach (Transform child in trans)
        {
            ChangeLayersRecursively(child, name);
        }
    }

    private IEnumerator WaitAnimationEnd(List<GameObject> instantUnits)
    {
        postVolume.enabled = true;
        while (instantUnits[0].GetComponent<UnitInterface>().GetAnimator().GetCurrentAnimatorStateInfo(0).normalizedTime <1.0f)
            yield return null;

        for (int i = 0; i < instantUnits.Count; ++i)
            Destroy(instantUnits[i]);
        postVolume.enabled = false;
        endedAnimationCount++;
    }

    public void EndUnitTurn()
    {
        if (!turnUnitData.GetIsEnemy())
            for (int i = 0; i < enemyList.Count; ++i)
                enemyList[i].GetComponent<UnitInterface>().SetTargetBar(false);

        turnUnit.GetComponent<UnitInterface>().SetTurnBar(false);
    }

    public void DestoryUnit(GameObject unit)
    {
        if (unit.GetComponent<UnitInterface>().GetUnitData().GetIsEnemy())
            AlignUnitsInList(unit, enemyList);
        else
            AlignUnitsInList(unit, squadList);

        for (int i=0; i<turnList.Count ;++i)
            if (turnList[i].Value.Equals(unit))
            {
                turnList.RemoveAt(i);
                break;
            }

        Destroy(unit);
    }

    private void AlignUnitsInList(GameObject alignUnit ,List<GameObject> alignList)
    {
        int unitIndex = alignList.IndexOf(alignUnit);
        float nextXpos, currentXpos = alignUnit.transform.position.x;
        alignList.Remove(alignUnit);

        for (int i = unitIndex; i < alignList.Count; ++i)
        {
            nextXpos = alignList[unitIndex].transform.position.x;
            StartCoroutine(PullUnitCoroutine(alignList[i], currentXpos, true));
            currentXpos = nextXpos;
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
        int count = attackRange[RANGE_END_IDX] - attackRange[RANGE_START_IDX] + 1;
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
        {
            if (attackRange[RANGE_START_IDX] > enemyList.Count)
                return null;

            if (attackRange[RANGE_END_IDX] >= enemyList.Count)
                count = enemyList.Count - attackRange[RANGE_START_IDX];

            return enemyList.GetRange(attackRange[RANGE_START_IDX], count).ToArray();
        }

        if (attackRange[RANGE_START_IDX] > squadList.Count)
            return null;

        if (attackRange[RANGE_END_IDX] >= squadList.Count)
            count = squadList.Count - attackRange[RANGE_START_IDX];

        return squadList.GetRange(attackRange[RANGE_START_IDX], count).ToArray();
    }

    public GameObject[] GetOurSquad()
    {
        if (!turnUnitData.GetIsEnemy())
            return squadList.ToArray();

        return enemyList.ToArray();
    }

    public GameObject GetTurnUnit() { return this.turnUnit; }
    public int GetTurnUnitPosition() { return this.turnUnitPosition; }
    public void SetSelectedSkillData(SkillData data) { this.selectedSkill = data;}
    public SkillData GetSelectedSkillData() { return this.selectedSkill; }

    public float GetTotalDamage() {
        System.Random random = new System.Random();
        int[] attackPowerRange = turnUnitData.GetAttackPower();

        if (UnityEngine.Random.Range(0, 100f) > turnUnitData.GetAccuracy())
            return 0f;  //명중이 안되는 경우

        float totalDamage = random.Next(attackPowerRange[0], attackPowerRange[1] + 1) + (int)turnUnitData.GetBonusPower() + selectedSkill.GetSkillDamage();

        if (UnityEngine.Random.Range(0, 100f) <= turnUnitData.GetCritical())
            totalDamage *= 2f;

        return totalDamage;
    }
    public void SetPosChanger(bool setting){this.posChangerActive = setting;}

    public bool GetPosChanger(){return this.posChangerActive;}
}
