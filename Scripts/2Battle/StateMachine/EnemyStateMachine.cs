using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStateMachine : MonoBehaviour
{
    private BattleStateMachine BSM;
    public BaseEnemy enemy;
    public Animator enemyBattleAnim;

    public enum TurnsState
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD,
    }
    public TurnsState currentState;

    #region battle enemy 基本設定

    //timeForAction stuff
    private bool actionStarted = false;
    //private bool enemyFireOnOff;
    private bool enemyWildOnOff = false;
    private int enemy_Wild;
    public GameObject Selector;
    public GameObject HeroToAttack;

    #endregion


    #region battle enemy UI
    //UI
    [SerializeField]
    private Image[] images_Bar;
    private const int HP = 0, SP = 1;
    public GameObject enemyUI;

    //uiのbar
    public GameObject EnemyHpPanel;
    private Transform EnemyPanelSpacer;
    private EnemyPanelState enemyStats;
    private Image EnemyHpBar;
    private Image EnemyProgress;
    private float enemycurHp = 0f;
    private float enemymaxHp = 0f;

    //barの数値
    private float cur_cooldown = 0f;
    private float max_cooldown = 20f;

    //このGameObject
    private Vector3 startposition;
    private Quaternion startRotation;
    [SerializeField] Transform skillPosition;

    //DamageText
    public GameObject FloatingTextPrefab;
    private int enemyMiss;

    #endregion

    #region battle enemy 行動

    private float animSpeed = 5f;
    //alive
    private bool alive = true;

    private float rotSpeed = 20f;
    private float moveStart;
    private bool tired = false;
    private float startHP;

    //enemyParticle
    [SerializeField] private GameObject enemyContainer;
    [SerializeField] private ParticleSystem[] enemyParticle;
    [SerializeField] private GameObject[] enemyAttack_Particle;
    public bool enemySturnOn = false;
    public ParticleSystem enemySturn;
    public ParticleSystem enemyBreak;

    #endregion

    #region その他の設定
    //得られるCoinの数
    [SerializeField] int enemyCoin;

    //敵の数え
    private int battleEnemy = 1;

    #endregion



    // Start is called before the first frame update
    void Start()
    {

        currentState = TurnsState.PROCESSING;
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        startposition = transform.position;
        Selector.SetActive(false);
        if (enemyUI)
        {
            enemyUI.SetActive(false);
        }
        startRotation = transform.rotation;
        if (GameManager.instance.advancedBattle && gameObject.tag == "Enemy")//advancedBattle設定
        {
            enemy.curHP = enemy.baseHP * (4f / 5f);
        }
        else
        {
            enemy.curHP = enemy.baseHP;
        }
        enemy.curMP = enemy.baseMP;
        enemy_Wild = 1;
        if (gameObject.tag != "EnemyBoss")
        {

            enemyCoin = Random.Range(1, 2);
            cur_cooldown = Random.Range(0, 2.5f);
        }
        else//boss初期設定
        {
            enemyCoin = 5;
            cur_cooldown = max_cooldown - 3f;
        }

        EnemyPanelSpacer = GameObject.Find("BattleCanvas").transform.Find("SelectTargetHpPanel").transform.Find("BarSpacer");
        skillPosition = GameObject.Find("enemySkillPoint").transform;
        CreateEnemyPanel();






        
        


    }

    // Update is called once per frame
    void Update()
    {
        EnemyHpBarUpdate();

        switch (currentState)
        {

            case (TurnsState.PROCESSING):

                #region TurnsStatePROCESSING
                BSM.OnAttackPanel();
                UpgradeProgressBar();
                #endregion

                break;

            case (TurnsState.CHOOSEACTION):

                #region TurnsStateCHOOSEACTION
                ChooseAction();
                currentState = TurnsState.WAITING;
                #endregion

                break;
            case (TurnsState.WAITING):

                break;

            case (TurnsState.ACTION):

                #region TurnsState ACTION
                if (enemySturnOn && !enemyWildOnOff)
                {
                    StartCoroutine(SturnAction());
                }
                else
                {
                    if (BSM.PerformList[0].choosenAttack == enemy.attacks[0])
                    {
                        StartCoroutine(TimeForAction());
                        //StartCoroutine(EnemyFireAttack());
                    }
                    else if (BSM.PerformList[0].choosenAttack == enemy.attacks[1])
                    {
                        if (gameObject.tag == "EnemyBoss")
                        {
                            StartCoroutine(EnemyWildAttack());
                        }
                        else
                        {
                            StartCoroutine(EnemyFireAttack());
                        }

                    }
                    else if (BSM.PerformList[0].choosenAttack == enemy.attacks[2])
                    {
                        StartCoroutine(EnemyStartAttack());
                    }

                }
                #endregion

                break;
            case (TurnsState.DEAD):

                #region TurnsState DEAD
                if (!alive)
                {
                    return;
                }
                else
                {
                    //change tag of enemy
                    BSM.BattleCoinAdd(enemyCoin, battleEnemy);
                    this.gameObject.tag = "DeadEnemy";
                    BSM.EnemysInBattle.Remove(this.gameObject);
                    Selector.SetActive(false);

                    if (enemyUI)
                    {
                        enemyUI.SetActive(false);
                    }

                    if (enemySturnOn || BSM.HerosInBattle[0].GetComponent<HeroStateMachine>().wildStun || enemySturn.isPlaying)
                    {
                        enemySturn.Stop();
                        enemySturn.Clear();
                        enemySturnOn = false;
                    }

                    if (BSM.EnemysInBattle.Count > 0)
                    {
                        for (int i = 0; i < BSM.PerformList.Count; i++)
                        {
                            if (i != 0)
                            {
                                if (BSM.PerformList[i].AttackersGameobject == this.gameObject)
                                {
                                    BSM.PerformList.Remove(BSM.PerformList[i]);

                                }
                                if (BSM.PerformList[i].AttackersTarget == this.gameObject)
                                {
                                    BSM.PerformList[i].AttackersTarget = BSM.EnemysInBattle[Random.Range(0, BSM.EnemysInBattle.Count)];
                                }
                            }
                        }
                    }

                    alive = false;
                    //reset enemybuttons
                    BSM.EnemyButton();
                    //check alive
                    BSM.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;
                }
                #endregion

                break;

        }

        #region updateの基本設定

        enemyBattleAnim.SetFloat("moveStart", moveStart);
        BarUpdatte();

        #endregion
    }


    #region enemy 基本設定

    private bool MoveTowardsEnemy(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir = new Vector3(dir.x + 0.8f, dir.y, dir.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    private bool MoveTowardsStart(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    private void BattleMoveRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, startRotation, rotSpeed * Time.deltaTime * 5);
    }

    #endregion

    #region enemyAction

    void ChooseAction()
    {
        HandleTurn myAttack = new HandleTurn();
        myAttack.Attacker = enemy.theName;
        myAttack.Type = "Enemy";
        myAttack.AttackersGameobject = this.gameObject;
        myAttack.AttackersTarget = BSM.HerosInBattle[0];
        int num = Random.Range(0, 4);
        if (enemy.curMP != 0)
        {
            if (gameObject.tag == "EnemyBoss")
            {
                if (enemy.curMP == enemy.baseMP)
                {
                    myAttack.choosenAttack = enemy.attacks[2];
                    BSM.CollectActions(myAttack);
                }
                else if (enemy.curHP <= 150f && enemyWildOnOff && enemy_Wild == 1)
                {
                    myAttack.choosenAttack = enemy.attacks[1];
                    BSM.CollectActions(myAttack);
                }
                else
                {
                    myAttack.choosenAttack = enemy.attacks[0];
                    BSM.CollectActions(myAttack);
                }

            }
            else
            {
                if (num >= 1)
                {
                    myAttack.choosenAttack = enemy.attacks[1];
                    BSM.CollectActions(myAttack);
                }
                else
                {
                    myAttack.choosenAttack = enemy.attacks[0];
                    BSM.CollectActions(myAttack);
                }
            }

        }
        else
        {
            myAttack.choosenAttack = enemy.attacks[0];
            BSM.CollectActions(myAttack);
        }
    }

    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        if (enemyUI)
        {
            enemyUI.SetActive(false);
        }

        BSM.heroBarStop = true;
        BSM.enemyClicked = false;
        BSM.enemyBarStop = true;

        //行動開始前の設定
        for (int i = 0; i < BSM.EnemysInBattle.Count; i++)
        {
            BSM.EnemysInBattle[i].transform.Find("EnemyUI").transform.Find("Selector").gameObject.SetActive(false);
            BSM.EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(false);
        }
        BSM.EnemySelectPanel.SetActive(false);

        //行動
        Vector3 heroPosition = new Vector3(HeroToAttack.transform.position.x - 0.8f, HeroToAttack.transform.position.y, HeroToAttack.transform.position.z);
        while (MoveTowardsEnemy(heroPosition))
        {
            moveStart = 1f;
            yield return null;
        }
        moveStart = 0f;

        if (transform.position == heroPosition)
        {
            enemyBattleAnim.SetTrigger("atk2");
            yield return new WaitForSeconds(0.6f);
            DoDamage();
            yield return new WaitForSeconds(0.6f);
        }
        Vector3 firstPosition = startposition;
        while (MoveTowardsStart(firstPosition))
        {
            moveStart = 1f;
            yield return null;
        }
        moveStart = 0f;

        if (transform.position == firstPosition)
        {
            BattleMoveRotation();
        }

        //BSMのリストでこのperformerを排除
        BSM.PerformList.RemoveAt(0);
        //BSMをwaitに戻す
        BSM.battleStates = BattleStateMachine.PerformAction.WAIT;

        if (BSM.click)
        {
            for (int i = 0; i < BSM.EnemysInBattle.Count; i++)
            {
                BSM.EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(true);

                BSM.EnemysInBattle[i].transform.Find("EnemyUI").transform.Find("Selector").gameObject.SetActive(true);
            }
            BSM.EnemySelectPanel.SetActive(true);

        }
        BSM.enemyBarStop = false;
        BSM.enemyClicked = true;
        BSM.heroBarStop = false;
        actionStarted = false;
        //enemy stateをリセット
        cur_cooldown = 0f;
        currentState = TurnsState.PROCESSING;
    }

    private IEnumerator EnemyFireAttack()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;
        if (enemyUI)
        {
            enemyUI.SetActive(false);
        }
        BSM.heroBarStop = true;
        BSM.enemyClicked = false;

        //行動開始前の設定
        for (int i = 0; i < BSM.EnemysInBattle.Count; i++)
        {
            BSM.EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(false);
            BSM.EnemysInBattle[i].transform.Find("EnemyUI").transform.Find("Selector").gameObject.SetActive(false);
        }
        BSM.EnemySelectPanel.SetActive(false);
        enemy.curMP += BSM.PerformList[0].choosenAttack.attackCost;

        //行動
        Vector3 skill_Position = new Vector3(skillPosition.position.x, skillPosition.position.y, skillPosition.position.z);
        while (MoveTowardsEnemy(skill_Position))
        {
            moveStart = 1f;
            yield return null;
        }
        moveStart = 0f;

        if (transform.position == skill_Position)
        {
            yield return new WaitForSeconds(0.5f);
            enemyBattleAnim.SetTrigger("enemy_Fire");
            yield return new WaitForSeconds(0.8f);
            GameObject enemyFireEffect = Instantiate(enemyAttack_Particle[0] as GameObject);
            enemyFireEffect.transform.position = HeroToAttack.transform.position + Vector3.up*0.2f;
            ParticleSystem enemyFireEffectParticle = enemyFireEffect.GetComponent<ParticleSystem>();
            enemyFireEffectParticle.Play();
            SoundManager.instance.PlaySound("Enemy_Wild_Attack", 1);
            yield return new WaitForSeconds(0.1f);
            for (int i = 0; i < 3; i++)
            {
                DoDamage();
                yield return new WaitForSeconds(0.2f);
            }

            yield return new WaitForSeconds(1f);
            Debug.Log("enemy_Fire");
        }

        Vector3 firstPosition = startposition;
        while (MoveTowardsStart(firstPosition))
        {
            moveStart = 1f;
            yield return null;
        }
        moveStart = 0f;

        if (transform.position == firstPosition)
        {
            BattleMoveRotation();
        }

        //BSMのリストでこのperformerを排除
        BSM.PerformList.RemoveAt(0);
        //BSMをwaitに戻す
        BSM.battleStates = BattleStateMachine.PerformAction.WAIT;

        if (BSM.click)
        {
            for (int i = 0; i < BSM.EnemysInBattle.Count; i++)
            {
                BSM.EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(true);
                BSM.EnemysInBattle[i].transform.Find("EnemyUI").transform.Find("Selector").gameObject.SetActive(true);
            }
            BSM.EnemySelectPanel.SetActive(true);

        }
        BSM.enemyClicked = true;
        BSM.heroBarStop = false;
        actionStarted = false;
        cur_cooldown = 0f;
        currentState = TurnsState.PROCESSING;
    }

    private IEnumerator EnemyWildAttack()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        HeroStateMachine HSM = HeroToAttack.GetComponent<HeroStateMachine>();
        HSM.CamDMG();
        CameraShake CMS = HSM.cams[4].GetComponent<CameraShake>();

        if (enemyUI)
        {
            enemyUI.SetActive(false);
        }
        BSM.heroBarStop = true;
        BSM.enemyClicked = false;
        enemySturnOn = false;
        enemySturn.Stop();
        enemySturn.Clear();
        for (int i = 0; i < BSM.EnemysInBattle.Count; i++)
        {
            BSM.EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(false);
            BSM.EnemysInBattle[i].transform.Find("EnemyUI").transform.Find("Selector").gameObject.SetActive(false);
        }
        BSM.EnemySelectPanel.SetActive(false);

        enemy.curMP += BSM.PerformList[0].choosenAttack.attackCost;
        yield return new WaitForSeconds(0.5f);
        enemyBattleAnim.SetTrigger("enemy_Fire");
        yield return new WaitForSeconds(0.5f);
        SoundManager.instance.PlaySound("EnemyBoss_Wild_Attack", 1);
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(CMS.Shake(0.2f, 0.2f));
        yield return new WaitForSeconds(0.3f);

        GameObject enemyFireEffect = Instantiate(enemyAttack_Particle[0] as GameObject);
        enemyFireEffect.transform.SetParent(HeroToAttack.transform, false);       
        ParticleSystem enemyFireEffectParticle = enemyFireEffect.GetComponent<ParticleSystem>();        
        enemyFireEffectParticle.Play();
        SoundManager.instance.PlaySound("EnemyBoss_Wild_Attack0.1", 1);

        yield return new WaitForSeconds(0.6f);
        StartCoroutine(CMS.Shake(0.8f, 0.5f));
        for (int i = 0; i < 4; i++)
        {
            DoDamage();
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(1f);
        HSM.CamMain();
        yield return new WaitForSeconds(0.5f);

        //BSMのリストでこのperformerを排除
        BSM.PerformList.RemoveAt(0);
        //BSMをwaitに戻す
        BSM.battleStates = BattleStateMachine.PerformAction.WAIT;

        if (BSM.click)
        {
            for (int i = 0; i < BSM.EnemysInBattle.Count; i++)
            {
                BSM.EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(true);
                BSM.EnemysInBattle[i].transform.Find("EnemyUI").transform.Find("Selector").gameObject.SetActive(true);
            }
            BSM.EnemySelectPanel.SetActive(true);

        }
        enemyWildOnOff = false;
        enemy_Wild = 0;
        BSM.enemyClicked = true;
        BSM.heroBarStop = false;
        actionStarted = false;
        //enemy stateをリセット
        cur_cooldown = 0f;
        currentState = TurnsState.PROCESSING;
    }

    private IEnumerator EnemyStartAttack()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;
        if (enemyUI)
        {
            enemyUI.SetActive(false);
        }
        BSM.heroBarStop = true;
        BSM.enemyClicked = false;
        for (int i = 0; i < BSM.EnemysInBattle.Count; i++)
        {
            BSM.EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(false);
            BSM.EnemysInBattle[i].transform.Find("EnemyUI").transform.Find("Selector").gameObject.SetActive(false);
        }
        BSM.EnemySelectPanel.SetActive(false);

        yield return new WaitForSeconds(1.5f);
        enemy.curMP += BSM.PerformList[0].choosenAttack.attackCost;
        enemyBattleAnim.SetTrigger("atk_Start");

        yield return new WaitForSeconds(0.6f);
        HeroStateMachine HSM = HeroToAttack.GetComponent<HeroStateMachine>();
        HSM.CamDMG();
        CameraShake CMS = HSM.cams[4].GetComponent<CameraShake>();
        StartCoroutine(CMS.Shake(0.8f, 0.5f));
        for (int i = 0; i < 3; i++)
        {

            if (!BSM.heroStun && enemyMiss != 4)
            {
                BSM.heroStun = true;
                
            }
            DoDamage();
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(1f);
        HSM.CamMain();

        //BSMのリストでこのperformerを排除
        BSM.PerformList.RemoveAt(0);
        //BSMをwaitに戻す
        BSM.battleStates = BattleStateMachine.PerformAction.WAIT;

        if (BSM.click)
        {
            for (int i = 0; i < BSM.EnemysInBattle.Count; i++)
            {
                BSM.EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(true);
                BSM.EnemysInBattle[i].transform.Find("EnemyUI").transform.Find("Selector").gameObject.SetActive(true);
            }
            BSM.EnemySelectPanel.SetActive(true);
        }
        BSM.enemyClicked = true;
        BSM.heroBarStop = false;
        actionStarted = false;
        //enemy stateをリセット
        cur_cooldown = 0f;
        currentState = TurnsState.PROCESSING;
    }

    private IEnumerator SturnAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        if (enemyUI)
        {
            enemyUI.SetActive(false);
        }
        BSM.heroBarStop = true;
        BSM.enemyClicked = false;
        //行動開始前の設定
        for (int i = 0; i < BSM.EnemysInBattle.Count; i++)
        {
            BSM.EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(false);
            BSM.EnemysInBattle[i].transform.Find("EnemyUI").transform.Find("Selector").gameObject.SetActive(false);
        }
        BSM.EnemySelectPanel.SetActive(false);

        enemyBattleAnim.SetTrigger("sturn");
        yield return new WaitForSeconds(1f);

        //BSMのリストでこのperformerを排除
        BSM.PerformList.RemoveAt(0);
        //BSMをwaitに戻す
        BSM.battleStates = BattleStateMachine.PerformAction.WAIT;

        if (BSM.click)
        {
            for (int i = 0; i < BSM.EnemysInBattle.Count; i++)
            {
                BSM.EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(true);
                BSM.EnemysInBattle[i].transform.Find("EnemyUI").transform.Find("Selector").gameObject.SetActive(true);
            }
            BSM.EnemySelectPanel.SetActive(true);
        }

        enemySturnOn = false;
        enemySturn.Stop();
        enemySturn.Clear();
        BSM.enemyClicked = true;
        BSM.heroBarStop = false;
        actionStarted = false;
        //enemy stateをリセット
        cur_cooldown = 0f;
        currentState = TurnsState.PROCESSING;
    }

    #endregion

    #region enemyDamage処理

    void DoDamage()
    {
        enemyMiss = Random.Range(0, 5);        
        {
            if (enemyMiss == 4)
            {
                float calc_damage = 0;
                HeroToAttack.GetComponent<HeroStateMachine>().TakeDamage(calc_damage);
            }
            else
            {
                float calc_damage = enemy.curATK + BSM.PerformList[0].choosenAttack.attackDamage;
                HeroToAttack.GetComponent<HeroStateMachine>().TakeDamage(calc_damage);
            }
        }
    }

    public void TakeDamage(float getDamageAmount)
    {
        if (this.gameObject.tag != "DeadEnemy")
        {
            enemy.curHP -= getDamageAmount;
            if (startHP != enemy.curHP || enemy.curHP > 0)
            {
                enemyBattleAnim.SetTrigger("takeDamage");
                if (gameObject.tag == "EnemyBoss" && enemy.curHP <= 150f)
                {
                    if (enemy_Wild == 1)
                    {
                        enemyWildOnOff = true;
                    }
                }

                if (FloatingTextPrefab)
                {
                    var go = Instantiate(FloatingTextPrefab, transform.position, Quaternion.identity, transform);
                    go.GetComponent<TextMesh>().text = getDamageAmount.ToString();
                }

                if (BSM.HerosInBattle[0].GetComponent<HeroStateMachine>().stun)
                {
                    enemySturnOn = true;
                    enemySturn.Play();
                    BSM.HerosInBattle[0].GetComponent<HeroStateMachine>().stun = false;
                }
                else if (BSM.HerosInBattle[0].GetComponent<HeroStateMachine>().wildStun)
                {
                    if (enemySturnOn)
                    {
                        enemySturnOn = false;
                    }
                    //enemySturnOn = true;
                    enemySturn.Play();

                }
                else if (!BSM.HerosInBattle[0].GetComponent<HeroStateMachine>().wildStun && !enemySturnOn)
                {
                    enemySturn.Stop();
                    enemySturn.Clear();
                }
            }

            if (enemy.curHP <= 0)
            {
                enemyBattleAnim.SetTrigger("die");
                enemy.curHP = 0;
                currentState = TurnsState.DEAD;
                for (int i = 0; i < enemyParticle.Length; i++)
                {
                    enemyParticle[i].Stop();
                    enemyParticle[i].Clear();
                }
                if(BSM.autoEnemy == gameObject)
                {
                    BSM.autoEnemy = null;
                }
                enemyBreak.Play();
                if(gameObject.tag == "EnemyBoss")
                {

                    Invoke("SetActive", 2.0f);
                }
                else
                {
                    
                    Invoke("SetActive", 1.2f);
                }
               
            }
        }
    }

    #endregion

    #region enemy UI 設定

    private void SetActive()
    {       
        enemyContainer.SetActive(false);
    }

    public void BarUpdatte()
    {
        if (images_Bar[HP])
        {
            images_Bar[HP].fillAmount = (float)enemy.curHP / enemy.baseHP;
        }
    }

    void EnemyHpBarUpdate()
    {
        if (this.gameObject.tag != "DeadEnemy")
        {
            enemycurHp = enemy.curHP;
            enemymaxHp = enemy.baseHP;
            float calc_Hp = enemycurHp / enemymaxHp;
            EnemyHpBar.transform.localScale = new Vector3(Mathf.Clamp(calc_Hp, 0, 1), EnemyHpBar.transform.localScale.y, EnemyHpBar.transform.localScale.z);//Mathf.Clamp(position,最小位置,最大位置)で動きを制限する。(scale)
            enemyStats.enemyHpText.text = ((int)(enemy.curHP / enemy.baseHP * 100)).ToString() + '%';
            if (enemy.curHP == 0)
            {
                Invoke("Destroy", 1.2f);
            }
        }

    }

    void UpgradeProgressBar()
    {
        if (!BSM.enemyBarStop)
            cur_cooldown = cur_cooldown + Time.deltaTime * 3f;
        float calc_cooldown = cur_cooldown / max_cooldown;
        EnemyProgress.fillAmount = calc_cooldown;
        if (cur_cooldown >= max_cooldown)
        {
            currentState = TurnsState.CHOOSEACTION;
        }

    }

    void CreateEnemyPanel()
    {
        EnemyHpPanel = Instantiate(EnemyHpPanel) as GameObject;//gameobjectによって入力されている値を呼ぶ。
        enemyStats = EnemyHpPanel.GetComponent<EnemyPanelState>();
        EnemyHpBar = enemyStats.enemyProgressBar;
        enemyStats.enemyHpText.text = ((int)(enemy.curHP / enemy.baseHP * 100)).ToString() + '%';
        EnemyProgress = enemyStats.enemyProgressBar_Turn;
        EnemyHpPanel.transform.SetParent(EnemyPanelSpacer, false);
    }

    #endregion

    #region その他の設定

    private void Destroy()
    {
        Destroy(EnemyHpPanel);
    }

    public void Boss_StartAttack_Sound()
    {
        SoundManager.instance.PlaySound("Event_Boss_End", 1);
    }

    #endregion
}
