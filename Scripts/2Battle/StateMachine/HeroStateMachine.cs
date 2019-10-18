using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStateMachine : MonoBehaviour
{
    public BaseHero hero;
    public Animator battleAnim;
    BattleStateMachine BSM;
    CameraClick camClick;
    AlarmSlideManager alarmManager;
    [SerializeField]
    private GameObject playerAvatar;

    //playerのenum
    public enum TurnsState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        SELECING,
        ACTION,
        DEAD,
    }
    public TurnsState currentState;

    #region battle playerの基本設定

    //IeNumerator
    public GameObject EnemyToAttack;//単一攻撃のターゲット
    public List<GameObject> KillEnemy = new List<GameObject>();//全体攻撃(wild)のターゲット
    private Vector3 startPosition;//最初の位置を記憶
    private Quaternion startRotation;//最初の方向を記憶
    public bool actionStarted = false;
    public bool rotTime_Cor = false;
    public bool coolTimeStart = false;

    #endregion

    #region battle player UI

    //UI
    private HeroPanelStats stats;
    public GameObject HeroPanel;
    public GameObject buff_Icon;
    public GameObject fieldBuff_Icon;
    private GameObject buffIcon;
    private GameObject fieldBuffIcon;
    private GameObject knockDownIcon;
    public GameObject knockdown_Icon;
    private Transform HeroPanelSpacer;
    private GameObject BuffPanel;
    private float cur_cooldown = 0f;
    private float max_cooldown = 20f;   
    public float maxcoolDown   = 4f;
    public float coolDown;
    //startのhp GameManagerから前のシーンのhpを入れる
    private float startHP;
    //barの数値
    private Image BattleHp;
    private Image BattleMp;
    private Image ProgressBar;
    //Player_DMG_Text
    public GameObject Player_FloatingTextPrefab;
    public GameObject Player_MP_Text;

    #endregion    

    #region battle playerの行動

    //Turnの数え
    public int turnInt;

    //buff   

    public int buffInt = 4;//buff使用ターン
    public int buffOn = 1;
    public float buffAtkDmg;//buffで追加されるダメージ

    //buff用のcamera設定
    public GameObject camRotateSpace;//buffの時だけon
    public bool buffCameraReset = false;

    //sturn
    public bool stun = false;
    public bool wildStun = false;

    //Wild(全体攻撃)設定
    public Transform wildTarget;
    public GameObject blackScreenObject;

    //アニメーション
    private bool atk = false;//attackの場合tired処理ｘ   
    private float moveStart;//移動しwalkアニメーションon/off
    private bool fireAttack = false;
    public bool buffStart = false; //buff判定   
    public bool wildAtkStart = false;
    private bool wildEndAtk = false;
    private bool tiredStart = false;//tiredアニメーションon/off   
    private bool sturnStart = false;
    private float animSpeed = 8f;//移動スピード
    private float rotSpeed = 20f;//行動のrotationスピード

    //Raycast click 攻撃のの時、敵のターゲティング(selector on なった時だけtrue)
    private bool alive = true;//dead

    //battleCameraEvent
    [SerializeField] public Camera[] cams;
    public bool battleCamera = false;
    public bool wildCamera = false;

    //Particle設定
    public List<ParticleSystem> battleParticle = new List<ParticleSystem>();
    public List<GameObject> battleEffectParticle = new List<GameObject>();
    private bool fieldBuffOnOff;
    private float fieldDamage;
    [SerializeField]
    private Transform effectContainer;

    #endregion

    #region その他の設定

    [SerializeField]
    private GameObject potionItem;
    [SerializeField]
    private Inventory theInventory;

    #endregion

    void Start()
    {
        hero.curHP = GameManager.instance.hpBattleStart;
        hero.curMP = GameManager.instance.mpBattleStart;
        if (GameManager.instance.BaseATK != 0 || GameManager.instance.BaseDEF != 0 || GameManager.instance.BaseHeal != 0)
        {
            LoadInfo.LoadAllInfo();
            hero.baseATK = GameManager.instance.BaseATK;
            hero.baseDEF = GameManager.instance.BaseDEF;
            hero.baseHeal = GameManager.instance.BaseHeal;
        }
        hero.curATK = hero.baseATK;
        hero.curDEF = hero.baseDEF;
        hero.curHeal = hero.baseHeal;

        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        camClick = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraClick>();
        alarmManager = FindObjectOfType<AlarmSlideManager>();
        HeroPanelSpacer = GameObject.Find("BattleCanvas").transform.Find("HeroPanel").transform.Find("HeroPanelSpacer");
        BuffPanel = GameObject.Find("BattleCanvas").transform.Find("BuffPanel").transform.Find("BuffSpacer").gameObject;
        CreateHeroPanel();

        startPosition = transform.position;
        startRotation = transform.rotation;
        turnInt = 0;
        coolDown = 0f;
        cur_cooldown = Random.Range(0, 1.5f);
        currentState = TurnsState.PROCESSING;

        //CameraEvent:MainCameraOn
        CamMain();

        //mp100の場合最初はらParticleOn
        if (hero.curMP >= 100f)
        {
            battleParticle[0].Play();
        }
    }

    void Update()
    {
        switch (currentState)
        {


            case (TurnsState.PROCESSING):

                UpgradeProgressBar();

                break;
            case (TurnsState.ADDTOLIST):

                #region TurnsStateADDTOLIST
                turnInt++;
                FieldBuff();
                BSM.HeroToManage.Add(this.gameObject);
                currentState = TurnsState.WAITING;
                #endregion


                break;
            case (TurnsState.WAITING):


                break;
            case (TurnsState.ACTION):

                ActionCor();

                break;
            case (TurnsState.DEAD):

                #region TurnsState DEAD
                if (!alive)
                {
                    return;
                }
                else
                {
                    this.gameObject.tag = "DeadHero";
                    battleParticle[0].Stop();
                    BSM.HerosInBattle.Remove(this.gameObject);
                    BSM.HeroToManage.Remove(this.gameObject);
                    BSM.EnemySelectPanel.SetActive(false);
                    if (BSM.HerosInBattle.Count > 0)
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
                                    BSM.PerformList[i].AttackersTarget = BSM.HerosInBattle[Random.Range(0, BSM.HerosInBattle.Count)];
                                }
                            }
                        }
                    }                   
                    BSM.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;
                    alive = false;
                }
                #endregion

                break;
               
        }

        #region update基本設定

        UpdateHeroPanel();
        Tired();
        Anim();

        #endregion
    }

    #region Player行動の基本設定

    private bool RotationTowardTarget(Quaternion target, float rot)
    {
        return transform.rotation != (transform.rotation = Quaternion.Slerp(transform.rotation, target, rot * Time.deltaTime));
    }

    //敵へに移動処理
    private bool MoveTowardsEnemy(Vector3 target, float rot, float speed)
    {

        Vector3 dir = target - transform.position;//方向を求める
        dir = new Vector3(dir.x - 1.2f, dir.y, dir.z);//方向は敵      
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rot * Time.deltaTime);
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime));

    }

    //最初の位置へに移動処理
    private bool MoveTowardsStart(Vector3 target, float rot, float speed)
    {
        Vector3 dir = target - transform.position;//方向を求める
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rot * Time.deltaTime);//lerpでyellowゆっくり回転
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime));
    }

    //最初の方向に回転 playerが待機の位置に戻った時に使用
    private void BattleMoveRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, startRotation, rotSpeed * Time.deltaTime * 5f);
    }

    //hpが50以下になったらanimationをグロッギーに
    private void Tired()
    {

        if (hero.curHP > 0f && hero.curHP < 50f)
        {
            tiredStart = true;
        }
        else
        {
            tiredStart = false;
        }

        if (BSM.victory)
        {
            tiredStart = false;
        }

    }

    void TurnOff()
    {
        if (hero.curHP > 200f)
        {
            hero.curHP = 200f;
        }
        else if (hero.curHP < 0f)
        {
            hero.curHP = 0f;
        }

        if (hero.curMP > 100f)
        {
            hero.curMP = 100f;
        }
        else if (hero.curMP < 0f)
        {
            hero.curMP = 0f;
        }

        if (buffAtkDmg != 0)
        {
            hero.curHP -= 5f;
            GameObject go = Instantiate(Player_FloatingTextPrefab) as GameObject;
            go.GetComponent<Text>().text = "-5";
            go.transform.SetParent(HeroPanel.transform, false);
            buffInt--;
            if (buffInt == 0)
            {
                Destroy(buffIcon);
                buffAtkDmg = 0;
            }
        }

        //BSMをwaitに戻す
        if (BSM.battleStates != BattleStateMachine.PerformAction.WIN && BSM.battleStates != BattleStateMachine.PerformAction.LOSE)
        {

            camClick.playerTurn = false;



            if (hero.curMP >= 100f)
            {
                battleParticle[0].Play();
            }
            else
            {
                battleParticle[0].Stop();
            }
            BSM.battleStates = BattleStateMachine.PerformAction.WAIT;
        }
        else
        {
            if (BSM.victory)
            {
                StartCoroutine(Wait_Rot());

            }
            currentState = TurnsState.PROCESSING;

        }
        //end coroutine


        BSM.enemyBarStop = false;
        atk = false;

        actionStarted = false;

        //camera_reset



        //enemy stateをリセット
        cur_cooldown = 0f;
        currentState = TurnsState.PROCESSING;








    }

    void FieldBuff()
    {

        if (turnInt % 3 == 0)
        {
            int fieldBuffOn = Random.Range(0, 5);
            if (fieldBuffOn >= 3 && !fieldBuffOnOff)
            {
                fieldBuffOnOff = true;
                fieldDamage = 2f;
                AlarmSlideManager.alarmFinished = false;
                StartCoroutine(alarmManager.BattleAlarmAppear());
                fieldBuffIcon = Instantiate(fieldBuff_Icon) as GameObject;
                fieldBuffIcon.transform.SetParent(BuffPanel.transform, false);
                battleParticle[1].Play();
            }
            else if (fieldBuffOn < 2 || fieldBuffOnOff)
            {
                fieldBuffOnOff = false;
                fieldDamage = 0f;
                battleParticle[1].Stop();
                battleParticle[1].Clear();
                Destroy(fieldBuffIcon);
            }
        }
    }

    #endregion

    #region damageの処理

    //ダメージを受ける
    public void TakeDamage(float getDamageAmount)
    {

        startHP = hero.curHP;//現在のhpを代入して処理




        if (Player_FloatingTextPrefab)
        {
            GameObject go = Instantiate(Player_FloatingTextPrefab) as GameObject;
            if (getDamageAmount == 0)
            {
                go.GetComponent<Text>().text = "Miss";
                go.transform.SetParent(HeroPanel.transform, false);
            }
            else
            {
                float DMG = getDamageAmount - hero.curDEF;
                if (DMG <= 0)
                {
                    DMG = 0;
                    go.GetComponent<Text>().text = DMG.ToString();
                }
                else
                {
                    hero.curHP -= DMG;
                    go.GetComponent<Text>().text = "-" + DMG.ToString();
                }
                                
                go.transform.SetParent(HeroPanel.transform, false);
                if (BSM.heroStun)
                {
                    sturnStart = true;
                    battleParticle[7].Play();
                }
            }

            GameObject hitEffect = Instantiate(battleEffectParticle[2] as GameObject);
            ParticleSystem hitEffectParticle = hitEffect.GetComponent<ParticleSystem>();
            hitEffect.transform.SetParent(effectContainer, false);
            if (startHP != hero.curHP && hero.curHP > 0f)
            {
                battleAnim.SetTrigger("takeDamage");
                hitEffectParticle.Play();
                SoundManager.instance.PlaySound("Damaged_Player", 1);
            }
            if (hero.curHP <= 0f)
            {
                SoundManager.instance.PlaySound("Damaged_Player", 1);
                battleAnim.SetTrigger("takeDamage");
                hitEffectParticle.Play();
                battleAnim.SetTrigger("die");
                hero.curHP = 0f;
                currentState = TurnsState.DEAD;

            }
        }
    }
    //ダメージを与える
    void DoDamage(float x = 1f)
    {
        if (BSM.PerformList[0].AttackersTarget.tag != "DeadEnemy")
        {

            if (buffAtkDmg == 0)
            {
                GameObject hitEffect = Instantiate(battleEffectParticle[0]) as GameObject;
                ParticleSystem hitEffectParticle = hitEffect.GetComponent<ParticleSystem>();
                hitEffect.transform.position = BSM.PerformList[0].AttackersTarget.transform.position + Vector3.up * 0.4f + Vector3.right * 0.5f;
                hitEffectParticle.Play();
                if (stun || wildStun)
                {
                    SoundManager.instance.PlaySound("Sturn", 1);
                }
                else
                {
                    SoundManager.instance.PlaySound("Damage_Normal", 1);
                }

            }
            else
            {
                GameObject hitFireEffect = Instantiate(battleEffectParticle[1]) as GameObject;
                ParticleSystem hitFireEffectParticle = hitFireEffect.GetComponent<ParticleSystem>();
                hitFireEffect.transform.position = BSM.PerformList[0].AttackersTarget.transform.position + Vector3.up * 0.4f + Vector3.right * 0.5f;
                hitFireEffectParticle.Play();
                if (stun || wildStun)
                {
                    SoundManager.instance.PlaySound("Sturn", 1);
                }
                else
                {
                    SoundManager.instance.PlaySound("Damage_Fire", 1);
                }
            }
            float calc_damage = (int)((hero.curATK + buffAtkDmg + fieldDamage + BSM.PerformList[0].choosenAttack.attackDamage) * x);
            EnemyToAttack.GetComponent<EnemyStateMachine>().TakeDamage(calc_damage);

        }
    }
    void DoWildDamage()
    {
        for (int i = 0; i < KillEnemy.Count; i++)
        {
            if (buffAtkDmg == 0)
            {
                GameObject hitEffect = Instantiate(battleEffectParticle[0]) as GameObject;
                ParticleSystem hitEffectParticle = hitEffect.GetComponent<ParticleSystem>();                
                hitEffect.transform.position = KillEnemy[i].transform.position + Vector3.up * 0.4f - Vector3.right * 0.5f;
                hitEffectParticle.Play();
                if (stun || wildStun)
                {
                    SoundManager.instance.PlaySound("Sturn", 2);
                }
                else
                {
                    SoundManager.instance.PlaySound("Damage_Normal", 2);
                }

            }
            else
            {
                GameObject hitFireEffect = Instantiate(battleEffectParticle[1]) as GameObject;
                ParticleSystem hitFireEffectParticle = hitFireEffect.GetComponent<ParticleSystem>();
                hitFireEffect.transform.position = KillEnemy[i].transform.position + Vector3.up * 0.4f - Vector3.right * 0.5f;
                hitFireEffectParticle.Play();
                if (stun || wildStun)
                {
                    SoundManager.instance.PlaySound("Sturn", 2);
                }
                else
                {
                    SoundManager.instance.PlaySound("Damage_Fire", 2);
                }
            }
            float calc_damage = hero.curATK + buffAtkDmg + fieldDamage + BSM.PerformList[0].choosenAttack.attackDamage;
            KillEnemy[i].GetComponent<EnemyStateMachine>().TakeDamage(calc_damage);
        }
    }

    #endregion

    #region battle player UI 処理

    void UpgradeProgressBar()
    {
        if (!BSM.heroBarStop)
        {
            cur_cooldown = cur_cooldown + Time.deltaTime * 8f;
        }
        float calc_cooldown = cur_cooldown / max_cooldown;
        ProgressBar.fillAmount = calc_cooldown;
        if (cur_cooldown >= max_cooldown)
        {
            currentState = TurnsState.ADDTOLIST;
        }
    }


    //hero panelを作る
    void CreateHeroPanel()
    {
        HeroPanel = Instantiate(HeroPanel) as GameObject;//gameobjectによって入力されている値を呼ぶ。
        stats = HeroPanel.GetComponent<HeroPanelStats>();
        stats.HeroHP.text = hero.curHP.ToString();
        BattleMp = stats.BattleMpBar;
        ProgressBar = stats.Progress;
        HeroPanel.transform.SetParent(HeroPanelSpacer, false);
    }

    //ダメージによる更新
    void UpdateHeroPanel()
    {
        stats.HeroHP.text = hero.curHP.ToString() + '/' + hero.baseHP.ToString();
        BattleMp.fillAmount = hero.curMP / hero.baseMP;
    }

    #endregion

    #region PlayerAction

    void ActionCor()
    {
        if (hero.curHP > 0)
        {
            if (BSM.PerformList[0].choosenAttack == hero.attacks[0])
            {

                StartCoroutine(AttackAction());//timeforactionの場合startcorutionで呼べる。
            }
            else if (BSM.PerformList[0].choosenAttack == hero.MagicAttacks[1]/*attackName == "Fire"*/)
            {
                StartCoroutine(StunAction()); //timeforactionの場合startcorutionで呼べる。
            }
            else if (BSM.PerformList[0].choosenAttack == hero.MagicAttacks[0])
            {
                StartCoroutine(BuffAction());//timeforactionの場合startcorutionで呼べる。
            }
            else if (BSM.PerformList[0].choosenAttack == hero.WildAttack[0])
            {        
                StartCoroutine(WildAttack());//timeforactionの場合startcorutionで呼べる。
            }
            else if (BSM.PerformList[0].choosenAttack == hero.Heal[0])
            {
                StartCoroutine(HealAction());//timeforactionの場合startcorutionで呼べる。
            }
            else if (BSM.PerformList[0].choosenAttack == hero.attacks[1])
            {
                StartCoroutine(UseItem());//timeforactionの場合startcorutionで呼べる。
            }
            else if (BSM.PerformList[0].choosenAttack == hero.attacks[2])
            {
                StartCoroutine(SturnAction_Player());//timeforactionの場合startcorutionで呼べる。
            }
        }

    }

    private IEnumerator AttackAction()
    {
        if (actionStarted)
        {
            yield break;
        }
        actionStarted = true;
        atk = true;
        BSM.click = false;
        BSM.enemyBarStop = true;

        CameraShake CMS = cams[2].GetComponent<CameraShake>();
        EnemyStateMachine ESM = BSM.PerformList[0].AttackersTarget.GetComponent<EnemyStateMachine>();

        //行動開始前の設定
        for (int i = 0; i < BSM.EnemysInBattle.Count; i++)
        {
            if (!BSM.EnemysInBattle[i].transform.Find("EnemyUI").gameObject.activeSelf)
            {
                BSM.EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(true);
            }
            BSM.EnemysInBattle[i].transform.Find("EnemyUI").transform.Find("Selector").gameObject.SetActive(false);
        }

        //行動
        Vector3 enemyPosition = new Vector3(EnemyToAttack.transform.position.x + 1.2f, EnemyToAttack.transform.position.y, EnemyToAttack.transform.position.z);
        while (MoveTowardsEnemy(enemyPosition, rotSpeed, animSpeed))
        {
            moveStart = 1f;
            yield return null;
        }
        moveStart = 0f;

        //attackカメラに変更
        CamAttack();

        //攻撃開始
        if (transform.position == enemyPosition)
        {
            battleAnim.SetTrigger("atk_01");
            yield return new WaitForSeconds(0.6f);
            DoDamage();
            yield return new WaitForSeconds(0.1f);
            if (BSM.PerformList[0].AttackersTarget.tag != "DeadEnemy")
            {
                battleAnim.SetTrigger("atk_02");
                yield return new WaitForSeconds(0.45f);
                DoDamage();
                yield return new WaitForSeconds(0.1f);
                if (BSM.PerformList[0].AttackersTarget.tag != "DeadEnemy")
                {
                    battleAnim.SetTrigger("atk_03");
                    yield return new WaitForSeconds(0.75f);
                    DoDamage();
                    StartCoroutine(CMS.Shake(0.15f, 0.4f));
                    yield return new WaitForSeconds(0.1f);
                    if (!ESM.enemySturnOn || BSM.PerformList[0].AttackersTarget.tag == "DeadEnemy")
                    {
                        battleAnim.SetTrigger("toStart");
                        yield return new WaitForSeconds(1f);
                    }
                    else
                    {
                        battleAnim.SetTrigger("critical");
                        yield return new WaitForSeconds(1.6f);
                        DoDamage(1.5f);
                        battleParticle[5].Play();
                        StartCoroutine(CMS.Shake(0.25f, 0.6f));
                        ESM.enemySturn.Stop();
                        ESM.enemySturn.Clear();
                        ESM.enemySturnOn = false;
                        yield return new WaitForSeconds(1.2f);
                    }
                }
                else
                {
                    battleAnim.SetTrigger("toStart");
                    yield return new WaitForSeconds(0.8f);
                }
            }
            else
            {
                battleAnim.SetTrigger("toStart");
                yield return new WaitForSeconds(0.8f);
            }
        }

        //最初の位置に戻るanimate
        for (int i = 0; i < BSM.EnemysInBattle.Count; i++)
        {
            BSM.EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(false);       
        }

        //カメラMainに変更
        CamMain();

        //最初の位置に移動
        Vector3 firstPosition = startPosition;
        while (MoveTowardsStart(firstPosition, rotSpeed, animSpeed))
        {
            moveStart = 1f;
            yield return null;
        }
        moveStart = 0f;

        //最初の位置に戻ったらrotation
        if (transform.position == firstPosition)
        {

            while (RotationTowardTarget(startRotation, rotSpeed))
            {
                yield return null;
            }
            if (BSM.battleStates != BattleStateMachine.PerformAction.WIN && BSM.battleStates != BattleStateMachine.PerformAction.LOSE)
            {
                yield return new WaitForSeconds(0.1f);
            }            
        }

        //turn終わりのmp処理
        if (hero.curMP != hero.baseMP)
        {
            hero.curMP += BSM.PerformList[0].choosenAttack.attackCost;
            GameObject go_Mp = Instantiate(Player_MP_Text) as GameObject;
            go_Mp.GetComponent<Text>().text = "+" + BSM.PerformList[0].choosenAttack.attackCost.ToString();
            go_Mp.transform.SetParent(HeroPanel.transform, false);
        }

        //BSMのリストでこのperformerを排除
        BSM.PerformList.RemoveAt(0);
        //その後の処理
        TurnOff();
    }

    private IEnumerator HealAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;
        atk = true;
        BSM.click = false;
        BSM.enemyBarStop = true;
        coolTimeStart = true;
        //カメラBuffCameraに変更
        CamBuff();

        //行動開始前の設定
        for (int i = 0; i < BSM.EnemysInBattle.Count; i++)
        {
            BSM.EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(false);


            BSM.EnemysInBattle[i].transform.Find("EnemyUI").transform.Find("Selector").gameObject.SetActive(false);

        }

        //行動開始
        buffStart = true;
        battleParticle[8].Play();
        SoundManager.instance.PlaySound("Recovery_Heal", 1);
        yield return new WaitForSeconds(1.2f);
        if (hero.curHP != hero.baseHP)
        {
            float HEAL = BSM.PerformList[0].choosenAttack.attackHeal + hero.curHeal;
            hero.curHP += HEAL;
            GameObject go = Instantiate(Player_FloatingTextPrefab) as GameObject;
            go.GetComponent<Text>().text = "+" + HEAL.ToString();
            go.transform.SetParent(HeroPanel.transform, false);
            if (hero.curHP >= hero.baseHP)
            {
                hero.curHP = hero.baseHP;
            }
        }

        buffStart = false;
        coolDown = 1f;
        yield return new WaitForSeconds(0.8f);
        camClick.playerTurn = false;

        //カメラMainに変更
        yield return new WaitForSeconds(0.5f);
        CamMain();

        //turn終わりのmp処理
        if (hero.curMP != hero.baseMP)
        {
            hero.curMP += BSM.PerformList[0].choosenAttack.attackCost;
            GameObject go_Mp = Instantiate(Player_MP_Text) as GameObject;
            go_Mp.GetComponent<Text>().text = "+" + BSM.PerformList[0].choosenAttack.attackCost.ToString();
            go_Mp.transform.SetParent(HeroPanel.transform, false);
        }
        //BSMのリストでこのperformerを排除
        BSM.PerformList.RemoveAt(0);
        //BSMをwaitに戻す
        TurnOff();
    }

    private IEnumerator BuffAction()
    {

        if (actionStarted)
        {
            yield break;
        }
        actionStarted = true;
        atk = true;
        BSM.click = false;
        BSM.enemyBarStop = true;
        //カメラBuffCameraに変更
        CamBuff();

        //行動開始前の設定
        for (int i = 0; i < BSM.EnemysInBattle.Count; i++)
        {
            BSM.EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(false);
            BSM.EnemysInBattle[i].transform.Find("EnemyUI").transform.Find("Selector").gameObject.SetActive(false);
        }

        //行動開始
        buffStart = true;
        buffOn--;

        battleParticle[3].Play();

        yield return new WaitForSeconds(0.8f);
        SoundManager.instance.PlaySound("Buff", 1);
        battleParticle[4].Play();
        buffIcon = Instantiate(buff_Icon) as GameObject;
        buffIcon.transform.SetParent(BuffPanel.transform, false);
        if (hero.curMP != hero.baseMP)
        {
            hero.curMP += BSM.PerformList[0].choosenAttack.attackCost;
            GameObject go_Mp = Instantiate(Player_MP_Text) as GameObject;
            go_Mp.GetComponent<Text>().text = "+" + BSM.PerformList[0].choosenAttack.attackCost.ToString();
            go_Mp.transform.SetParent(HeroPanel.transform, false);
        }
        yield return new WaitForSeconds(1.2f);
        buffAtkDmg = BSM.PerformList[0].choosenAttack.attackBuff;
        buffStart = false;

        yield return new WaitForSeconds(0.8f);
        //camera_reset
        camClick.playerTurn = false;
        //カメラMainに変更
        yield return new WaitForSeconds(0.5f);
        CamMain();

        //BSMのリストでこのperformerを排除
        BSM.PerformList.RemoveAt(0);
        //その後の処理
        TurnOff();




    }

    private IEnumerator StunAction()
    {
        if (actionStarted)
        {
            yield break;
        }
        actionStarted = true;
        atk = true;
        BSM.click = false;
        BSM.enemyBarStop = true;

        EnemyStateMachine ESM = BSM.PerformList[0].AttackersTarget.GetComponent<EnemyStateMachine>();

        //行動開始前の設定
        for (int i = 0; i < BSM.EnemysInBattle.Count; i++)
        {
            if (!BSM.EnemysInBattle[i].transform.Find("EnemyUI").gameObject.activeSelf)
            {
                BSM.EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(true);
            }
            BSM.EnemysInBattle[i].transform.Find("EnemyUI").transform.Find("Selector").gameObject.SetActive(false);


        }

        //移動
        fireAttack = true;
        battleParticle[2].Play();
        SoundManager.instance.PlaySound("Lightning", 1);
        yield return new WaitForSeconds(0.5f);
        battleParticle[2].Clear();
        playerAvatar.SetActive(false);

        Vector3 enemyPosition = new Vector3(EnemyToAttack.transform.position.x - 3.4f, EnemyToAttack.transform.position.y, EnemyToAttack.transform.position.z);
        while (MoveTowardsEnemy(enemyPosition, 30f, 20f))
        {
            yield return null;
        }
        //攻撃start
        if (transform.position == enemyPosition)
        {
            battleParticle[2].Play();
            SoundManager.instance.PlaySound("Lightning", 1);
            yield return new WaitForSeconds(0.5f);
            battleParticle[2].Clear();
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y + 90f, transform.rotation.z);
            fireAttack = false;

            playerAvatar.SetActive(true);

            battleParticle[2].Play();
            SoundManager.instance.PlaySound("Lightning", 1);

            yield return new WaitForSeconds(0.5f);
            battleParticle[2].Clear();
            playerAvatar.SetActive(false);
            fireAttack = true;
            Vector3 firePosition = new Vector3(enemyPosition.x + 4.8f, enemyPosition.y, enemyPosition.z);


            int sturnOnOff = Random.Range(0, 4);
            if (sturnOnOff <= 2 && !ESM.enemySturnOn)
            {
                AlarmSlideManager.alarmInt = 1;
                AlarmSlideManager.alarmFinished = false;
                StartCoroutine(alarmManager.BattleAlarmAppear());
                theInventory.AcquireItem(potionItem.GetComponent<Item_PickUP>().item);
                stun = true;
            }
            DoDamage();

            while (MoveTowardsStart(firePosition, 30f, 20f))
            {
                yield return null;
            }
            battleParticle[2].Play();
            SoundManager.instance.PlaySound("Lightning", 1);

            yield return new WaitForSeconds(0.5f);
            battleParticle[2].Clear();

            playerAvatar.SetActive(true);
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(0.1f);
                DoDamage();
            }
            battleParticle[2].Play();
            SoundManager.instance.PlaySound("Lightning", 1);

            yield return new WaitForSeconds(0.5f);

            battleParticle[2].Clear();
            playerAvatar.SetActive(false);
        }

        //最初の位置に戻る
        for (int i = 0; i < BSM.EnemysInBattle.Count; i++)
        {
            BSM.EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(false);
        }

        Vector3 firstPosition = startPosition;
        while (MoveTowardsStart(firstPosition, 30f, 20f))
        {
            yield return null;
        }
        fireAttack = false;
        battleParticle[2].Play();
        SoundManager.instance.PlaySound("Lightning", 1);

        yield return new WaitForSeconds(0.5f);
        battleParticle[2].Clear();
        playerAvatar.SetActive(true);

        //最初の位置に戻ったらrotation
        if (transform.position == firstPosition)
        {
            BattleMoveRotation();
        }
        //turn終わりのmp処理
        if (hero.curMP != hero.baseMP)
        {
            hero.curMP += BSM.PerformList[0].choosenAttack.attackCost;
            GameObject go_Mp = Instantiate(Player_MP_Text) as GameObject;
            go_Mp.GetComponent<Text>().text = "+" + BSM.PerformList[0].choosenAttack.attackCost.ToString();
            go_Mp.transform.SetParent(HeroPanel.transform, false);
        }

        //BSMのリストでこのperformerを排除
        BSM.PerformList.RemoveAt(0);
        //BSMをwaitに戻す
        TurnOff();
    }

    private IEnumerator WildAttack()
    {
        if (actionStarted)
        {
            yield break;
        }
        actionStarted = true;
        atk = true;
        BSM.click = false;
        BSM.enemyBarStop = true;

        //行動開始前の設定
        for (int i = 0; i < BSM.EnemysInBattle.Count; i++)
        {

            BSM.EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(true);
            BSM.EnemysInBattle[i].transform.Find("EnemyUI").transform.Find("Selector").gameObject.SetActive(false);

        }

        //mp全体攻撃
        hero.curMP += BSM.PerformList[0].choosenAttack.attackCost;
        wildAtkStart = true;

        CameraShake CMS = cams[3].GetComponent<CameraShake>();

        GameObject wildBuff = Instantiate(battleEffectParticle[3]) as GameObject;
        ParticleSystem wildBuffParticle = wildBuff.GetComponent<ParticleSystem>();

        GameObject wildSlash = Instantiate(battleEffectParticle[4]) as GameObject;
        ParticleSystem wildSlashParticle = wildSlash.GetComponent<ParticleSystem>();

        GameObject wildEffect = Instantiate(battleEffectParticle[5]) as GameObject;
        ParticleSystem wildEffectParticle = wildEffect.GetComponent<ParticleSystem>();

        wildStun = true;
        wildBuff.transform.SetParent(effectContainer, false);
        wildBuffParticle.Play();

        yield return new WaitForSeconds(1.5f);
        DoWildDamage();
        battleParticle[6].Play();
        //StartCoroutine(CMS.Shake(0.15f, 0.4f));
        yield return new WaitForSeconds(0.5f);
        wildStun = false;
        wildEndAtk = true;

        yield return new WaitForSeconds(0.4f);
        wildSlashParticle.Play();
        Vector3 wildTargetPosition = new Vector3(wildTarget.position.x, wildTarget.position.y, wildTarget.position.z);
        while (MoveTowardsEnemy(wildTargetPosition, 30f, 40f))
        {
            yield return null;
        }
        SoundManager.instance.PlaySound("Wild_Attack", 1);
        //カメラWild(全体攻撃用)に変更
        CamWild();        
        wildBuffParticle.Stop();

        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(0.15f);
            wildEffectParticle.Play();
            DoWildDamage();
            StartCoroutine(CMS.Shake(0.15f, 0.4f));
        }

        yield return new WaitForSeconds(0.1f);
        wildAtkStart = false;
        wildEndAtk = false;

        yield return new WaitForSeconds(1f);
        Destroy(wildBuff);
        Destroy(wildSlash);
        Destroy(wildEffect);
        //カメラMainに変更
        CamMain();

        for (int i = 0; i < BSM.EnemysInBattle.Count; i++)
        {
            BSM.EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(false);
        }
        battleParticle[2].Play();
        SoundManager.instance.PlaySound("Lightning", 1);
        yield return new WaitForSeconds(0.5f);
        battleParticle[2].Clear();
        playerAvatar.SetActive(false);

        //最初の位置に戻る
        Vector3 firstPosition = startPosition;
        while (MoveTowardsStart(firstPosition, 30f, 40f))
        {

            yield return null;
        }
        battleParticle[2].Play();
        SoundManager.instance.PlaySound("Lightning", 1);
        yield return new WaitForSeconds(0.5f);
        battleParticle[2].Clear();
        playerAvatar.SetActive(true);

        //最初の位置に戻ったらrotation
        if (transform.position == firstPosition)
        {
            BattleMoveRotation();
        }

        //BSMのリストでこのperformerを排除
        BSM.PerformList.RemoveAt(0);
        
        //BSMをwaitに戻す
        TurnOff();

    }

    private IEnumerator UseItem()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;
        atk = true;
        BSM.click = false;
        BSM.enemyBarStop = true;

        for (int i = 0; i < BSM.EnemysInBattle.Count; i++)
        {
            //if (BSM.EnemysInBattle[i].tag != "EnemyBoss")
            //{
            BSM.EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(false);

            //}
            BSM.EnemysInBattle[i].transform.Find("EnemyUI").transform.Find("Selector").gameObject.SetActive(false);

        }

        yield return new WaitForSeconds(1.2f);
        SoundManager.instance.PlaySound("Recovery_Item", 1);
        yield return new WaitForSeconds(1f);
        SoundManager.instance.PlaySound("Recovery_Field", 1);
        battleParticle[9].Play();
        yield return new WaitForSeconds(0.5f);
        if (hero.curHP != hero.baseHP)
        {
            hero.curHP += BSM.PerformList[0].choosenAttack.attackHeal;
            GameObject go = Instantiate(Player_FloatingTextPrefab) as GameObject;
            go.GetComponent<Text>().text = "+" + BSM.PerformList[0].choosenAttack.attackHeal.ToString();
            go.transform.SetParent(HeroPanel.transform, false);
            if (hero.curHP >= hero.baseHP)
            {
                hero.curHP = hero.baseHP;

            }
        }

        yield return new WaitForSeconds(0.8f);

        //camera_reset
        sturnStart = false;
        camClick.playerTurn = false;
        yield return new WaitForSeconds(0.5f);

        //turn終わりのmp処理
        if (hero.curMP != hero.baseMP)
        {
            hero.curMP += BSM.PerformList[0].choosenAttack.attackCost;
            GameObject go_Mp = Instantiate(Player_MP_Text) as GameObject;
            go_Mp.GetComponent<Text>().text = "+" + BSM.PerformList[0].choosenAttack.attackCost.ToString();
            go_Mp.transform.SetParent(HeroPanel.transform, false);
        }

        //BSMのリストでこのperformerを排除
        BSM.PerformList.RemoveAt(0);
        //BSMをwaitに戻す
        TurnOff();
    }

    private IEnumerator SturnAction_Player()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;
        atk = true;
        BSM.click = false;
        BSM.enemyBarStop = true;

        for (int i = 0; i < BSM.EnemysInBattle.Count; i++)
        {
            //if (BSM.EnemysInBattle[i].tag != "EnemyBoss")
            //{
            BSM.EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(false);

            //}
            BSM.EnemysInBattle[i].transform.Find("EnemyUI").transform.Find("Selector").gameObject.SetActive(false);

        }

        yield return new WaitForSeconds(1.2f);
        battleParticle[7].Stop();
        battleParticle[7].Clear();
        BSM.heroStun = false;
        sturnStart = false;
        camClick.playerTurn = false;
        yield return new WaitForSeconds(0.8f);

        //BSMのリストでこのperformerを排除
        BSM.PerformList.RemoveAt(0);
        //BSMをwaitに戻す
        TurnOff();

    }

    private IEnumerator Wait_Rot()
    {
        if (rotTime_Cor)
        {
            yield break;
        }

        rotTime_Cor = true;

        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 2; i++)
        {
            battleParticle[i].Stop();
            battleParticle[i].Clear();
        }
        BuffPanel.SetActive(false);
        battleAnim.SetTrigger("victory");
        transform.LookAt(transform.position + cams[0].transform.rotation * Vector3.back, cams[0].transform.rotation * Vector3.up);

        SoundManager.instance.StopBGM();
        SoundManager.instance.PlaySound("Victory", 0);

        BSM.ResultUIAppear();

        rotTime_Cor = false;
    }

    void Anim()
    {
        battleAnim.SetFloat("moveStart", moveStart);
        battleAnim.SetBool("tired", tiredStart);
        battleAnim.SetBool("attack", atk);
        battleAnim.SetBool("fireAtk_01", fireAttack);
        battleAnim.SetBool("wild", wildAtkStart);
        battleAnim.SetBool("wildEndAtk", wildEndAtk);
        battleAnim.SetBool("buff", buffStart);
        battleAnim.SetBool("sturn", sturnStart);
    }

    #endregion

    #region Cameraの処理

    public void CamMain()
    {
        battleCamera = false;
        wildCamera = false;
        cams[0].enabled = true;
        camRotateSpace.SetActive(false);
        cams[1].enabled = false;
        cams[2].enabled = false;
        cams[3].enabled = false;
        cams[4].enabled = false;
        buffCameraReset = false;
    }

    private void CamBuff()
    {
        battleCamera = false;
        wildCamera = false;
        cams[0].enabled = false;
        camRotateSpace.SetActive(true);
        cams[1].enabled = true;
        cams[2].enabled = false;
        cams[3].enabled = false;
        cams[4].enabled = false;




    }

    private void CamAttack()
    {
        battleCamera = true;
        wildCamera = false;
        cams[0].enabled = false;
        camRotateSpace.SetActive(false);
        cams[1].enabled = false;
        cams[2].enabled = true;
        cams[3].enabled = false;
        cams[4].enabled = false;


    }

    private void CamWild()
    {
        battleCamera = false;
        wildCamera = true;
        cams[0].enabled = false;
        camRotateSpace.SetActive(false);
        cams[1].enabled = false;
        cams[2].enabled = false;
        cams[3].enabled = true;
        cams[4].enabled = false;


    }

    public void CamDMG()
    {
        battleCamera = false;
        wildCamera = false;
        cams[0].enabled = false;
        camRotateSpace.SetActive(false);
        cams[1].enabled = false;
        cams[2].enabled = false;
        cams[3].enabled = false;
        cams[4].enabled = true;
    }

    #endregion

}
