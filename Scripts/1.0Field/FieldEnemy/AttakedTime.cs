using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttakedTime : MonoBehaviour
{
    //MapEvent mapEvent;
    public Animator anim;


    public void Boss_Move(int a)
    {
        if (anim)
        {
            anim.SetInteger("move", a);
        }
        
    }

    public void Boss_WildEvent()
    {
        if (anim)
        {
            anim.SetTrigger("Field_wild");
        }
    }

    public void Boss_WildJump()
    {
        if (anim)
        {
            anim.SetTrigger("Field_Event_Jump");
        }
    }

    public void Boss_WildEvent_Sound()
    {
        SoundManager.instance.PlaySound("Event_Boss_End", 2);
    }




    public void SlowTimeEnd()
    {
        Time.timeScale = 1.0f;
    }
        

}
