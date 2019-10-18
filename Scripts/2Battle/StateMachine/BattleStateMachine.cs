using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStateMachine : MonoBehaviour
{
    CameraClick camClick;
    SlideManager slideManager;
    ResultSlideManager result;
    Inventory inven;
    #region battleの基本設定

    public enum PerformAction
    {
        WAIT,
        TAKEACTION,
        PERFORMACTION,
        CHECKALIVE,
        WIN,
        LOSE

    }
    public PerformAction battleStates;

    public List<HandleTurn> PerformList = new List<HandleTurn>();
    public List<GameObject> HerosInBattle = new List<GameObject>();
    public List<GameObject> EnemysInBattle = new List<GameObject>();


    public enum HeroGui
    {
        ACTIVATE,
        WAITING,
        INPUT1,
        INPUT2,
        DONE

    }
    public HeroGui HeroInput;
    public List<GameObject> HeroToManage = new List<GameObject>();
    private HandleTurn HeroChoise;//ターン処理

    #endregion

    #region battleの進行設定
    //開始位置設定
    public List<Transform> spawnPosints = new List<Transform>();

    public GameObject blackScreen;

    public bool heroBarStop = false;
    public bool enemyBarStop = false;


    //click判定
    public bool click = false;
    public bool enemyClicked;
    //slide設定
    public bool slideOn = false;
    //処理設定
    public bool victory = false;
    public bool victoryButton = false;
    //player stun 判定
    public bool heroStun = false;
    //player autoAttack 判定
    public bool autoAttack = false;
    public GameObject autoEnemy;

    //倒した敵の数
    private int battleEnemyInt = 0;
    [SerializeField]
    private Text battleEnemyIntText;

    //敵から獲得したCoinの数
    private int battleCoin = 0;
    [SerializeField]
    private Text battleCoinText;

    #endregion

    #region UI設定

    public GameObject EnemySelectPanel;   
    public GameObject enemyButton;
    private List<GameObject> enemyBtns = new List<GameObject>();
    public Transform Spacer;
    //heroattack
    public GameObject buttonPanel;   
    public Transform buttonPanel_pr;
    public GameObject actionButton_pr;
    public GameObject MagicPanel;
    public GameObject magicButton_pr;
    public Transform magicSpacer;
    public GameObject bf_Button;
    public GameObject fr_Button;
    public GameObject heal_Button;
    public GameObject wild_Button;
    public GameObject InventoryOpen_Button;
    private List<GameObject> atkBtns = new List<GameObject>();
    public List<GameObject> mgcBtns = new List<GameObject>();

    #endregion

    void Awake()
    {
        //敵の配置        
        if (GameManager.instance.enemyAmount == 1)//敵の数が1の場合中央に配置
        {
            GameObject newEnemy = Instantiate(GameManager.instance.enemyToBattle[0], spawnPosints[1].position, Quaternion.Euler(0, 90, 0)) as GameObject;//GameManager参考（敵の数）,位置と角度基本設定
            newEnemy.name = newEnemy.GetComponent<EnemyStateMachine>().enemy.theName+ "_" + 1;
            newEnemy.GetComponent<EnemyStateMachine>().enemy.theName = newEnemy.name;
            EnemysInBattle.Add(newEnemy);
        }
        else//そうじゃない場合順番に配置
        {
            for (int i = 0; i < GameManager.instance.enemyAmount; i++)
            {
                GameObject newEnemy = Instantiate(GameManager.instance.enemyToBattle[i], spawnPosints[i].position, Quaternion.Euler(0, 90, 0)) as GameObject;//GameManager参考（敵の数）,位置と角度基本設定
                newEnemy.name = newEnemy.GetComponent<EnemyStateMachine>().enemy.theName + "_" + i;
                newEnemy.GetComponent<EnemyStateMachine>().enemy.theName = newEnemy.name;
                EnemysInBattle.Add(newEnemy);

            }
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        camClick = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraClick>();
        slideManager = FindObjectOfType<SlideManager>();//skill用のslide
        result = FindObjectOfType<ResultSlideManager>();//勝利の用のslide
        inven = FindObjectOfType<Inventory>();        
        HerosInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));//BattleをするPlayerを追加
        battleStates = PerformAction.WAIT;
        HeroInput = HeroGui.ACTIVATE;//PlayerのProgressBarが100%になるまで待機状態。
        EnemySelectPanel.SetActive(false); //Enemy選択Buttonを含むpanelをOffにする。
        EnemyButton();//Enemy選択Buttonを作る
        enemyClicked = true;


    }

    // Update is called once per frame
    void Update()
    {
        switch (battleStates)
        {
            case (PerformAction.WAIT):

                #region PerformAction WAIT
                //Player,EnemyのProgressBarが100%になったら行動可能なList(PerformListに追加)
                if (PerformList.Count > 0)
                {
                    battleStates = PerformAction.TAKEACTION;
                }
                #endregion

                break;
            case (PerformAction.TAKEACTION):

                #region PerformAction TAKEACTION
                GameObject performer = GameObject.Find(PerformList[0].Attacker);
                //EnemyのTurn
                if (PerformList[0].Type == "Enemy")
                {
                    EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
                    for (int i = 0; i < HerosInBattle.Count; i++)
                    {
                        if (PerformList[0].AttackersTarget == HerosInBattle[i])
                        {
                            ESM.HeroToAttack = PerformList[0].AttackersTarget;
                            ESM.currentState = EnemyStateMachine.TurnsState.ACTION;
                            break;
                        }
                        else
                        {
                            PerformList[0].AttackersTarget = HerosInBattle[Random.Range(0, HerosInBattle.Count)];
                            ESM.HeroToAttack = PerformList[0].AttackersTarget;
                            ESM.currentState = EnemyStateMachine.TurnsState.ACTION;
                        }
                    }
                }
                //PlayerのTurn
                if (PerformList[0].Type == "Hero")
                {
                    HeroStateMachine HSM = performer.GetComponent<HeroStateMachine>();
                    HSM.EnemyToAttack = PerformList[0].AttackersTarget;
                    HSM.KillEnemy = PerformList[0].KillTarget;
                    HSM.currentState = HeroStateMachine.TurnsState.ACTION;
                }
                battleStates = PerformAction.PERFORMACTION;
                #endregion

                break;
            case (PerformAction.PERFORMACTION):

                break;
            case (PerformAction.CHECKALIVE):

                #region PerformAction CHECKALIVE

                if (HerosInBattle.Count < 1)
                {
                    battleStates = PerformAction.LOSE;
                }
                else if (EnemysInBattle.Count < 1)
                {
                    battleStates = PerformAction.WIN;
                }
                else
                {
                    //Playerがaliveの場合panel,buttonをclearさせる。
                    clearAttackPanel();
                    HeroInput = HeroGui.ACTIVATE;
                }

                #endregion

                break;
            case (PerformAction.LOSE):

                #region PerformAction LOSE
                {
                    //敗北の時GameOverを呼ぶ。
                    End_Battle();
                }
                #endregion

                break;
            case (PerformAction.WIN):

                #region PerFormAction WIN
                {
                    //勝利の時Save。
                    battleEnemyIntText.text = "Enemy :  " + battleEnemyInt;
                    battleCoinText.text = "Coin :     " + battleCoin;
                    for (int i = 0; i < HerosInBattle.Count; i++)
                    {
                        HerosInBattle[i].GetComponent<HeroStateMachine>().currentState = HeroStateMachine.TurnsState.WAITING;//いつでも使うため
                        GameManager.instance.hpBattleExit = HerosInBattle[i].GetComponent<HeroStateMachine>().hero.curHP;
                        GameManager.instance.mpBattleExit = HerosInBattle[i].GetComponent<HeroStateMachine>().hero.curMP;
                    }
                    victory = true;
                    if (victoryButton)
                    {
                        //勝利の時Fieldに戻る。
                        SoundManager.instance.StopBGM();
                        GameManager.instance.LoadSceneAfterBattle();
                        GameManager.instance.gameState = GameManager.GameStates.START_STATE;
                        GameManager.instance.enemyToBattle.Clear();
                    }
                }
                #endregion

                break;
        }

        switch (HeroInput)
        {
            case (HeroGui.ACTIVATE):

                #region HeroGui ACTIVATE

                if (HeroToManage.Count > 0)
                {
                    //PlayerのTurn
                    HeroChoise = new HandleTurn();
                    if (heroStun)//playerがstunの時行動不能になり、1Turnスキップ
                    {
                        Input8();
                    }
                    else
                    {
                        SoundManager.instance.PlaySound("Button", 1);
                        createAttackButton();
                        MagicAttackButton();
                        createHealButton();
                        InventoryOpen_Button.SetActive(true);
                        CoolTimeCheck();
                        if (HeroToManage[0].GetComponent<HeroStateMachine>().hero.curMP == 100)
                        {
                            createWildAttackButton();
                        }
                        HeroInput = HeroGui.WAITING;
                    }
                }

                #endregion

                break;
            case (HeroGui.WAITING):

                break;
            case (HeroGui.DONE):

                HeroInputDone();

                break;
        }

    }  

    #region battle行動

    public void CollectActions(HandleTurn input)//Turn開始
    {
        PerformList.Add(input);
    }

    //ボタンで実行されるPlayer input
    public void Input1()//Attack input
    {
        if (slideOn)
        {
            SlideManager.isFinished = false;
            StartCoroutine(slideManager.DisappearSlide());
        }
        SoundManager.instance.PlaySound("Button", 1);
        camClick.playerTurn = true;
        click = true;
        if (!autoAttack)
        {
            autoAttack = true;
            Inventory.inventoryActivated = false;                     
            HeroStateMachine HSM = HeroToManage[0].GetComponent<HeroStateMachine>();
            HeroChoise.Attacker = HeroToManage[0].name;
            HeroChoise.AttackersGameobject = HeroToManage[0];
            HeroChoise.Type = "Hero";
            HeroChoise.choosenAttack = HSM.hero.attacks[0];
            //Input1実行の時のUI設定
            //敵を選択できるbuttonを含むPanelをtrueにする
            EnemySelectPanel.SetActive(true);
            //selector表示をtrueにする
            if (enemyClicked)
            {
                for (int i = 0; i < EnemysInBattle.Count; i++)
                {

                    EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(true);

                    EnemysInBattle[i].transform.Find("EnemyUI").transform.Find("Selector").gameObject.SetActive(true);

                }
            }
        }         
        else
        {
            autoAttack = false;
            if (autoEnemy == null)
            {
                for (int i = 0; i < EnemysInBattle.Count; i++)
                {
                    Input2(EnemysInBattle[i].gameObject);
                    return;
                }
            }
            else
            {
                Input2(autoEnemy);
            }
            
        }
    }

    public void Input2(GameObject choosenEnemy)//敵を選択
    {
        SoundManager.instance.PlaySound("Button", 1);

        click = false;
        autoAttack = false;
        autoEnemy = choosenEnemy;

        HeroStateMachine HSM = HeroToManage[0].GetComponent<HeroStateMachine>();
        HeroChoise.AttackersTarget = choosenEnemy;
        HSM.battleParticle[0].Stop();
        HeroInput = HeroGui.DONE;
    }

    public void Input3()//magicを設定する
    {
        SoundManager.instance.PlaySound("SkillOpen", 2);

        if (!slideOn)
        {
            SlideManager.isFinished = false;
            StartCoroutine(slideManager.AppearSlide());
        }
        else
        {
            SlideManager.isFinished = false;
            StartCoroutine(slideManager.DisappearSlide());
        }
        
        camClick.playerTurn = true;　//touchの判定CameraClickからclickの許可を持ってくる(playerTurnで攻撃のbuttonの時だclick可能にする)
        click = false;
        autoAttack = false;　
        Inventory.inventoryActivated = false;
                
        HeroStateMachine HSM = HeroToManage[0].GetComponent<HeroStateMachine>();
        for (int i = 0; i < EnemysInBattle.Count; i++)
        {
            EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(false);
            EnemysInBattle[i].transform.Find("EnemyUI").transform.Find("Selector").gameObject.SetActive(false);
        }

    }

    public void Input4(BaseAttacks choosenmagic)//magic,buffで実行されるinput
    {
        SoundManager.instance.PlaySound("Button", 1);

        autoAttack = false;

        Inventory.inventoryActivated = false;       
        HeroStateMachine HSM = HeroToManage[0].GetComponent<HeroStateMachine>();
        HeroChoise.Attacker = HeroToManage[0].name;
        HeroChoise.AttackersGameobject = HeroToManage[0];
        HeroChoise.Type = "Hero";
        HeroChoise.choosenAttack = choosenmagic;
        if (choosenmagic.attackName == "Buff")
        {
            enemyBarStop = true;
            HSM.battleParticle[0].Stop();
            HeroInput = HeroGui.DONE;
        }
        else
        {
            click = true;
            EnemySelectPanel.SetActive(true);
            if (enemyClicked)
            {
                for (int i = 0; i < EnemysInBattle.Count; i++)
                {
                    EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(true);
                    EnemysInBattle[i].transform.Find("EnemyUI").transform.Find("Selector").gameObject.SetActive(true);
                }
            }
        }
    }

    public void Input5(List<GameObject> killChoosenEnemys)//敵を選択
    {
        SoundManager.instance.PlaySound("Button", 1);
        camClick.playerTurn = true;
        click = false;
        autoAttack = false;
        enemyBarStop = true;        
        Inventory.inventoryActivated = false;
        
        HeroStateMachine HSM = HeroToManage[0].GetComponent<HeroStateMachine>();
        HeroChoise.Attacker = HeroToManage[0].name;
        HeroChoise.AttackersGameobject = HeroToManage[0];
        HeroChoise.Type = "Hero";
        HeroChoise.KillTarget = killChoosenEnemys;
        HeroChoise.choosenAttack = HSM.hero.WildAttack[0];
        HSM.battleParticle[0].Stop();

        HeroInput = HeroGui.DONE;

    }

    public void Input6()//healbuttonで実行されるinput
    {
        SoundManager.instance.PlaySound("Button", 1);

        click = false;
        autoAttack = false;
        enemyBarStop = true;        
        Inventory.inventoryActivated = false;

        HeroStateMachine HSM = HeroToManage[0].GetComponent<HeroStateMachine>();
        HeroChoise.Attacker = HeroToManage[0].name;
        HeroChoise.AttackersGameobject = HeroToManage[0];
        HeroChoise.Type = "Hero";
        HeroChoise.choosenAttack = HSM.hero.Heal[0];
        HSM.battleParticle[0].Stop();

        HeroInput = HeroGui.DONE;

    }

    public void Input7()//potionbuttonで実行されるinput
    {
        SoundManager.instance.PlaySound("Button", 1);

        click = false;
        autoAttack = false;
        enemyBarStop = true;       
        Inventory.inventoryActivated = false;
        inven.titleButton.SetActive(false);
        inven.endButton.SetActive(false);
        
        HeroStateMachine HSM = HeroToManage[0].GetComponent<HeroStateMachine>();
        HeroChoise.Attacker = HeroToManage[0].name;
        HeroChoise.AttackersGameobject = HeroToManage[0];
        HeroChoise.Type = "Hero";
        HeroChoise.choosenAttack = HSM.hero.attacks[1];
        HSM.battleParticle[0].Stop();

        HeroInput = HeroGui.DONE;
    }

    public void Input8()//playerがstunの時で実行されるinput
    {
        click = false;
        autoAttack = false;
        enemyBarStop = true;        
        Inventory.inventoryActivated = false;

        HeroStateMachine HSM = HeroToManage[0].GetComponent<HeroStateMachine>();
        HeroChoise.Attacker = HeroToManage[0].name;
        HeroChoise.AttackersGameobject = HeroToManage[0];
        HeroChoise.Type = "Hero";
        HeroChoise.choosenAttack = HSM.hero.attacks[2];
        HSM.battleParticle[0].Stop();

        HeroInput = HeroGui.DONE;
    }

    #endregion

    #region Turnの処理(Turnが終った時reset

    public void CoolTimeCheck()//healのcooltimecheck
    {
        var heroCoolTime = HeroToManage[0].GetComponent<HeroStateMachine>();
        if (heroCoolTime.coolTimeStart)
        {
            heroCoolTime.coolDown++;
        }
        if (heroCoolTime.coolDown == heroCoolTime.maxcoolDown)
        {
            heroCoolTime.coolDown = 0;
            heroCoolTime.coolTimeStart = false;
        }
    }

    void HeroInputDone()
    {

        if (slideOn)
        {
            SlideManager.isFinished = false;
            StartCoroutine(slideManager.DisappearSlide());
        }
        PerformList.Add(HeroChoise);
        clearAttackPanel();
        InventoryOpen_Button.SetActive(false);
        HeroToManage.RemoveAt(0);

        HeroInput = HeroGui.ACTIVATE;
    }

    public void OffAttackPanel()
    {
        foreach (GameObject atkBtn in atkBtns)
        {
            atkBtn.SetActive(false);
        }
    }

    public void OnAttackPanel()
    {
        foreach (GameObject atkBtn in atkBtns)
        {
            atkBtn.SetActive(true);
        }
    }

    void clearAttackPanel()
    {
        EnemySelectPanel.SetActive(false);
        foreach (GameObject atkBtn in atkBtns)
        {
            Destroy(atkBtn);
        }
        atkBtns.Clear();
        //attackPanelをclearした後skillもclear
        StartCoroutine(clearMagicButton());
    }

    IEnumerator clearMagicButton()
    {
        yield return new WaitUntil(() => !slideManager.slideOnOff);
        foreach (GameObject mgcBtn in mgcBtns)
        {
            Destroy(mgcBtn);
        }
        mgcBtns.Clear();
    }

    #endregion

    #region 行動button生成

    //button 生成
    void createAttackButton()
    {
        //Attack->Input1
        GameObject AttackButton = Instantiate(actionButton_pr) as GameObject;
        AttackButton.GetComponent<Button>().onClick.AddListener(() => Input1());//クリックでinput1メソッド開始
        AttackButton.transform.SetParent(buttonPanel_pr, false);
        atkBtns.Add(AttackButton);
    }

    void MagicAttackButton()
    {
        //Skill->Input3
        GameObject MagicAttackButton = Instantiate(magicButton_pr) as GameObject;
        MagicAttackButton.GetComponent<Button>().onClick.AddListener(() => Input3());//クリックでinput3メソッド開始
        MagicAttackButton.transform.SetParent(buttonPanel_pr, false);
        atkBtns.Add(MagicAttackButton);

        if (HeroToManage[0].GetComponent<HeroStateMachine>().hero.MagicAttacks.Count > 0)
        {
            foreach (BaseAttacks magicAtck in HeroToManage[0].GetComponent<HeroStateMachine>().hero.MagicAttacks)
            {
                if (magicAtck.attackName == "Buff")
                {
                    //Skill->Buff->Input4
                    GameObject MagicButton = Instantiate(bf_Button) as GameObject;
                    Text MagicButtonText = MagicButton.transform.Find("Text").gameObject.GetComponent<Text>();
                    MagicButtonText.text = magicAtck.attackName;
                    AttackButton ATB = MagicButton.GetComponent<AttackButton>();
                    ATB.magicAttacktoPerform = magicAtck;
                    MagicButton.transform.SetParent(magicSpacer, false);
                    mgcBtns.Add(MagicButton);
                    if (HeroToManage[0].GetComponent<HeroStateMachine>().buffOn == 0)
                    {
                        MagicButton.GetComponent<Button>().interactable = false;
                    }

                }
                if (magicAtck.attackName == "Fire")
                {
                    //Skill->Stun->Input4
                    GameObject MagicButton = Instantiate(fr_Button) as GameObject;
                    Text MagicButtonText = MagicButton.transform.Find("Text").gameObject.GetComponent<Text>();
                    MagicButtonText.text = magicAtck.attackName;
                    AttackButton ATB = MagicButton.GetComponent<AttackButton>();
                    ATB.magicAttacktoPerform = magicAtck;
                    MagicButton.transform.SetParent(magicSpacer, false);
                    mgcBtns.Add(MagicButton);
                }
            }

        }
        else
        {
            MagicAttackButton.GetComponent<Button>().interactable = false;
        }
    }

    void createHealButton()
    {
        //Heal->Input6
        GameObject HealButton = Instantiate(heal_Button) as GameObject;
        HealButton.GetComponent<Button>().onClick.AddListener(() => Input6());//クリックでinput6メソッド開始
        HealButton.transform.SetParent(buttonPanel_pr, false);
        Image HealButtonImage = HealButton.transform.Find("CoolDown").gameObject.GetComponent<Image>();
        HealButtonImage.fillAmount = HeroToManage[0].GetComponent<HeroStateMachine>().coolDown / HeroToManage[0].GetComponent<HeroStateMachine>().maxcoolDown;
        if (HeroToManage[0].GetComponent<HeroStateMachine>().coolDown == 0)
        {
            HealButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            HealButton.GetComponent<Button>().interactable = false;
        }
        atkBtns.Add(HealButton);
    }

    void createWildAttackButton()
    {
        //全体攻撃->Input5
        GameObject WildButton = Instantiate(wild_Button) as GameObject;
        Text WildButtonText = WildButton.transform.Find("Text").gameObject.GetComponent<Text>();
        WildButtonText.text = "Wild";
        WildButton.GetComponent<Button>().onClick.AddListener(() => Input5(EnemysInBattle));//クリックでinput5メソッド開始
        WildButton.transform.SetParent(buttonPanel_pr, false);
        atkBtns.Add(WildButton);
    }

    #endregion

    #region その他の処理

    public void BattleCoinAdd(int coinToAdd, int battleEnemyToAdd)//倒したEnemyからCoinを貰う。
    {
        battleCoin += coinToAdd;
        battleEnemyInt += battleEnemyToAdd;
    }

    private void End_Battle()//Loseの時GameOverを実行させる。
    {        
        Inventory.blackActivated = true;
    }

    public void ResultUIAppear()
    {
        ResultSlideManager.resultFinished = false;
        StartCoroutine(result.BattleResultAppear());
    }

    public void NextSceneButton()
    {
        victoryButton = true;
        GameManager.instance.AddCoin(battleCoin);
    }

    public void EnemyButton()
    {
        //cleanup
        foreach (GameObject enemyBtn in enemyBtns)
        {
            Destroy(enemyBtn);
        }
        enemyBtns.Clear();
        //create buttons
        foreach (GameObject enemy in EnemysInBattle) //enemysinbattleの中のenemyprefabsを呼ぶ
        {
            GameObject newButton = Instantiate(enemyButton) as GameObject;
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();
            EnemyStateMachine cur_enemy = enemy.GetComponent<EnemyStateMachine>();
            Text buttonText = newButton.transform.Find("Text").gameObject.GetComponent<Text>();
            buttonText.text = cur_enemy.enemy.theName;
            button.EnemyPrefab = enemy;
            newButton.transform.SetParent(Spacer, false);
            enemyBtns.Add(newButton);
        }
    }

    #endregion
}
