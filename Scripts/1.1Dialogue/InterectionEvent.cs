using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterectionEvent : MonoBehaviour
{
    [SerializeField] DialogueEvent dialogueEvent;

    //public bool eventCheck = false;

    public Dialogue[] GetDialogue()
    {
        if (dialogueEvent.name != "Start")
        {
            if (!GameManager.instance.eventStartCheck)
            {
                //最初の対話
                if (GameManager.instance.eventCheck == 0)
                {
                    //eventCheck = 0;
                    GameManager.instance.eventCheck = 1;
                    dialogueEvent.dialogues = SettingDialogue(dialogueEvent.dialogues, (int)dialogueEvent.line.x, (int)dialogueEvent.line.y);
                    return dialogueEvent.dialogues;

                }
                //後の対話
                else if (GameManager.instance.eventCheck == 1)
                {

                    //eventCheck = 1;
                    dialogueEvent.dialoguesB = SettingDialogue(dialogueEvent.dialoguesB, (int)dialogueEvent.lineB.x, (int)dialogueEvent.lineB.y);
                    return dialogueEvent.dialoguesB;
                }

                else if(GameManager.instance.eventCheck == 2)
                {

                    //eventCheck = 1;
                    GameManager.instance.eventCheck = 3;
                    dialogueEvent.dialoguesC = SettingDialogue(dialogueEvent.dialoguesC, (int)dialogueEvent.lineC.x, (int)dialogueEvent.lineC.y);
                    return dialogueEvent.dialoguesC;
                }
                else 
                {

                    //eventCheck = 1;
                    dialogueEvent.dialoguesD = SettingDialogue(dialogueEvent.dialoguesD, (int)dialogueEvent.lineD.x, (int)dialogueEvent.lineD.y);
                    return dialogueEvent.dialoguesD;
                }
            }
            //QuestClear後の対話
            else
            {
                dialogueEvent.dialoguesE = SettingDialogue(dialogueEvent.dialoguesE, (int)dialogueEvent.lineE.x, (int)dialogueEvent.lineE.y);
                return dialogueEvent.dialoguesE;
            }
        }

        else
        {
            GameManager.instance.eventFlags[dialogueEvent.eventTiming.eventNum[0]] = true;
            dialogueEvent.dialogues = SettingDialogue(dialogueEvent.dialogues, (int)dialogueEvent.line.x, (int)dialogueEvent.line.y);
            return dialogueEvent.dialogues;
        }

        
        

        

        
    }

    Dialogue[] SettingDialogue(Dialogue[]p_Dialogue, int p_lineX,int p_lineY)
    {
        if(dialogueEvent.name != "Start")
        {
            Dialogue[] t_Dialogues = GameManager.instance.GetDialogue(p_lineX, p_lineY);
            if (!GameManager.instance.eventStartCheck)
            {

                if (GameManager.instance.eventCheck == 0)
                {
                    for (int i = 0; i < dialogueEvent.dialogues.Length; i++)
                    {
                        t_Dialogues[i].tf_Target = p_Dialogue[i].tf_Target;
                        t_Dialogues[i].cameraType = p_Dialogue[i].cameraType;
                    }
                }
                else if(GameManager.instance.eventCheck == 1)/*if(eventCheck == 1)*/
                {
                    for (int i = 0; i < dialogueEvent.dialoguesB.Length; i++)
                    {
                        t_Dialogues[i].tf_Target = p_Dialogue[i].tf_Target;
                        t_Dialogues[i].cameraType = p_Dialogue[i].cameraType;
                    }
                }
                else if(GameManager.instance.eventCheck == 2)/*if(eventCheck == 1)*/
                {
                    for (int i = 0; i < dialogueEvent.dialoguesC.Length; i++)
                    {
                        t_Dialogues[i].tf_Target = p_Dialogue[i].tf_Target;
                        t_Dialogues[i].cameraType = p_Dialogue[i].cameraType;
                    }
                }
                else 
                {
                    for (int i = 0; i < dialogueEvent.dialoguesD.Length; i++)
                    {
                        t_Dialogues[i].tf_Target = p_Dialogue[i].tf_Target;
                        t_Dialogues[i].cameraType = p_Dialogue[i].cameraType;
                    }
                }
                //return t_Dialogues;

            }
            else
            {
                for (int i = 0; i < dialogueEvent.dialoguesE.Length; i++)
                {
                    t_Dialogues[i].tf_Target = p_Dialogue[i].tf_Target;
                    t_Dialogues[i].cameraType = p_Dialogue[i].cameraType;
                }
            }

            return t_Dialogues;
        }
        else
        {
            Dialogue[] t_Dialogues = GameManager.instance.GetDialogue(p_lineX, p_lineY);
            for (int i = 0; i < dialogueEvent.dialogues.Length; i++)
            {
                t_Dialogues[i].tf_Target = p_Dialogue[i].tf_Target;
                t_Dialogues[i].cameraType = p_Dialogue[i].cameraType;
            }
            return t_Dialogues;
        }
        


        //dialogueEvent.dialogues = t_DialogueEvent.dialogues;
    }
}
