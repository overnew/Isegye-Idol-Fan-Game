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

    private int roundCounter = 0;
    private bool isTurnEnd = false;

    private bool posChangerActive = false;
    private bool posSwitching = false;
    private GameObject switchingUnit;
    private float turnXpos;
    private float selectedXpos;
    private float switchSpeed;
    private Vector3 moveDirection;

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
        skillPanel = GameObject.Find("skillPanel");
        blurCamera = GameObject.Find("BlurCamera");
        unitStatusText = GameObject.Find("unitStatus").GetComponent<Text>();

        roundCounter = 0;
        BattleStart();
        postVolume.enabled = false;
    }

    void Update()
    {
        if (posSwitching) // 위치 변경
        {
            turnUnit.transform.Translate(moveDirection * switchSpeed);
            turnUnit.GetComponent<UnitInterface>().SetUnitUIPosition();

            switchingUnit.transform.Translate(-moveDirection * switchSpeed);
            switchingUnit.GetComponent<UnitInterface>().SetUnitUIPosition();

            if (turnUnit.transform.position.x <= selectedXpos && moveDirection == Vector3.right
                || turnUnit.transform.position.x >= selectedXpos && moveDirection == Vector3.left)
            {
                switchingUnit.transform.position = new Vector3(turnXpos, switchingUnit.transform.position.y, 0);
                turnUnit.transform.position = new Vector3(selectedXpos, turnUnit.transform.position.y, 0);
                posSwitching = false;
            }
                
        }
    }

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

        turnUnit = turnList[FRONT_IDX].Value;
        turnList.RemoveAt(FRONT_IDX);

        turnUnit.GetComponent<UnitInterface>().SetTurnBar(true);
        turnUnitData = turnUnit.GetComponent<UnitInterface>().GetUnitData();

        if (turnUnitData.GetIsEnemy())
        {
            turnUnitPosition = enemyList.IndexOf(turnUnit);
            turnUnit.GetComponent<UnitInterface>().AIBattleExecute();
        }else
        {
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

        Button[] skillButtons = skillPanel.GetComponentsInChildren<Button>();
        skills = turnUnit.GetComponent<UnitInterface>().GetUnitSkills();
        const string path = "SkillsIcon/";

        for (int i=0; i< skillButtons.Length -1  ; ++i )    // execept posChangerButton in last
        {
            skillButtons[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(path + skills[i].GetIconName());
            skillButtons[i].GetComponent<SkillButton>().SetSkillToButton(skills[i]);
        }
    }

    public void SwitchPositionWithTurnUnit(GameObject selectedUnit)
    {
        switchingUnit = selectedUnit;
        turnXpos = turnUnit.transform.position.x;
        selectedXpos = selectedUnit.transform.position.x;
        switchSpeed = Math.Abs(turnXpos - selectedXpos)/20f;
        moveDirection = Vector3.right;

        if ((turnXpos - selectedXpos) < 0)
            moveDirection = Vector3.left;

        posSwitching = true;
        StartCoroutine(SwitchPosCoroutine());

        //list내 순서 변경
        int selectedUnitIndex = squadList.IndexOf(selectedUnit);
        int turnUnitIndex = squadList.IndexOf(turnUnit);
        squadList[selectedUnitIndex] = turnUnit;
        squadList[turnUnitIndex] = selectedUnit;
    }
    private IEnumerator SwitchPosCoroutine()
    {
        yield return new WaitUntil(() => !posSwitching);
        EndUnitTurn();
        isTurnEnd = true;
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
        GameObject[] targeredUnits = GetTargetedEnemy(selectedSkill.GetAttackRange(), selectedSkill.GetIsTargetedEnemy());
        SkillAnimationStart(targeredUnits);

        blurCamera.GetComponent<BlurCamera>().CameraAction(true, turnUnitData.GetIsEnemy());
        if (selectedSkill.GetIsSplashSkill())
        {
            for (int i = 0; i < targeredUnits.Length; ++i)
            {
                if (selectedSkill.GetIsBuff())
                    targeredUnits[i].GetComponent<UnitInterface>().BuffSkillExcute(selectedSkill, roundCounter);

                if(selectedSkill.GetSkillDamage() != 0)
                    targeredUnits[i].GetComponent<UnitInterface>().GetDamage();
            }
            return;
        }

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

    private void AlignUnitsInList(GameObject unit ,List<GameObject> list)
    {
        int unitIndex = list.IndexOf(unit);
        Vector3 nextPosition;
        Vector3 movePosition = unit.transform.position;
        list.Remove(unit);

        for (int i = unitIndex; i < list.Count; ++i)
        {
            nextPosition = list[i].transform.position;
            StartCoroutine(moveUnitToDest(list[i], movePosition));
            movePosition = nextPosition;
        }
    }
    private IEnumerator moveUnitToDest(GameObject unit, Vector3 dest)
    {
        Vector3 destDir = Vector3.right;
        float moveSpeed = (unit.transform.position.x - dest.x) / 20f;

        if (unit.transform.position.x - dest.x >= 0)
            destDir = Vector3.left;

        while (!(destDir.Equals(Vector3.left) && unit.transform.position.x <= dest.x) &&
            !(destDir.Equals(Vector3.right) && unit.transform.position.x >= dest.x))
        {
            unit.transform.Translate(destDir * moveSpeed);
            unit.GetComponent<UnitInterface>().SetUnitUIPosition();
            yield return new WaitForSeconds(0.01f);
        }
        unit.transform.position = dest;
    }

    public void OffAllTargerBar()
    {
        if (turnUnitData.GetIsEnemy())
        {
            for (int i=0; i<squadList.Count ;++i )
            {
                squadList[i].GetComponent<UnitInterface>().SetTargetBar(false);
            }
            return;
        }

        for (int i = 0; i < enemyList.Count; ++i)
        {
            enemyList[i].GetComponent<UnitInterface>().SetTargetBar(false);
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

        float totalDamage = random.Next(attackPowerRange[0], attackPowerRange[1] + 1) + selectedSkill.GetSkillDamage();

        if (UnityEngine.Random.Range(0, 100f) <= turnUnitData.GetCritical())
            totalDamage *= 2f;

        return totalDamage;
    }
    public void SetPosChanger(bool setting){this.posChangerActive = setting;}

    public bool GetPosChanger(){return this.posChangerActive;}
}
