using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class INEScript : MonoBehaviour, UnitInterface
{
    public GameObject unit;
    public Animator animator;
    public GameObject unitButton;
    public GameObject damageText;

    public GameObject UIcanvas;
    public GameObject targetBar;
    public GameObject turnBar;
    public GameObject changeBar;

    public GameObject hpBar;
    public Image hpBarImage;

    private BattleController battleController;

    private const string unitDataPath = "DataBase";
    public string unintDataName = "unitData.json";
    [SerializeField] private UnitData unitData;

    private List<SkillData> skillsData;
    List<KeyValuePair<int, List<SkillData>>> buffEndRound;

    //유닛 스텟
    private float hp;
    private float height = -0.4f;
    private float buttonHeight = 1.3f;

    /*
    [ContextMenu("To Json Data")]
    void SaveUnitDataToJson()
    {
        string jsonData = JsonUtility.ToJson(unitData, true);
        string path = Path.Combine(Application.dataPath, unitDataPath, unintDataName);
        File.WriteAllText(path, jsonData);
    }*/

    //[ContextMenu("From Json Data")]
    private void Awake()
    {
        LoadUnitDataFromJson();

        this.animator = GetComponent<Animator>();
        turnBar.SetActive(false);
        changeBar.SetActive(false);

        hpBarImage.fillAmount = 1;
        ScaleSet();

        battleController = GameObject.Find("BattleController").GetComponent<BattleController>();
        Canvas unitCanvas = UIcanvas.GetComponent<Canvas>();
        Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        unitCanvas.worldCamera = cam;

        buffEndRound = new List<KeyValuePair<int, List<SkillData>>>();

    }
    void LoadUnitDataFromJson()
    {
        string path = Path.Combine(Application.dataPath, unitDataPath, unintDataName);
        string jsonData = File.ReadAllText(path);
        unitData = JsonUtility.FromJson<UnitData>(jsonData);
        skillsData = unitData.LoadSkillData();
    }

    void Start()
    {
        hp = unitData.GetMaxHp();
        SetUnitUIPosition();
        SetTargetBar(false);
        damageText.SetActive(false);
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
    public void SetUnitUIPosition()
    {
        Vector3 hpBarPos = new Vector3(transform.position.x, transform.position.y + height, 0);
        targetBar.transform.position = turnBar.transform.position = changeBar.transform.position = hpBar.transform.position = hpBarPos;
        unitButton.transform.position = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + buttonHeight, 0));
        damageText.transform.position = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + buttonHeight, 0));
    }
    private void ScaleSet()
    {
        if (!unitData.GetIsEnemy())
            hpBar.transform.localEulerAngles = new Vector3(0, 180f, 0);

        hpBar.transform.localScale = targetBar.transform.localScale = changeBar.transform.localScale
            = turnBar.transform.localScale = new Vector3(0.025f, 0.025f,0f);
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
    }

    public void SetChangeBar(bool setting)
    {
        changeBar.SetActive(setting);
        unitButton.SetActive(setting);
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
        battleController.SetSelectedSkillData(skillsData[randomSkillIdx]);
        battleController.SkillExcute(randomEnemy);
        battleController.EndUnitTurn();
    }

    public void OnClickUnit()
    {
        if (battleController.GetPosChanger())
        {
            battleController.SwitchPositionWithTurnUnit(gameObject);
            ChangeExecuted();
            return;
        }

        if (battleController.GetSelectedSkillData().GetIsPosChanger())
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
        if (skillDamage > 0 && (UnityEngine.Random.Range(0, 100f) > unitData.GetAvoidability()))
        {
            hp -= skillDamage;
            hpBarImage.fillAmount = hp / unitData.GetMaxHp();
        }

        if(hp <= 0)
            battleController.DestoryUnit(gameObject);
    }
    public void DisplayDamage(float skillDamage)
    {
        damageText.SetActive(true);
        damageText.GetComponent<Text>().text = skillDamage.ToString();
    }

    public void BuffSkillExcute(SkillData buffSkill, int roundNum)
    {
        StoreRoundEndBuff(buffSkill, roundNum);
        unitData.ApplyBuffEffect(buffSkill, false);
    }
    private void StoreRoundEndBuff(SkillData buffSkill, int roundNum)
    {
        int storedIndex = buffEndRound.FindIndex((data) => (data.Key.Equals(roundNum)));
        if ( storedIndex != -1)
        {
            buffEndRound[storedIndex].Value.Add(buffSkill);
        }
        else
        {
            List<SkillData> buffList = new List<SkillData>();
            buffList.Add(buffSkill);
            buffEndRound.Add(new KeyValuePair<int, List<SkillData>>(buffSkill.GetEffectedRound() + roundNum, buffList));
        }
    }

    public void EndBuffEffect(int roundNum)
    {
        int storedIndex = buffEndRound.FindIndex((data) => (data.Key.Equals(roundNum)));
        if (storedIndex != -1)
        {
            List<SkillData> buffList = buffEndRound[storedIndex].Value;
            
            for(int i=0; i <buffList.Count ;++i )
                unitData.ApplyBuffEffect(buffList[i], true);

            buffEndRound.RemoveAt(storedIndex);
        }

    }
    public void SetTurnBar(bool Setting)
    {
        turnBar.SetActive(Setting);
    }

    public List<SkillData> GetUnitSkills() { return skillsData;}
    public UnitData GetUnitData(){ return this.unitData; }

    public Animator GetAnimator() { return this.animator; }

    public float GetStepSpeed(){ return unitData.GetStepSpeed(); }
    public float GetHp(){return this.hp;}
}