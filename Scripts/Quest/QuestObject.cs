using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestObject : MonoBehaviour
{
    private bool inTrigger = false;


    public List<int> availableQuestIDs = new List<int>();
    public List<int> receivableQuestIDs = new List<int>();

    public GameObject questMarker;
    //public GameObject quest_Button;
    public Image theIamge;

    public Sprite questAvailableSprite;
    public Sprite questReceivableSprite;


    // Start is called before the first frame update
    void Start()
    {
        //quest_Button.SetActive(false);
        SetQuestMaker();
    }

    public void SetQuestMaker()
    {
        if (QuestManager.questManager.CheckCompleteQuests(this))
        {
            questMarker.SetActive(true);
            theIamge.sprite = questReceivableSprite;
            theIamge.color = Color.yellow;
        }
        else if (QuestManager.questManager.CheckAvailableQuests(this))
        {
            questMarker.SetActive(true);
            theIamge.sprite = questAvailableSprite;
            theIamge.color = Color.yellow;
        }
        else if (QuestManager.questManager.CheckAcceptedQuests(this))
        {
            questMarker.SetActive(true);
            theIamge.sprite = questReceivableSprite;
            theIamge.color = Color.gray;
        }
        else
        {
            questMarker.SetActive(false);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
        //if (inTrigger/* && Input.GetKeyDown(KeyCode.Space)*/)
        //{
            //quest_Button.SetActive(inTrigger);

        //}
        
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            inTrigger = true;
            quest_Button.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            inTrigger = false;
            quest_Button.SetActive(false);
        }
    }

    public void QuestButtonON()
    {
        if (!QuestUIManager.uiManager.questPanelActive)
        {
            //Quest UI Manager
            //QuestManager.questManager.QuestRequest(this);
            QuestUIManager.uiManager.CheckQuests(this);
        }
    }
    */
}
