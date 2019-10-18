using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlarmSlideManager : MonoBehaviour
{

    public Animation anim;
    public static bool alarmFinished = true;
    public static int alarmInt = 0;
    private Text alarmText;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
        alarmText = transform.Find("Text").gameObject.GetComponent<Text>();
    }

    public IEnumerator BattleAlarmAppear()
    {
        if(alarmInt == 1)
        {
            alarmText.text = "ポーション獲得";
        }
        else if(alarmInt == 2)
        {
            alarmText.text = "Advanced"+ System.Environment.NewLine + "Battle";
        }
        else
        {
            alarmText.text = "ATTACK UP!";
        }
        
       
        anim.Play("BattleAlarmAppear");

        if (GameManager.instance.advancedBattle)
        {
            GameManager.instance.advancedBattle = false;
        }

        yield return new WaitForSeconds(2f);

        anim.Play("BattleAlarmDisappear");

        alarmFinished = true;
    }

    public void AlarmIntRest()
    {
        alarmInt = 0;
    }
}
