using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    

    public static QuestManager questManager;

    public List<Quest> questList = new List<Quest>();
    public List<Quest> currentQuestList = new List<Quest>();


    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (questManager == null)
        {
            questManager = this;
        }
        else if(questManager != this)
        {
            Destroy(gameObject);
        }

    }

    #region questの処理
    public void QuestRequest(QuestObject NPCthisQuestobject)
    {
        //AVAILABLE QUEST
        if(NPCthisQuestobject.availableQuestIDs.Count > 0)
        {
            for(int i = 0; i< questList.Count; i++)
            {
                for(int j = 0; j < NPCthisQuestobject.availableQuestIDs.Count; j++)
                {
                    if(questList[i].id == NPCthisQuestobject.availableQuestIDs[j] && questList[i].progress == Quest.QuestProgress.AVAILABLE)
                    {
                       
                        QuestUIManager.uiManager.questAvailable = true;
                        QuestUIManager.uiManager.availableQuests.Add(questList[i]);
                    }
                }
            }
        }
        if (currentQuestList.Count > 0)
        {
            QuestUIManager.uiManager.questRunning = true;
        }
        
    }

    //ACCEPT QUEST
    public void AcceptQuest(int questID)
    {
        for(int i = 0; i < questList.Count; i++)
        {
            if(questList[i].id == questID && questList[i].progress == Quest.QuestProgress.AVAILABLE)
            {

                currentQuestList.Add(questList[i]);
                questList[i].progress = Quest.QuestProgress.ACCEPTED;
            } 
        }
    }

    //GIVE UP QUEST
    public void GIveUpQuest(int questID)
    {
        for (int i = 0; i < currentQuestList.Count; i++)
        {
            if (currentQuestList[i].id == questID && currentQuestList[i].progress == Quest.QuestProgress.ACCEPTED)
            {
                currentQuestList[i].progress = Quest.QuestProgress.AVAILABLE;
                currentQuestList[i].questObjectiveCount = 0;
                currentQuestList.Remove(currentQuestList[i]);
            }
        }
    }

    //COMPLETE QUEST
    public void CompleteQuest(int questID)
    {
        for(int i = 0; i< currentQuestList.Count; i++)
        {
            if(currentQuestList[i].id == questID && currentQuestList[i].progress == Quest.QuestProgress.COMPLETE)
            {
                if (currentQuestList[i].questDialogue != 0)
                {
                    GameManager.instance.eventCheck = currentQuestList[i].questDialogue;
                }
                GameManager.instance.AddFieldCoin(currentQuestList[i].goldReward);  
                currentQuestList[i].progress = Quest.QuestProgress.DONE;
                currentQuestList.Remove(currentQuestList[i]);
                
            }
            //reward
        }
        //check for chain quest
        CheckChainQuest(questID);
    }

    void CheckChainQuest(int questID)
    {
        int tempID = 0;
        for(int i = 0; i < questList.Count; i++)
        {
            if(questList[i].id == questID && questList[i].nextQeust > 0)
            {
                tempID = questList[i].nextQeust;
            }
        }

        if (tempID > 0)
        {
            for(int i = 0; i < questList.Count; i++)
            {
                if(questList[i].id == tempID && questList[i].progress == Quest.QuestProgress.NOT_AVAILABLE)
                {
                    questList[i].progress = Quest.QuestProgress.AVAILABLE;
                } 
            }
        }
    }

    //要求されるquestItemの処理
    public void AddQuestItem(string questObjective, int itemAmount)
    {
        for (int i = 0; i < currentQuestList.Count; i++)
        {
            CreatePlayer statePlayer = FindObjectOfType<CreatePlayer>();

            if (currentQuestList[i].questObjective == questObjective && currentQuestList[i].progress == Quest.QuestProgress.ACCEPTED)
            {
                statePlayer.CorStop();
                StartCoroutine(statePlayer.StateInfoAppear_Cor(2, currentQuestList[i].questObjective, null, currentQuestList[i].questObjectiveCount, currentQuestList[i].questObjectiveRequirement, 2f));
                currentQuestList[i].questObjectiveCount += itemAmount;
            }
            if (currentQuestList[i].questObjectiveCount >= currentQuestList[i].questObjectiveRequirement && currentQuestList[i].progress == Quest.QuestProgress.ACCEPTED)
            {
                statePlayer.CorStop();
                StartCoroutine(statePlayer.StateInfoAppear_Cor(3, currentQuestList[i].questObjective, currentQuestList[i].title, currentQuestList[i].questObjectiveCount, currentQuestList[i].questObjectiveRequirement, 2f));
                currentQuestList[i].progress = Quest.QuestProgress.COMPLETE;
            }

        }
    }

    #endregion

    #region questのcheck

    //BOOL
    public bool RequestAvailableQuest(int questID)
    {
        for(int i = 0; i < questList.Count; i++)
        {
            if(questList[i].id == questID && questList[i].progress == Quest.QuestProgress.AVAILABLE)
            {
                return true;
            }
        }
        return false;
    }

    public bool RequestAcceptedQuest(int questID)
    {
        for (int i = 0; i < questList.Count; i++)
        {
            if (questList[i].id == questID && questList[i].progress == Quest.QuestProgress.ACCEPTED)
            {
                return true;
            }
        }
        return false;
    }

    public bool RequestCompleteQuest(int questID)
    {
        for (int i = 0; i < questList.Count; i++)
        {
            if (questList[i].id == questID && questList[i].progress == Quest.QuestProgress.COMPLETE)
            {
                return true;
            }
        }
        return false;
    }

    //BOOL2(Npcに該当するquest)
    public bool CheckAvailableQuests(QuestObject NPCQuestObject)
    {
        for(int i = 0; i< questList.Count; i++)
        {
            for(int j = 0; j < NPCQuestObject.availableQuestIDs.Count; j++)
            {
                if(questList[i].id == NPCQuestObject.availableQuestIDs[j] && questList[i].progress == Quest.QuestProgress.AVAILABLE)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool CheckAcceptedQuests(QuestObject NPCQuestObject)
    {
        for (int i = 0; i < questList.Count; i++)
        {
            for (int j = 0; j < NPCQuestObject.receivableQuestIDs.Count; j++)
            {
                if (questList[i].id == NPCQuestObject.receivableQuestIDs[j] && questList[i].progress == Quest.QuestProgress.ACCEPTED)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool CheckCompleteQuests(QuestObject NPCQuestObject)
    {
        for (int i = 0; i < questList.Count; i++)
        {
            for (int j = 0; j < NPCQuestObject.receivableQuestIDs.Count; j++)
            {
                if (questList[i].id == NPCQuestObject.receivableQuestIDs[j] && questList[i].progress == Quest.QuestProgress.COMPLETE)
                {
                    return true;
                }
            }
        }
        return false;
    }

    #endregion

    #region questlog
    //Show Quest Log
    public void ShowQuestLog(int questID)
    {
        for(int i = 0; i< currentQuestList.Count; i++)
        {
            if(currentQuestList[i].id == questID)
            {
                QuestUIManager.uiManager.ShowQuestLog(currentQuestList[i]); 
            }
        }
    }
    #endregion
}
