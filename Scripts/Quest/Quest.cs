using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
   public enum QuestProgress { NOT_AVAILABLE,AVAILABLE,ACCEPTED,COMPLETE,DONE}

    public string title;
    public int id;
    public QuestProgress progress;
    public string description;
    public string hint;
    public string congratulation;
    public string summary;
    public int nextQeust;

    public string questObjective;
    
    public string rewardObjective;
    public int questObjectiveCount;
    public int questObjectiveRequirement;

    public int expReward;
    public int goldReward;
    public string itemReward;
    public int questDialogue;
}

    

