using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QuestUIManager : MonoBehaviour
{
    public static QuestUIManager uiManager;

    #region questUI

    //BOOLS
    public bool questAvailable = false;
    public bool questRunning = false;
    public bool questPanelActive = false;
    public bool questLogPanelActive = false;

    //PANELS
    public GameObject questPanel;
    public GameObject questLogPanel;

    //QuestObject
    private QuestObject currentQuestObject;

    //QuestLists
    public List<Quest> availableQuests = new List<Quest>();
    public List<Quest> activeQuests = new List<Quest>();

    //Buttons
    public GameObject qButton;
    public GameObject qLogButton;
    private List<GameObject> qButtons = new List<GameObject>();

    private GameObject acceptButton;
    private GameObject giveUpButton;
    private GameObject completeButton;

    //Spacer
    public Transform qButtonSpacer1;
    public Transform qButtonSpacer2;
    public Transform qLogButtonSpacer;

    //Quest Info
    public Text questTitle;
    public Text questDescription;
    public Text questSummary;
    public Text questReward;

    //Quest Log Info
    public Text questLogTitle;
    public Text questLogDescription;
    public Text questLogSummary;
    public Text questLogReward;

    public Transform[] QuestTransformDic;

    #endregion

    private void Awake()
    {
        if(uiManager == null)
        {
            uiManager = this;
        }
        else if(uiManager != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        HideQuestPanel();
        HideQuestLogPanel();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            questLogPanelActive = !questLogPanelActive;
            ShowQuestLogPanel();
        }
    }

    public void CheckQuests(QuestObject questObject)
    {
        currentQuestObject = questObject;
        QuestManager.questManager.QuestRequest(questObject);
        if((questRunning || questAvailable) && !questLogPanelActive)
        {
            Invoke("ShowQuestPanel", 0.05f);
        }
        else
        {
            questPanelActive = true;
            questPanel.SetActive(questPanelActive);
            
        }

    }

    #region show Panel
    //show Panel

    public void ShowQuestPanel()
    {
        questPanelActive = true;
        questPanel.SetActive(questPanelActive);
        FillQuestButtons();
    }

    public void ShowQuestLogPanel()
    {
        questLogPanel.SetActive(questLogPanelActive);
        if(questLogPanelActive && !questPanelActive)
        {
            foreach (Quest curQuest in QuestManager.questManager.currentQuestList)
            {
                GameObject questButton = Instantiate(qLogButton);
                QLogButtonScript qbutton = questButton.GetComponent<QLogButtonScript>();

                qbutton.questID = curQuest.id;
                qbutton.questTitle.text = curQuest.title;

                questButton.transform.SetParent(qLogButtonSpacer, false);
                qButtons.Add(questButton);
            }
        }
        else if (!questLogPanelActive && !questPanelActive)
        {
            HideQuestLogPanel();
        }
    }

    public void ShowQuestLog(Quest activeQuest)
    {
        questLogTitle.text = activeQuest.title;
        if(activeQuest.progress == Quest.QuestProgress.ACCEPTED)
        {
            questLogDescription.text = activeQuest.hint;
            questLogSummary.text = activeQuest.questObjective + ":" + activeQuest.questObjectiveCount + " / " + activeQuest.questObjectiveRequirement;
            questLogReward.text = activeQuest.rewardObjective + ":" + activeQuest.goldReward;
        }
        else if(activeQuest.progress == Quest.QuestProgress.COMPLETE)
        {
            questLogDescription.text = activeQuest.congratulation;
            questLogSummary.text = activeQuest.questObjective + ":" + activeQuest.questObjectiveCount + " / " + activeQuest.questObjectiveRequirement;
            questLogReward.text = activeQuest.rewardObjective + ":" + activeQuest.goldReward;
        }

    }

    #endregion

    #region Hide Panel
    //Hide Quest Panel
    public void HideQuestPanel()
    {
        questPanelActive = false;
        questAvailable = false;
        questRunning = false;

        //text clear
        questTitle.text = "";
        questDescription.text = "";
        questSummary.text = "";
        questReward.text = "";

        //Clear Lists
        availableQuests.Clear();
        activeQuests.Clear();
        for(int i = 0; i< qButtons.Count; i++)
        {
            Destroy(qButtons[i]);
        }
        qButtons.Clear();
        //Hide Panel
        questPanel.SetActive(questPanelActive);

    }

    //Hide Quest Log Panel
    public void HideQuestLogPanel()
    {
        questLogPanelActive = false;

        questLogTitle.text = "";
        questLogDescription.text = "";
        questLogSummary.text = "";
        questLogReward.text = "";

        //Clear Button List 
        for(int i = 0; i< qButtons.Count; i++)
        {
            Destroy(qButtons[i]);
        }
        qButtons.Clear();
        questLogPanel.SetActive(questLogPanelActive);
    }

    #endregion

    //fill buttons for quest panel
    void FillQuestButtons()
    {
        foreach(Quest availableQuest in availableQuests)
        {
            GameObject questButton = Instantiate(qButton);
            qButtonScript qBScript = questButton.GetComponent<qButtonScript>();

            qBScript.questID = availableQuest.id;
            qBScript.questTitle.text = availableQuest.title;

            questButton.transform.SetParent(qButtonSpacer1, false);
            qButtons.Add(questButton);
        }
        foreach (Quest activeQuest in QuestManager.questManager.currentQuestList)
        {
            GameObject questButton = Instantiate(qButton);
            qButtonScript qBScript = questButton.GetComponent<qButtonScript>();

            qBScript.questID = activeQuest.id;
            qBScript.questTitle.text = activeQuest.title;

            questButton.transform.SetParent(qButtonSpacer2, false);
            qButtons.Add(questButton);
        }
    }

    //Show quest on button press in Questpanel
    public void showSelectedQuest(int questID)
    {
        for(int i = 0; i < availableQuests.Count; i++)
        {
            if(availableQuests[i].id == questID)
            {
                questTitle.text = availableQuests[i].title;
                if(availableQuests[i].progress == Quest.QuestProgress.AVAILABLE)
                {
                    questDescription.text = availableQuests[i].description;
                    questSummary.text = availableQuests[i].questObjective + " : " + availableQuests[i].questObjectiveCount + " / " + availableQuests[i].questObjectiveRequirement;
                    questReward.text = availableQuests[i].rewardObjective + " : " + availableQuests[i].goldReward;

                }
            }
        }
        for(int i = 0;i< QuestManager.questManager.currentQuestList.Count; i++)
        {
            if (QuestManager.questManager.currentQuestList[i].id == questID)
            {
                questTitle.text = QuestManager.questManager.currentQuestList[i].title;
                if(QuestManager.questManager.currentQuestList[i].progress == Quest.QuestProgress.ACCEPTED)
                {
                    questDescription.text = QuestManager.questManager.currentQuestList[i].hint;
                    questSummary.text = QuestManager.questManager.currentQuestList[i].questObjective + " :" + QuestManager.questManager.currentQuestList[i].questObjectiveCount + " / " + QuestManager.questManager.currentQuestList[i].questObjectiveRequirement;
                    questReward.text = QuestManager.questManager.currentQuestList[i].rewardObjective + " : " + QuestManager.questManager.currentQuestList[i].goldReward;
                }
                else if (QuestManager.questManager.currentQuestList[i].progress == Quest.QuestProgress.COMPLETE)
                {
                    questDescription.text = QuestManager.questManager.currentQuestList[i].congratulation;
                    questSummary.text = QuestManager.questManager.currentQuestList[i].questObjective + " :" + QuestManager.questManager.currentQuestList[i].questObjectiveCount + " / " + QuestManager.questManager.currentQuestList[i].questObjectiveRequirement;
                    questReward.text = QuestManager.questManager.currentQuestList[i].rewardObjective + " : " + QuestManager.questManager.currentQuestList[i].goldReward;
                }
            }
            

        }
        
    }
}
