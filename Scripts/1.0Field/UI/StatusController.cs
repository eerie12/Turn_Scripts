using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StatusController : MonoBehaviour
{
    HealthManager HM;
    PlayerControllerrbody playerMovement;
    CreatePlayer PlayerStatus;

    //scene移動用のname
    [SerializeField] string sceneName;

    #region health
    //スタミナマイナス
    public bool spUsed;
    public bool spCheck;
    public bool spCharge;
    public bool playerTired;
    public GameObject hpBar;
    public GameObject spBar;

    //スタミナ回復量
    [SerializeField] private int spIncreaseSpeed;

    //スタミナ再回復ディレイ
    [SerializeField] private int maxSpRechargeTime;
    private int currentSpReachargeTime;

    #endregion

    #region UI

    //uiイメージ
    [SerializeField] private Image[] images_Gauge;
    private const int HP = 0, SP = 1;
    [SerializeField] private Text hpText;
    [SerializeField] private Text spText;
    //menu
    public GameObject pasueMenu;
    public GameObject statusUI;
    public GameObject helpUI;

    #endregion

    #region quest
    //QuestObject
    public GameObject questObject;
    public string questquestObjectTag;
    [SerializeField] GameObject[] questButtons;
    [SerializeField] GameObject questMark_Status;

    #endregion

    

    // Start is called before the first frame update
    void Start()
    {
        HM = FindObjectOfType<HealthManager>();
        PlayerStatus = FindObjectOfType<CreatePlayer>();
        playerMovement = FindObjectOfType<PlayerControllerrbody>();
    }


    // Update is called once per frame
    void Update()
    {
        SpReChargeTime();
        SpRecover();
        GaugeUpdatte();

    }

    #region health処理

    private void SpReChargeTime()
    {
        if (spUsed)
        {
            if (currentSpReachargeTime < maxSpRechargeTime)
                currentSpReachargeTime++;
            else
                spUsed = false;

        }
    }

    private void SpRecover()
    {
        if (!spUsed && HM.currentSp < HM.maxSp)
        {
            spCharge = true;
            HM.currentSp += spIncreaseSpeed;
        }
        else
        {
            spCharge = false;

        }
    }
    private void GaugeUpdatte()
    {
        images_Gauge[HP].fillAmount =(float)HM.currentHp / HM.maxHp;
        hpText.text = HM.currentHp.ToString() + '/' + HM.maxHp.ToString();
        images_Gauge[SP].fillAmount = (float)HM.currentSp / HM.maxSp;
        spText.text = HM.currentSp.ToString();
    }


    public void DecreaseStamina(int _count)
    {
        spUsed = true;
        currentSpReachargeTime = 0;

        if (HM.currentSp - _count > 0)
            HM.currentSp -= _count;
        else
        {
            HM.currentSp = 0;

        }
            
    }

    public int GetCurrentSP()
    {
        return HM.currentSp;
    }

    #endregion

    #region UI
    public void PauseMenu()
    {
        SoundManager.instance.PlaySound("Button", 1);
        HM.isFadeToDialogueBackground = true;
        playerMovement.SettingUI(false);

        if (questObject)
        {
            questObject.tag = "QuestObjectOFF";
        }


        playerMovement.quest_Button.SetActive(false);
        playerMovement.objectMessage.SetActive(false) ;
        playerMovement.boxMessage.SetActive(false);
        playerMovement.status_Button.SetActive(false);
        pasueMenu.SetActive(true);
        
    }

    public void PauseOff()
    {
        SoundManager.instance.PlaySound("Button", 1);
        HM.isFadeFromBlack = true;
        playerMovement.SettingUI(true);
        pasueMenu.SetActive(false);
        if (questObject)
        {
            questObject.tag = questquestObjectTag;
        }
    }

    public void OpenPanel()
    {
        SoundManager.instance.PlaySound("Button", 1);
        HM.isFadeToDialogueBackground = true;
        playerMovement.SettingUI(false);

        if (questObject)
        {
            questObject.tag = "QuestObjectOFF";
        }


        playerMovement.quest_Button.SetActive(false);
        playerMovement.objectMessage.SetActive(false);
        playerMovement.boxMessage.SetActive(false);
        playerMovement.status_Button.SetActive(false);
        QuestUIManager.uiManager.questLogPanelActive = !QuestUIManager.uiManager.questLogPanelActive;
        QuestUIManager.uiManager.ShowQuestLogPanel();

    }


    public void ClosePanel()
    {
        SoundManager.instance.PlaySound("Button", 1);
        HM.isFadeFromBlack = true;

        playerMovement.SettingUI(true);
        if (questObject)
        {
            questObject.tag = questquestObjectTag;
        }

        QuestUIManager.uiManager.HideQuestLogPanel();
    }

    public void OpenStatusPanel()
    {
        SoundManager.instance.PlaySound("Button", 1);
        if (questMark_Status)
        {
            questMark_Status.SetActive(false);
            GameManager.instance.fieldMonName.Add(questMark_Status.name);
        }
        HM.isFadeToDialogueBackground = true;

        playerMovement.SettingUI(false);

        if (questObject)
        {
            questObject.tag = "QuestObjectOFF";
        }
        playerMovement.status_Button.SetActive(false);
        statusUI.SetActive(true);
        PlayerStatus.SetHeroClass();

    }

    public void CloseStatusPanel()
    {
        SoundManager.instance.PlaySound("Button", 1);
        HM.isFadeFromBlack = true;

        playerMovement.SettingUI(true);

        if (questObject)
        {
            questObject.tag = questquestObjectTag;
        }
        statusUI.SetActive(false);

    }

    public void OpenHelpPanel()
    {

        SoundManager.instance.PlaySound("Button", 1);
        HM.isFadeToDialogueBackground = true;
        playerMovement.SettingUI(false);
        if (questObject)
        {
            questObject.tag = "QuestObjectOFF";
        }
        playerMovement.quest_Button.SetActive(false);
        playerMovement.objectMessage.SetActive(false);
        playerMovement.boxMessage.SetActive(false);
        playerMovement.status_Button.SetActive(false);
        helpUI.SetActive(true);



    }

    public void CloseHelpPanel()
    {
        SoundManager.instance.PlaySound("Button", 1);
        HM.isFadeFromBlack = true;
        helpUI.SetActive(false);

        if (questObject)
        {
            questObject.tag = questquestObjectTag;
        }
        playerMovement.SettingUI(true);

    }

    private void PlayerTired()
    {
        if (HM.currentSp == 0)
        {
            playerTired = true;
        }
        else if (HM.currentSp == 100)
        {
            playerTired = false;
        }
    }

    public void ClickToTitle()
    {
        SoundManager.instance.PlaySound("Button", 1);

        Destroy(GameObject.Find("GameManager"));
        Destroy(GameObject.Find("QuestManager"));
        SceneManager.LoadScene(sceneName);
    }

    public void ClickToExit()
    {
        SoundManager.instance.PlaySound("Button", 1);
        Destroy(GameObject.Find("GameManager"));
        Destroy(GameObject.Find("QuestManager"));
        Application.Quit();
    }

    #endregion

    #region quest

    public void QuestButtonON()
    {
        SoundManager.instance.PlaySound("Button", 1);
        if (questObject)
        {
            questObject.tag = "QuestObjectOFF";
        }

        playerMovement.quest_Button.SetActive(false);
        HM.isFadeToDialogueBackground = true;
        playerMovement.SettingUI(false);


        if (!QuestUIManager.uiManager.questPanelActive)
        {
            QuestObject QuestObj = questObject.GetComponent<QuestObject>();

            QuestUIManager.uiManager.CheckQuests(QuestObj);
        }
    }

    public void QuestButtonOff()
    {
        SoundManager.instance.PlaySound("Button", 1);
        HM.isFadeFromBlack = true;
        playerMovement.SettingUI(true);
        QuestUIManager.uiManager.HideQuestPanel();
        if (questObject)
        {
            questObject.tag = questquestObjectTag;
        }
    }

    
    public void AcceptQuest()
    {
        SoundManager.instance.PlaySound("Button", 1);
        HM.isFadeFromBlack = true;
        playerMovement.SettingUI(true);
        int questID = questButtons[0].GetComponent<qButtonScript>().questID;
        QuestManager.questManager.AcceptQuest(questID);
        QuestUIManager.uiManager.HideQuestPanel();
        if (questObject)
        {
            questObject.tag = questquestObjectTag;
        }
        

        //Update All Npc 
        QuestObject[] currentQuestGuys = FindObjectsOfType(typeof(QuestObject)) as QuestObject[];
        foreach (QuestObject obj in currentQuestGuys)
        {
            obj.SetQuestMaker();
        }

    }

    public void GiveUpQuest()
    {
        SoundManager.instance.PlaySound("Button", 1);
        HM.isFadeFromBlack = true;
        playerMovement.SettingUI(true);
        int questID = questButtons[1].GetComponent<qButtonScript>().questID;
        QuestManager.questManager.GIveUpQuest(questID);
        QuestUIManager.uiManager.HideQuestPanel();
        if (questObject)
        {
            questObject.tag = questquestObjectTag;
        }
       

        //Update All Npc 
        QuestObject[] currentQuestGuys = FindObjectsOfType(typeof(QuestObject)) as QuestObject[];
        foreach (QuestObject obj in currentQuestGuys)
        {
            obj.SetQuestMaker();
        }

    }

    public void CompleteQuest()
    {
        SoundManager.instance.PlaySound("Button", 1);
        HM.isFadeFromBlack = true;
        playerMovement.SettingUI(true);
        int questID = questButtons[2].GetComponent<qButtonScript>().questID;
        QuestManager.questManager.CompleteQuest(questID);
        questButtons[2].SetActive(false);
        QuestUIManager.uiManager.HideQuestPanel();
        if (questObject)
        {
            questObject.tag = questquestObjectTag;
        }
        //Update All Npc 
        QuestObject[] currentQuestGuys = FindObjectsOfType(typeof(QuestObject)) as QuestObject[];
        foreach (QuestObject obj in currentQuestGuys)
        {
            obj.SetQuestMaker();
        }



    }

    #endregion

    
    


}
