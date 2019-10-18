using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class qButtonScript : MonoBehaviour
{
    public int questID;
    public Text questTitle;

    public Transform acceptButton;
    public Transform giveUpButton;
    public Transform completeButton;
    private GameObject ui_Quest;

    private qButtonScript acceptButtonScript;
    private qButtonScript giveUpButtonScript;
    private qButtonScript completeButtonScript;

    private Image acceptImage;
    private Image giveUpButtonImage;
    private Image completeButtonImage;
    private Button acceptButtonOnOff;
    private Button giveUpButtonOnOff;
    private Button completeButtonOnOff;
    private Text acceptText;
    private Text giveUpText;
    private Text completeText;

    private void Awake()
    {
        
    }
    //Show all info
    void Start()
    {
        ui_Quest = GameObject.Find("UI_Quest");

        acceptButton =  GameObject.Find("QuestButton_Obj").transform.GetChild(0);
        //acceptButton = ui_Quest.transform.Find("QuestPanel").gameObject.transform.Find("QuPanel").gameObject.transform.Find("QuestDescription").gameObject.transform.Find("GameObject").gameObject.transform.Find("AcceptButton");
        //acceptButton = GameObject.FindWithTag("AcceptButton");
        //acceptButton = GameObject.FindGameObjectWithTag("AcceptButton");
        //acceptButton = QuestUIManager.uiManager.QuestTransformDic[0];
        acceptButtonScript = acceptButton.GetComponent<qButtonScript>();
        acceptImage = acceptButton.GetComponent<Image>();
        acceptButtonOnOff = acceptButton.GetComponent<Button>();
        acceptText = acceptButton.transform.Find("Text").GetComponent<Text>();
        acceptImage.enabled = false;
        acceptButtonOnOff.enabled = false;
        acceptText.enabled = false;

        giveUpButton = GameObject.Find("QuestButton_Obj").transform.GetChild(1);
        //giveUpButton = GameObject.FindWithTag("GiveUPButton");
        //giveUpButton = GameObject.FindGameObjectWithTag("GiveUPButton").transform;
        //giveUpButton = QuestUIManager.uiManager.QuestTransformDic[1];
        giveUpButtonScript = giveUpButton.GetComponent<qButtonScript>();
        giveUpButtonImage = giveUpButton.GetComponent<Image>();
        giveUpButtonOnOff = giveUpButton.GetComponent<Button>();
        giveUpText = giveUpButton.transform.Find("Text").GetComponent<Text>();
        giveUpButtonImage.enabled = false;
        giveUpButtonOnOff.enabled = false;
        giveUpText.enabled = false;

        completeButton = GameObject.Find("QuestButton_Obj").transform.GetChild(2);
        //completeButton = GameObject.FindWithTag("CompleteButton");
       // completeButton = GameObject.FindGameObjectWithTag("CompleteButton");
        //completeButton = QuestUIManager.uiManager.QuestTransformDic[2];
        completeButtonScript = completeButton.GetComponent<qButtonScript>();
        completeButtonImage = completeButton.GetComponent<Image>();
        completeButtonOnOff = completeButton.GetComponent<Button>();
        completeText = completeButton.transform.Find("Text").GetComponent<Text>();
        completeButtonImage.enabled = false;
        completeButtonOnOff.enabled = false;
        completeText.enabled = false;

        //acceptImage.enabled = false;
        //giveUpButtonImage.enabled = false;
        //completeButtonImage.enabled = false;
        //acceptButton.gameObject.SetActive(false);
        //giveUpButton.gameObject.SetActive(false);
        //completeButton.gameObject.SetActive(false);

        Invoke("QuestButtonSet", 0.01f);

    }

    public void QuestButtonSet()
    {
        acceptImage.enabled = true;
        acceptButtonOnOff.enabled = true;
        acceptText.enabled = true;

        giveUpButtonImage.enabled = true;
        giveUpButtonOnOff.enabled = true;
        giveUpText.enabled = true;

        completeButtonImage.enabled = true;
        completeButtonOnOff.enabled = true;
        completeText.enabled = true;

        acceptButton.gameObject.SetActive(false);
        giveUpButton.gameObject.SetActive(false);
        completeButton.gameObject.SetActive(false);
        Debug.Log("buttonSet");
    }




    public void ShowAllInfos()
    {
        SoundManager.instance.PlaySound("Button", 1);
        Debug.Log("buttonSet");
        QuestUIManager.uiManager.showSelectedQuest(questID);

        Debug.Log("1");
        if (QuestManager.questManager.RequestAvailableQuest(questID))
        {
            
            
            acceptButton.gameObject.SetActive(true);
            //acceptImage.enabled = true;
            //acceptButtonOnOff.enabled = true;
            //acceptText.enabled = true;
            acceptButtonScript.questID = questID;
        }
        else
        {
            //acceptImage.enabled = false;
            //acceptButtonOnOff.enabled = false;
            //acceptText.enabled = false;
            acceptButton.gameObject.SetActive(false);
        }
        //give up button
        if (QuestManager.questManager.RequestAcceptedQuest(questID))
        {
            giveUpButton.gameObject.SetActive(true);
            //giveUpButtonImage.enabled = true;
            //giveUpButtonOnOff.enabled = true;
            //giveUpText.enabled = true;
            giveUpButtonScript.questID = questID;
        }
        else
        {
            //giveUpButtonImage.enabled = false;
            //giveUpButtonOnOff.enabled = false;
            //giveUpText.enabled = false;
            giveUpButton.gameObject.SetActive(false);
        }

        //complete button
        if (QuestManager.questManager.RequestCompleteQuest(questID))
        {
            completeButton.gameObject.SetActive(true);
            //completeButtonImage.enabled = true;
            //completeButtonImage.enabled = true;
            //completeText.enabled = true;
            completeButtonScript.questID = questID;
        }
        else
        {
            //completeButtonImage.enabled = false;
            //completeButtonOnOff.enabled = false;
            //completeText.enabled = false;
            completeButton.gameObject.SetActive(false);
        }


    }
    public void AcceptQuest()
    {
        QuestManager.questManager.AcceptQuest(questID);
        QuestUIManager.uiManager.HideQuestPanel();

        //Update All Npc 
        QuestObject[] currentQuestGuys = FindObjectsOfType(typeof(QuestObject)) as QuestObject[];
        foreach(QuestObject obj in currentQuestGuys)
        {
            obj.SetQuestMaker();
        }

    }

    public void GiveUpQuest()
    {
        QuestManager.questManager.GIveUpQuest(questID);
        QuestUIManager.uiManager.HideQuestPanel();

        //Update All Npc 
        QuestObject[] currentQuestGuys = FindObjectsOfType(typeof(QuestObject)) as QuestObject[];
        foreach (QuestObject obj in currentQuestGuys)
        {
            obj.SetQuestMaker();
        }

    }

    public void CompleteQuest()
    {
        QuestManager.questManager.CompleteQuest(questID);
        QuestUIManager.uiManager.HideQuestPanel();

        //Update All Npc 
        QuestObject[] currentQuestGuys = FindObjectsOfType(typeof(QuestObject)) as QuestObject[];
        foreach (QuestObject obj in currentQuestGuys)
        {
            obj.SetQuestMaker();
        }

        

    }

    public void ClosePanel()
    {
        QuestUIManager.uiManager.HideQuestPanel();
        acceptButton.gameObject.SetActive(false);
        giveUpButton.gameObject.SetActive(false);
        completeButton.gameObject.SetActive(false);
    }

}
