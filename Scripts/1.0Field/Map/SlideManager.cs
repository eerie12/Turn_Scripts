using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideManager : MonoBehaviour
{

    //[SerializeField] private GameObject uiSpacer;
    BattleStateMachine BSM;
    public Animation anim;

    public static bool isFinished = true;
    public bool slideOnOff;


    void Start()
    {
        BSM = FindObjectOfType<BattleStateMachine>();
        anim = GetComponent<Animation>();
        slideOnOff = false;
    }


    public IEnumerator AppearSlide()
    {
        BSM.slideOn = true;
        anim.Play("MagicSlideAppear");

        yield return new WaitForSeconds(0.5f);

        slideOnOff = true;
        isFinished = true;





    }

    public IEnumerator DisappearSlide()

    {
        BSM.slideOn = false;
        anim.Play("MagicSlideDisappear");

        yield return new WaitForSeconds(0.5f);


        slideOnOff = false;
        isFinished = true;

    }

    






}
