using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Start : MonoBehaviour
{
    Title title;
    public GameObject blackScreenObject;
    public Image blackScreen;
    public bool isFadeToBlack;
    public bool isFadeFromBlack;
    [SerializeField] float fadeSpeed;
    // Start is called before the first frame update

    private void Awake()
    {
        Screen.SetResolution(1280, 720, true);
    }

    void Start()
    {
        title = FindObjectOfType<Title>();
        isFadeFromBlack = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (isFadeToBlack)
        {
            //isFade_boss = true;
            blackScreenObject.SetActive(true);
            blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, Mathf.MoveTowards(blackScreen.color.a, 1f, fadeSpeed * Time.deltaTime));
            if (blackScreen.color.a == 1f)
            {
                
                isFadeToBlack = false;
            }
        }



        if (isFadeFromBlack)
        {
            blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, Mathf.MoveTowards(blackScreen.color.a, 0f, fadeSpeed * Time.deltaTime));

            if (blackScreen.color.a == 0f)
            {
                title.BGM_Title.Play();
                blackScreenObject.SetActive(false);
                isFadeFromBlack = false;
            }


        }

    }

    
}

