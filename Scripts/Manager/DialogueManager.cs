using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject dialogueBar;
    [SerializeField] GameObject dialogueNameBar;

    [SerializeField] Text text_Dialogue;
    [SerializeField] Text text_Name;

    Dialogue[] dialogues;
   

    bool isDialogue = false; //対話中の場合trueに変換
    bool isNext = false;//入力待機


    //bool event_Finish = false;

    [Header("textの速度")]
    [SerializeField] float textDelay;//textの速度

    int lineCount = 0;//対話のカウントダウン用
    int contextCount = 0;//セリフのカウントダウン用
    PlayerControllerrbody playerMovement;
    HealthManager HM;
    FieldCameraController theEventCam;

    void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerrbody>();
        HM = FindObjectOfType<HealthManager>();
        theEventCam = FindObjectOfType<FieldCameraController>();

    }

    void Update()
    {
        if (isDialogue)
        {
            if (isNext)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isNext = false;
                    text_Dialogue.text = "";
                    if (++contextCount < dialogues[lineCount].contexts.Length)
                    {
                        StartCoroutine(TypeWriter());
                    }
                    else
                    {
                        contextCount = 0;
                        if (++lineCount < dialogues.Length)
                        {
                            CameraTargettingType();
                        }
                        else
                        {
                            EndDialogue();
                        }
                    }                   
                }
            }
        }        
    }


    public void ShowDialogue(Dialogue[] p_dialogue)
    {
        
       
        playerMovement.SettingUI(false);

        isDialogue = true;
        text_Dialogue.text = "";
        text_Name.text = "";
        //playerMovement.playerBody.SetActive(false);
        dialogues = p_dialogue;

        //SettingUI(true);
        theEventCam.CamStartSetting();
        CameraTargettingType();
        
        

        


    }

    void CameraTargettingType()
    {
        switch (dialogues[lineCount].cameraType)
        {
            case CameraType.ObjectFront: theEventCam.CameraTargetting(dialogues[lineCount].tf_Target);break;
            case CameraType.Reset: theEventCam.CameraTargetting(null,0.1f,true,false); break;
        }
        StartCoroutine(eventFinish());
        //StartCoroutine(TypeWriter());
    }

    void EndDialogue()
    {
        isDialogue = false;
        contextCount = 0;
        lineCount = 0;
        dialogues = null;
        isNext = false;
        theEventCam.CameraTargetting(null, 0.1f, true, true);

        HM.isFadeFromBlack = true;
        SettingUI(false);
        dialogueNameBar.SetActive(false);
        //if ()
        //{
            //SoundManager.instance.PlaySound("Town", 0);
        //}
        

    }

    void PlaySound()
    {
        if(dialogues[lineCount].VoiceName[contextCount] != "")
        {
            SoundManager.instance.PlaySound(dialogues[lineCount].VoiceName[contextCount],2);
        }
    }
    
    public IEnumerator eventFinish()
    {
        //playerMovement.SettingUI(false);
        //if (event_Finish)
        //{
        //    yield break;
        //}
        //event_Finish = true;
        yield return new WaitUntil(() => theEventCam.camEvent);

        StartCoroutine(TypeWriter());

        //event_Finish = false;
        
        
        //isDialogue = true;
        //text_Dialogue.text = "";
        //text_Name.text = "";
        //playerMovement.playerBody.SetActive(false);
        //dialogues = p_dialogue;

        //SettingUI(true);
        //theEventCam.CamStartSetting();
        //CameraTargettingType();

    }
    



    IEnumerator TypeWriter()
    {
        
        SettingUI(true);

        string t_ReplaceText = dialogues[lineCount].contexts[contextCount];
        t_ReplaceText = t_ReplaceText.Replace("'", "、");//'を、に置換
        t_ReplaceText = t_ReplaceText.Replace("\\n", "\n");//'を、に置換

        //text_Dialogue.text = t_ReplaceText;
        

        bool t_white = false, t_red = false; 
        bool t_ignore = false, t_green = false;

        for (int i = 0; i < t_ReplaceText.Length; i++)
        {
            switch (t_ReplaceText[i])
            {
                case 'ⓦ': t_white = true; t_red = false; t_green = false; t_ignore = true; break;
                case 'ⓡ': t_white = false; t_red = true; t_green = false; t_ignore = true; break;
                case 'ⓖ': t_white = false; t_red = false; t_green = true; t_ignore = true; break;
                //case '①': SoundManager.instance.PlaySound("Dialogue", 1); t_ignore = true; break;

            }

            string t_letter = t_ReplaceText[i].ToString();

            if (!t_ignore)
            {
                if (t_white) { t_letter = "<color=#ffffff>" + t_letter + "</color>"; }
                else if (t_red) { t_letter = "<color=#FFA22A>" + t_letter + "</color>"; }
                else if (t_green) { t_letter = "<color=#C8EF76>" + t_letter + "</color>"; }
                text_Dialogue.text += t_letter;
            }
            t_ignore = false;


            //text_Dialogue.text += t_ReplaceText[i];
            yield return new WaitForSeconds(textDelay);
        }

        isNext = true;
        

    }

    void SettingUI(bool p_flag)
    {
        dialogueBar.SetActive(p_flag);
        if (p_flag)
        {
            if(dialogues[lineCount].name == "")
            {
                dialogueNameBar.SetActive(false);
            }
            else
            {
                dialogueNameBar.SetActive(true);
                text_Name.text = dialogues[lineCount].name;
            }
        }
        else
        {
            dialogueNameBar.SetActive(false);
        }
        
        
    }

   

 




}
