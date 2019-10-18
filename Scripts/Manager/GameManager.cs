using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;   
    public GameObject playerCharacter;

    public enum GameStates
    {
        START_STATE,
        WORLD_STATE,     
        BATTLE_STATE,
        IDLE 

    }
    public GameStates gameState;

    #region Scene,Event変数

    //SCENES
    public string sceneToLoad;
    public string lastScene;
    LoadingSlideManager loading;
    public bool loadingStarted = false;

    //startEvent
    public bool eventStartCheck;
    public bool eventFinish = false;


    #endregion

    #region Battle変数

    //status更新
    public string PlayerName { get; set; }
    public BaseClass PlayerClass { get; set; }
    public float BaseATK { get; set; }
    public float BaseDEF { get; set; }
    public float BaseHeal { get; set; }

    //hp,mpの伝達
    private PlayerControllerrbody playerMovement;
    private HealthManager HM;
    private HeroStateMachine HSM;
    private BattleStateMachine BSM;
    public float hpBattleStart;
    public float mpBattleStart;
    public float hpBattleExit = 200;
    public float mpBattleExit;

    public Vector3 nextPlayerPosition;//battleから戻る時の位置  
    public Vector3 lastPlayerPosition;//nextPlayerPositionに伝達

    //advancedBattle
    public bool advancedBattle = false;

    //Coin
    public int currentCoin = 0;

    //他の設定
    public int enemyAmount;
    public int currentGold;
    public Text goldText;
    public List<GameObject> enemyToBattle = new List<GameObject>();
    public List<string> fieldMonName = new List<string>();
    public Region curRegions;
    public bool isWalking = false;
    public bool canGetEncounter = false;
    public bool Attacked = false;
    
    #endregion

    #region 会話設定変数(csv)
    //csvをDicに追加
    [SerializeField] string csv_FileName;
    Dictionary<int, Dialogue> dialogueDic = new Dictionary<int, Dialogue>();
    public bool[] eventFlags = new bool[3];
    public int eventCheck;
    public static bool isFinish = false;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        Screen.SetResolution(1280, 720, true);//画面設定
        //instancecheck
        if (instance == null)
        {
            //if not set the instance to this
            instance = this;
            DialogueParser theParser = GetComponent<DialogueParser>();
            Dialogue[] dialouges = theParser.Parse(csv_FileName);
            for (int i = 0; i < dialouges.Length; i++)
            {
                dialogueDic.Add(i + 1, dialouges[i]);
            }
            isFinish = true;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }      
        DontDestroyOnLoad(gameObject);
        if (!GameObject.Find("PlayerCharacter"))
        {
            GameObject Player = Instantiate(playerCharacter, nextPlayerPosition, Quaternion.identity) as GameObject;
            Player.name = "PlayerCharacter";
        }    
    }

    void Start()
    {
        eventStartCheck = false;//MainQuestのClear判定

    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case (GameStates.START_STATE):

                #region GameStates.START_STATE

                StartCoroutine(LoadingFinished("Town", 0));
                if (!LoadingSlideManager.loadingOnOff)
                gameState = GameStates.WORLD_STATE;

                #endregion

                break;

            case (GameStates.WORLD_STATE):

                #region GameStates.WORLD_STATE

                //FieldEnemyに触れた時
                Invoke("RandomEncounter",0.1f);
                //PlayerUI設定後
                if (Attacked)
                {
                    HM = FindObjectOfType<HealthManager>();
                    HM.isFadeToDialogueBackground = true;
                    StartCoroutine(Loading());
                    if(LoadingSlideManager.loadingOnOff)
                    gameState = GameStates.BATTLE_STATE;
                }

                #endregion

                break;

            case (GameStates.BATTLE_STATE):

                #region GameStates.BATTLE_STATE

                //battlessceneに移動               
                StartBattle();
                gameState = GameStates.IDLE;

                #endregion

                break;
            case (GameStates.IDLE):

                break;
        }
    }

    

    #region Loading設定

    IEnumerator Loading()
    {
        if (loadingStarted)
        {
            yield break;
        }

        loadingStarted = true;

        if (loading != null)
        loading = null;

        loading = FindObjectOfType<LoadingSlideManager>();
        LoadingSlideManager.loadingFinished = false;
        StartCoroutine(loading.LoadingAppearSlide());

        yield return new WaitUntil(() => LoadingSlideManager.loadingFinished);
        loadingStarted = false;

    }

    //(Bgmとマップの種類を含む)Scene移動用のCoroutine
    IEnumerator LoadingFinished(string bgmName,int number)
    {
        if (loadingStarted)
        {
            yield break;
        }

        loadingStarted = true;

        if (loading != null)
        loading = null;
      
        yield return new WaitForSeconds(0.01f);
        
        loading = FindObjectOfType<LoadingSlideManager>();
        Attacked = false;
        canGetEncounter = false;
        if (SceneManager.GetActiveScene().name == "town(pr)")
        {
            if (fieldMonName != null)
            {
                for (int i = 0; i < fieldMonName.Count; i++)
                {
                    
                    Destroy(GameObject.Find(fieldMonName[i]));

                    //モンスター退治QuestのAccept確認
                    if (QuestManager.questManager.currentQuestList.Count > 0)
                    {
                        for (int j = 0; j < QuestManager.questManager.currentQuestList.Count; j++)
                        {
                            if (QuestManager.questManager.currentQuestList[j].id == 2)
                            {
                                QuestManager.questManager.AddQuestItem("Monster", 1);
                                QuestObject[] currentQuestGuys = FindObjectsOfType(typeof(QuestObject)) as QuestObject[];
                                foreach (QuestObject obj in currentQuestGuys)
                                {
                                    obj.SetQuestMaker();
                                }
                            }
                        }
                    }
                }
                
            }

            GameObject coinText = GameObject.Find("coinText");
            Text conText_text = coinText.GetComponent<Text>();
            conText_text.text = currentCoin.ToString();

        }

        yield return new WaitForSeconds(0.05f);
        LoadingSlideManager.loadingFinished = false;
        StartCoroutine(loading.LoadingDisappearSlide(bgmName,number));
        yield return new WaitUntil(() => !LoadingSlideManager.loadingFinished);

        loadingStarted = false;

    }

    public void LoadNextScene()
    {       
        SceneManager.LoadScene(sceneToLoad);      
    }

    public void LoadSceneAfterBattle()
    {
        StartCoroutine(Loading());
        if (LoadingSlideManager.loadingOnOff)
        SceneManager.LoadScene(lastScene);
    }

    #endregion

    #region Battle設定

    void RandomEncounter()
    {
        if (canGetEncounter)
        {
            playerMovement = FindObjectOfType<PlayerControllerrbody>();
            playerMovement.SettingUI(false);
            Attacked = true;
        }
    }
    
    void StartBattle()
    {
        //battleに移動

        //敵の数
        enemyAmount = Random.Range(1, curRegions.maxAmountEnemys+1);
        if(curRegions.tag == "FieldEnemyB")//特定FieldEnemyだけ必ずBattleのEnemyを三匹出現させる。
        {
            enemyAmount = 3;
        }
        for(int i = 0; i< enemyAmount;i++)//ランダムで設定された数分設置
        {
            enemyToBattle.Add(curRegions.possibleEnemmys[Random.Range(0, curRegions.possibleEnemmys.Count)]);

        }
        lastPlayerPosition = GameObject.Find("PlayerCharacter").gameObject.transform.position;//playerpositionをセーブ     
        nextPlayerPosition = lastPlayerPosition;//battleから戻る時の位置

        //hp,mpの伝達
        HM = FindObjectOfType<HealthManager>();
        hpBattleStart = HM.currentHp;
        mpBattleStart = HM.currentMp;
        
        //現在のシーンをセーブ
        lastScene = SceneManager.GetActiveScene().name;        
        SceneManager.LoadScene(curRegions.battleScene);

        //マップによるBGM設定
        if(curRegions.battleScene == "Battle")
        {
            StartCoroutine(LoadingFinished("Dungeon1", 0));//通常BattleScencのBGMOn
        }
        else
        {
            StartCoroutine(LoadingFinished("Dungeon2", 0));//BossBattleScencのBGMOn
        }     
        
        //playerの状態をreset
        Attacked = false;
        canGetEncounter = false;
    }

    #endregion

    #region Dialogue設定
    public Dialogue[] GetDialogue(int _StartNum, int _EndNum)
    {
        List<Dialogue> dialogueList = new List<Dialogue>();

        for (int i = 0; i <= _EndNum - _StartNum; i++)
        {
            dialogueList.Add(dialogueDic[_StartNum + i]);
        }
        return dialogueList.ToArray();
    }
    #endregion

    #region その他の設定

    //Battleで獲得したCoinをFieldのCoinに追加
    public void AddCoin(int coinToAdd)
    {
        currentCoin += coinToAdd;
        GameObject coinText = GameObject.Find("coinText");

    }
    //Fieldで獲得したCoinをFieldのCoinに追加
    public void AddFieldCoin(int coinToAdd)
    {
        currentCoin += coinToAdd;
        GameObject coinText = GameObject.Find("coinText");
        Text conText_text = coinText.GetComponent<Text>();
        conText_text.text = currentCoin.ToString();
    }

    #endregion

    
}
