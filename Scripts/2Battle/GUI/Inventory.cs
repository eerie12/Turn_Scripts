using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Inventory : MonoBehaviour
{
    BattleStateMachine BSM;

    public static bool inventoryActivated = false;
    public static bool blackActivated = false;
    public static bool helpActiveated = false;
   

    [SerializeField]
    private GameObject go_InventoryBase;
    [SerializeField]
    private GameObject go_SlotsParebt;
    public GameObject gameOverText;
    public GameObject titleButton;
    public GameObject endButton;
    public GameObject help;

    private Slot[] slots;

    public GameObject blackScreenObject_Battle;
    public Image blackScreen_Battle;
    public bool EndBattleAction;
    public bool isFadeToDialogueBackground_Battle;
    public bool isFadeToBlack_Battle;
    public bool isFadeFromBlack_Battle;
    //private bool defeatAction = false;
    public float fadeSpeed_Battle;

    //public bool DeadEnd_Sound = false;

    [SerializeField] private string sceneName = "Title";


    // Start is called before the first frame update
    void Start()
    {
        inventoryActivated = false;
        blackActivated = false;
        helpActiveated = false;
        slots = go_SlotsParebt.GetComponentsInChildren<Slot>();
        BSM = FindObjectOfType<BattleStateMachine>();
    }

    // Update is called once per frame
    void Update()
    {
        Setting();

        if (isFadeToBlack_Battle)
        {
            //Debug.Log("blackScreenObject_Battle"+ isFadeToBlack_Battle);
            blackScreenObject_Battle.SetActive(true);
            //StartCoroutine(blackScreen_EndBattle());
            StartCoroutine(blackScreen_EndBattle());

        }

        if (isFadeToDialogueBackground_Battle)
        {
            blackScreenObject_Battle.SetActive(true);
            blackScreen_Battle.color = new Color(blackScreen_Battle.color.r, blackScreen_Battle.color.g, blackScreen_Battle.color.b, Mathf.MoveTowards(blackScreen_Battle.color.a, 0.3f, fadeSpeed_Battle * Time.deltaTime));
            if (blackScreen_Battle.color.a == 0.3f)
            {
                if (helpActiveated)
                {
                    Time.timeScale = 0.0f;
                }
                isFadeToDialogueBackground_Battle = false;
            }
        }




        if (isFadeFromBlack_Battle)
        {
            blackScreen_Battle.color = new Color(blackScreen_Battle.color.r, blackScreen_Battle.color.g, blackScreen_Battle.color.b, Mathf.MoveTowards(blackScreen_Battle.color.a, 0f, fadeSpeed_Battle * Time.deltaTime));

            if (blackScreen_Battle.color.a == 0f)
            {
                blackScreenObject_Battle.SetActive(false);
                isFadeFromBlack_Battle = false;
            }


        }

        
    }

    public void TryOpenInventory()
    {
        SoundManager.instance.PlaySound("Button", 1);

        inventoryActivated = !inventoryActivated;
        if (inventoryActivated)
        {

            titleButton.SetActive(true);
            endButton.SetActive(true);
        }
        else
        {

            titleButton.SetActive(false);
            endButton.SetActive(false);
        }


    }

    public void TryOpenHelp()
    {
        SoundManager.instance.PlaySound("Button", 1);
        helpActiveated = !helpActiveated;
    }


    private void Setting()
    {
        if (blackActivated)
        {
            isFadeToBlack_Battle = true;
        }
        else if (helpActiveated)
        {
            OpenHelp();
        }
        else if(!inventoryActivated)
        {
            CloseHelp();
            CloseInventory();
        }
        else if(inventoryActivated)
        {            
            OpenInventory();

        }


        
    }

    private void OpenInventory()
    {
        isFadeToDialogueBackground_Battle = true;
        go_InventoryBase.SetActive(true);
    }

    private void CloseInventory()
    {
        
        isFadeFromBlack_Battle = true;
        go_InventoryBase.SetActive(false);
    }

    private void OpenHelp()
    {

        isFadeToDialogueBackground_Battle = true;
        help.SetActive(true);
    }

    private void CloseHelp()
    {
        if(Time.timeScale != 1.0f)
        {
            Time.timeScale = 1.0f;
        }
        
        isFadeFromBlack_Battle = true;
        help.SetActive(false);
    }




    public void AcquireItem(Item_Pr _item, int _count = 1)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if(slots[i].item != null)
            {
                if (slots[i].item.itmeName == _item.itmeName)
                {

                    slots[i].SetSlotCount(_count);
                    return;

                }
            }
            

        }

        for (int i = 0; i < slots.Length; i++)
        {
            
            if (slots[i].item == null)
            {
                slots[i].AddItem(_item, _count);
                return;
            }

            
        }
    }


    private IEnumerator blackScreen_EndBattle()
    {



        yield return new WaitForSeconds(2f);
        go_InventoryBase.SetActive(false);
        help.SetActive(false);
        for (int i = 0; i < BSM.EnemysInBattle.Count; i++)
        {

            BSM.EnemysInBattle[i].transform.Find("EnemyUI").gameObject.SetActive(false);

            BSM.EnemysInBattle[i].transform.Find("EnemyUI").transform.Find("Selector").gameObject.SetActive(false);


        }
       
        BSM.enemyBarStop = true;
        blackScreen_Battle.color = new Color(blackScreen_Battle.color.r, blackScreen_Battle.color.g, blackScreen_Battle.color.b, Mathf.MoveTowards(blackScreen_Battle.color.a, 1f, (fadeSpeed_Battle - 1.5f) * Time.deltaTime));
        if (blackScreen_Battle.color.a == 1f)
        {

            
            gameOverText.SetActive(true);
            titleButton.SetActive(true);
            endButton.SetActive(true);





        }



    }

    public void ClickToTitle()
    {

        SoundManager.instance.PlaySound("Button", 1);

        Invoke("MoveTitle", 0.5f);

    }

    public void MoveTitle()
    {
        blackActivated = false;
        Destroy(GameObject.Find("GameManager"));
        Destroy(GameObject.Find("QuestManager"));
        SceneManager.LoadScene(sceneName);
    }

    public void ClickToExit()
    {
        SoundManager.instance.PlaySound("Button", 1);
        Invoke("MoveExit", 0.5f);

    }

    public void MoveExit()
    {
        blackActivated = false;
        Destroy(GameObject.Find("GameManager"));
        Destroy(GameObject.Find("QuestManager"));
        Application.Quit();
    }

}
