using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UnitControlloer : MonoBehaviour, UnitInterface
{
    const string AVOID_SUCCESS = "회피";
    const string RED_HEXA_DECIMAL = "#C00000";  //<color=#FE4554>o</color>/ //4BE198
    const string GREEN_HEXA_DECIMAL = "#4BE198";
    const string TAUNT = "taunt";

    public GameObject unit;
    public Animator animator;
    public GameObject unitButton;
    public GameObject conditionText;

    public GameObject UIcanvas;
    public GameObject targetBar;
    public GameObject turnBar;
    public GameObject changeBar;

    public GameObject hpBar;
    public Image hpBarImage;
    public Image buffIcon;
    public Image debuffIcon;
    public Image posionIcon;
    public Image angryIcon;
    public Image tauntIcon;
    public Image deathMark;

    private BattleManager battleController;

    private const string dataBasePath = "DataBase";
    public string unintDataName = "unitData.json";
    private UnitData unitData;

    private UnitSaveData unitSaveData;

    private List<SkillData> skillsData;
    List<KeyValuePair<int, List<AbilityInterface>>> buffEndRound;

    private bool isTaunted = false;
    private GameObject tauntUnit;
    private int tauntEndRound = 0;

    private bool isPosioning = false;
    private float posionDamage = 0;
    private int posionEndRound = 0;

    private float hp;
    private float barHeight = -2.6f;
    private float buttonHeight = 1.3f;
    private float damageHeight = 2f;
    private float damageXpos = -0.4f;
    private float buffHeight = -0.15f;
    private float buffXpos = 0.2f;
    private int buffCount = 0;
    private int debuffCount = 0;

    private PanelController panelController;
    private PanelInterface panelInterface;
    private SoundManager soundManager;
    public AudioClip attackClip;

    private bool isBattleMode = false;

    private void Awake()
    {
        LoadUnitDataFromJson();

        this.animator = GetComponent<Animator>();
        SetAllIconEnable(false);
        isPosioning = false;

        hpBarImage.fillAmount = 1;
        ScaleSet();

        if (SceneManager.GetActiveScene().name.Equals("Battle"))
        {
            isBattleMode = true;

            battleController = GameObject.Find("BattleController").GetComponent<BattleManager>();

            Canvas unitCanvas = UIcanvas.GetComponent<Canvas>();
            Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();
            unitCanvas.worldCamera = cam;

            soundManager = GameObject.Find("Sound").GetComponent<SoundManager>();
        }

        buffEndRound = new List<KeyValuePair<int, List<AbilityInterface>>>();

        buffCount = 0;
        debuffCount = 0;
    }

    private void SetAllIconEnable(bool setting)
    {
        turnBar.SetActive(setting);
        changeBar.SetActive(setting);
        debuffIcon.enabled = setting;
        buffIcon.enabled = setting;
        posionIcon.enabled = setting;
        angryIcon.enabled = setting;
        tauntIcon.enabled = setting;
        deathMark.enabled = setting;
    }
    void LoadUnitDataFromJson()
    {
        string path = Path.Combine(Application.dataPath, dataBasePath, unintDataName);
        string jsonData = File.ReadAllText(path);
        unitData = JsonUtility.FromJson<UnitData>(jsonData);
        skillsData = unitData.LoadSkillData(unitData);
    }

    void Start()
    {
        SetUnitUIPosition();
        SetTargetBar(false);
        conditionText.SetActive(false);

        if (isBattleMode)
        {
            panelController = battleController.GetPanelController();
            panelInterface = panelController;
        }
        else
        {
            panelInterface = GameObject.Find("CafeManager").GetComponent<CafeManager>().GetSquadPanel();
        }
            
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.animator.SetBool("walking", true);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.animator.SetBool("walking", false);
        }
    }
    private void OnMouseEnter(){ panelInterface.LoadUnitStatusText(gameObject, true);}

    private void OnMouseExit(){ panelInterface.LoadUnitStatusText(gameObject, false);}

    public void SetUnitUIPosition()
    {
        if (!unitData.GetIsEnemy())
            damageXpos *= -1;
        Vector3 hpBarPos = new Vector3(transform.position.x, transform.position.y - barHeight, 0);
        targetBar.transform.position = turnBar.transform.position = changeBar.transform.position = hpBar.transform.position = hpBarPos;
        deathMark.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, 0);
        
        angryIcon.transform.position = new Vector3(transform.position.x + buffXpos + 0.4f, transform.position.y + buffHeight, 0);
        tauntIcon.transform.position = new Vector3(transform.position.x - buffXpos - 0.2f, transform.position.y + buffHeight, 0);

        posionIcon.transform.position = new Vector3(transform.position.x + buffXpos + 0.2f, transform.position.y + buffHeight, 0);
        debuffIcon.transform.position = new Vector3(transform.position.x + buffXpos, transform.position.y + buffHeight, 0);
        buffIcon.transform.position = new Vector3(transform.position.x - buffXpos, transform.position.y + buffHeight, 0);

        unitButton.transform.position = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + buttonHeight, 0));
        conditionText.transform.position = new Vector3(transform.position.x + damageXpos, transform.position.y + damageHeight, 0);
    }
    private void ScaleSet()
    {
        if (!unitData.GetIsEnemy())
            hpBar.transform.localEulerAngles = conditionText.transform.localEulerAngles = new Vector3(0, 180f, 0);

        hpBar.transform.localScale = targetBar.transform.localScale = changeBar.transform.localScale = conditionText.transform.localScale
            = turnBar.transform.localScale = debuffIcon.transform.localScale = buffIcon.transform.localScale  = posionIcon.transform.localScale
            = angryIcon.transform.localScale = tauntIcon.transform.localScale
            = deathMark.transform.localScale = new Vector3(0.025f, 0.025f,0f);
        unitButton.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
    }

    public Sprite GetUnitIcon()
    {
        string PATH = "Icon/" + unitData.GetUnitIconName();
        return Resources.Load<Sprite>(PATH);
    }

    public void SetTargetBar(bool setting)
    {
        targetBar.SetActive(setting);
        unitButton.SetActive(setting);
        StartCoroutine(BarMotionCoroutine(targetBar));
    }

    public void SetChangeBar(bool setting)
    {
        changeBar.SetActive(setting);
        unitButton.SetActive(setting);
        StartCoroutine(BarMotionCoroutine(changeBar));
    }

    private IEnumerator BarMotionCoroutine(GameObject bar)
    {
        Vector3 originScale = bar.transform.localScale;
        float originXScale = bar.transform.localScale.x;
        float addedXScale = originXScale / 5f, longestXScale = originXScale + addedXScale;
        float div = 20, addspeed = addedXScale / div;

        Vector3 addScale = new Vector3(addspeed, addspeed, 0);

        while (bar.activeSelf == true)
        {
            while (bar.transform.localScale.x <= longestXScale)
            {
                bar.transform.localScale += addScale;
                yield return new WaitForSeconds(0.04f);
            }

            while (bar.transform.localScale.x >= originXScale)
            {
                bar.transform.localScale -= addScale;
                yield return new WaitForSeconds(0.04f);
            }
        }

        bar.transform.localScale = originScale;
    }

    public void SetUnitSaveData(UnitSaveData _unitSaveData)
    {
        this.unitSaveData = _unitSaveData;
        this.hp = unitSaveData.GetHp();

        if (hp == -1)   // -1은 최대 체력을 의미
            hp = unitData.GetMaxHp();
    }

    public void AIBattleExecute()
    {
        System.Random random = new System.Random();
        int randomSkillIdx;
        GameObject[] enemysInRange;

        do
        {
            randomSkillIdx = random.Next(0, skillsData.Count);
            enemysInRange = battleController.GetTargetedEnemy(skillsData[randomSkillIdx].GetAttackRange(), skillsData[randomSkillIdx].GetIsTargetedEnemy());
        } while (enemysInRange == null);

        StartCoroutine(SlowlyExcuteSkill(randomSkillIdx, enemysInRange[random.Next(0, enemysInRange.Length)]));
    }

    private IEnumerator SlowlyExcuteSkill(int randomSkillIdx, GameObject randomEnemy)
    {
        yield return new WaitForSeconds(1.5f);    //잠시 대기 후 스킬 시전
        battleController.SetSelectedAbilityData(skillsData[randomSkillIdx], false);
        battleController.SkillExcute(randomEnemy);
        battleController.EndUnitTurn();
    }

    public void OnClickUnit()
    {
        if (isBattleMode)
        {
            BattelModeClick();
            return;
        }

        UsuallModeClick();
    }

    private void UsuallModeClick()
    {
        Item item = GameObject.Find("SquadPanel").GetComponent<SquadPanel>().GetSelectedItem();
        if (item.GetIsBuff())
        {
            if (item.GetBuffEffectedStatus().Equals("hp"))
            {
                HealExecute(item);
                SetTargetBar(false);
            }
        }
    }

    private void BattelModeClick()
    {
        soundManager.Play(attackClip);
        panelController.BlockAllButton();

        if (battleController.GetPosChanger())
        {
            battleController.SwitchPositionWithTurnUnit(gameObject);
            ChangeExecuted();
            return;
        }

        battleController.OffAllUnitsBar();

        if (battleController.GetSelectedAbilityData().GetIsPosChanger())
        {
            battleController.PullUnitToFront(gameObject);
            battleController.SkillExcute(gameObject);
            battleController.EndUnitTurn();
            return;
        }

        battleController.SkillExcute(gameObject);

        battleController.EndUnitTurn();
    }

    private void ChangeExecuted()   //모든 changeBar를 꺼주자.
    {
        GameObject[] squad = battleController.GetOurSquad();

        for (int i = 0; i < squad.Length; ++i)
            squad[i].GetComponent<UnitInterface>().SetChangeBar(false);

        battleController.SetPosChanger(false);
    }

    public void GetDamage()
    {
        float skillDamage = battleController.GetTotalDamage() - unitData.GetDefense();
        
        //회피 기동
        if(Random.Range(0, 100f) <= unitData.GetAvoidability())
        {
            DisplayCondition(AVOID_SUCCESS, RED_HEXA_DECIMAL);
        }
        else if (skillDamage >= 0)
        {
            DisplayCondition(skillDamage.ToString(), RED_HEXA_DECIMAL);
            hp -= skillDamage;
            hpBarImage.fillAmount = hp / unitData.GetMaxHp();
        }

        CheckHp();
    }
    public void DisplayCondition(string condition, string textColor)
    {
        conditionText.SetActive(true);
        conditionText.GetComponent<Text>().text = "<color="+ textColor + ">" + condition + "</color>";
    }

    public void UndisplayCondition()
    {
        conditionText.SetActive(false);
    }

    public void BuffSkillExcute(AbilityInterface buffSkill, int roundNum)
    {
        if (buffSkill.GetBuffEffectedStatus().Equals("hp"))
        {
            HealExecute(buffSkill);
            return;
        }
        else if (buffSkill.GetBuffEffectedStatus().Equals(TAUNT))
        {
            isTaunted = true;
            angryIcon.enabled = true;
            tauntUnit = battleController.GetTurnUnit();
            tauntEndRound = roundNum + buffSkill.GetEffectedRound();
            return;
        }
        else if (buffSkill.GetBuffEffectedStatus().Equals("posion"))
        {
            posionIcon.enabled = true;
            isPosioning = true;
            posionDamage = buffSkill.GetEffectValue();
            posionEndRound = roundNum + buffSkill.GetEffectedRound();
            return;
        }
        else if (buffSkill.GetEffectValue() < 0)
        {
            debuffIcon.enabled = true;
            ++debuffCount;
        }
        else
        {
            buffIcon.enabled = true;
            ++buffCount;
        }

        StoreRoundEndBuff(buffSkill, roundNum);
        unitData.ApplyBuffEffect(buffSkill, false);
    }

    private void HealExecute(AbilityInterface buffSkill)
    {
        float healValue = buffSkill.GetEffectValue();
        hp += healValue;
        if (hp > unitData.GetMaxHp())
            hp = unitData.GetMaxHp();

        StartCoroutine(TextDisplayCoroutine("+ " + healValue, GREEN_HEXA_DECIMAL));

        hpBarImage.fillAmount = hp / unitData.GetMaxHp();
        return;
    }

    private void GetPosionDamage()
    {
        StartCoroutine(TextDisplayCoroutine("중독  " + posionDamage.ToString(), RED_HEXA_DECIMAL));
        hp -= posionDamage;
        hpBarImage.fillAmount = hp / unitData.GetMaxHp();

        CheckHp();
    }

    private void CheckHp()
    {
        if (hp <= 0)
        {
            hp = 0;
            deathMark.enabled = true;
            battleController.ReserveUnitToDestory(gameObject);
        }

    }

    private IEnumerator TextDisplayCoroutine(string text, string textColor)
    {
        DisplayCondition(text, textColor);
        yield return new WaitForSeconds(2f);
        conditionText.SetActive(false);

        if(isBattleMode)
            battleController.DestoryReservedUnits();
    }

    private void StoreRoundEndBuff(AbilityInterface buffSkill, int roundNum)
    {
        int storedIndex = buffEndRound.FindIndex((KeyValuePair<int, List<AbilityInterface>> data) => (data.Key.Equals(buffSkill.GetEffectedRound() + roundNum)));

        if ( storedIndex != -1)
        {
            buffEndRound[storedIndex].Value.Add(buffSkill);
        }
        else
        {
            List<AbilityInterface> buffList = new List<AbilityInterface>();
            buffList.Add(buffSkill);
            buffEndRound.Add(new KeyValuePair<int, List<AbilityInterface>>(buffSkill.GetEffectedRound() + roundNum, buffList));
        }
    }

    public void CheckEndedEffect(int roundNum)
    {
        if (isPosioning)
        {
            GetPosionDamage();
            if (posionEndRound <= roundNum)
            {
                posionIcon.enabled = false;
                isPosioning = false;
                posionEndRound = 0;
            }
        }

        if (isTaunted && tauntEndRound <= roundNum)
        {
            isTaunted = false;
            angryIcon.enabled = false;
            tauntUnit.GetComponent<UnitInterface>().SetTauntIcon(false);
            tauntEndRound = 0;
        }

        int storedIndex = buffEndRound.FindIndex((data) => (data.Key.Equals(roundNum)));
        if (storedIndex != -1)
        {
            List<AbilityInterface> buffList = buffEndRound[storedIndex].Value;
            
            for(int i=0; i <buffList.Count ;++i)
            {
                if (buffList[i].GetBuffEffectedStatus().Equals(TAUNT))
                {
                    tauntIcon.enabled = false;
                    continue;
                }

                unitData.ApplyBuffEffect(buffList[i], true);
                if (buffList[i].GetEffectValue() < 0)
                    debuffCount--;
                else
                    buffCount--;
            }

            buffEndRound.RemoveAt(storedIndex);
        }

        if (debuffCount == 0)
            debuffIcon.enabled = false;
        if (buffCount == 0)
            buffIcon.enabled = false;
    }

    public void SetTauntIcon(bool setting)
    {
        tauntIcon.enabled = setting;
    }

    public void SetTurnBar(bool Setting)
    {
        turnBar.SetActive(Setting);
        StartCoroutine(BarMotionCoroutine(turnBar));
    }

    public List<SkillData> GetUnitSkills() { return skillsData;}
    public UnitData GetUnitData(){ return this.unitData; }

    public Animator GetAnimator() { return this.animator; }

    public float GetStepSpeed(){ return unitData.GetStepSpeed(); }
    public float GetHp(){return this.hp;}

    public GameObject GetTauntUnit() { return this.tauntUnit; }
    public bool GetIsTaunt() { return this.isTaunted; }
}


/*
[ContextMenu("To Json Data")]
void SaveUnitDataToJson()
{
    string jsonData = JsonUtility.ToJson(unitData, true);
    string path = Path.Combine(Application.dataPath, unitDataPath, unintDataName);
    File.WriteAllText(path, jsonData);
}*/

//[ContextMenu("From Json Data")]