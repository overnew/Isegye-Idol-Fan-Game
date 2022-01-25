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
    private List<GameObject> destoryList = new List<GameObject>();

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
            DestoryReservedUnits();
            yield return new WaitForSeconds(2.0f);  //��� ���
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
            for (int i=0; i<skillButtons.Length ; ++i)  //�� �Ͽ��� ��ư Ŭ�� ����
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
            => (int)(pairB.Key -pairA.Key));  //�������� ����

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

        //list�� ���� ����
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
        Vector3 squadPosition = new Vector3(-4,-1,0);
        Vector3 enmeyPosition = new Vector3(4, -1, 0);
        List<GameObject> squadUnits = new List<GameObject>();
        List<GameObject> enemyUnits = new List<GameObject>();
        endedAnimationCount = 0;

        if (turnUnitData.GetIsEnemy())
            enemyUnits.Add(turnUnit);
        else
            squadUnits.Add(turnUnit);

        if(targetedUnits != null )
        {
            if (targetedUnits[0].GetComponent<UnitInterface>().GetUnitData().GetIsEnemy())
                enemyUnits.AddRange(targetedUnits);
            else
                squadUnits.AddRange(targetedUnits);
        }

        postVolume.enabled = true;
        SkillAnimationTrigger(squadUnits, squadPosition); 
        SkillAnimationTrigger( enemyUnits, enmeyPosition);

        //�ִϸ��̼� ������ ����
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

    private void SkillAnimationTrigger(List<GameObject> animationUnits, Vector3 instantPosition)
    {
        if(animationUnits.Count ==0)
            return;

        for (int i = 0; i < animationUnits.Count; ++i)
            StartCoroutine(SkillAnimationCoroutine(animationUnits[i], instantPosition.x + (i*2), animationUnits[i].transform.position.x));
        
    }
    private static void ChangeLayersRecursively(Transform trans, string name)
    {
        trans.gameObject.layer = LayerMask.NameToLayer(name);
        foreach (Transform child in trans)
        {
            ChangeLayersRecursively(child, name);
        }
    }

    private IEnumerator SkillAnimationCoroutine(GameObject animeUnit, float aniXpos, float originXpos)
    {
        string originLayer = LayerMask.LayerToName(animeUnit.layer);
        ChangeLayersRecursively(animeUnit.transform, "UI");

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
        animator.SetBool("damaged", true);
        yield return new WaitForSeconds(1.5f);

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            yield return null;
        animator.SetBool("damaged", false);


        ChangeLayersRecursively(animeUnit.transform, originLayer);

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

    private void DestoryReservedUnits()
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
        int count = attackRange[RANGE_END_IDX] - attackRange[RANGE_START_IDX] + 1;
        bool isTargetSquad;

        if (!turnUnitData.GetIsEnemy())
        {
            isTargetSquad = true;
            if (isTargetedMyEnemy)
                isTargetSquad = false;
        }
        else    //���� ���
        {
            isTargetSquad = false;
            if (isTargetedMyEnemy)
                isTargetSquad = true;
        }

        if (!isTargetSquad) // �� ��� ��ų�� ���
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
            return 0f;  //������ �ȵǴ� ���

        float totalDamage = random.Next(attackPowerRange[0], attackPowerRange[1] + 1) + (int)turnUnitData.GetBonusPower() + selectedSkill.GetSkillDamage();

        if (UnityEngine.Random.Range(0, 100f) <= turnUnitData.GetCritical())
            totalDamage *= 2f;

        return totalDamage;
    }
    public void SetPosChanger(bool setting){this.posChangerActive = setting;}

    public bool GetPosChanger(){return this.posChangerActive;}
}
