using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CreatePlayer : MonoBehaviour
{
    BasePlayerClass newPlayer;
    private string playName = "Player";

    public Text ATKText;
    public Text DEFText;
    public Text HealText;
    public GameObject stateText_Obj;
    public Text stateText;
    private bool cor_Start = false;
    //private bool cor_Stop  = false;


    private int poinsToSpend = 20;
    public Text pointText;
    // Start is called before the first frame update
    void Start()
    {
        newPlayer = new BasePlayerClass();
        UpdateUI();
    }

    public void CreateNewPlayer()
    {
        SoundManager.instance.PlaySound("Button", 1);
        newPlayer.playerName = playName;

        GameManager.instance.PlayerName = newPlayer.playerName;
        GameManager.instance.PlayerClass = newPlayer.playerClass;
        GameManager.instance.BaseATK = newPlayer.BaseATK;
        GameManager.instance.BaseDEF = newPlayer.BaseDEF;
        GameManager.instance.BaseHeal = newPlayer.BaseHeal;
        GameManager.instance.currentCoin = poinsToSpend;
        GameObject coinText = GameObject.Find("coinText");
        Text conText_text = coinText.GetComponent<Text>();
        conText_text.text = GameManager.instance.currentCoin.ToString();

        SaveInfo.SaveAllInfo();
        SetHeroClass();




    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetHeroClass()
    {
        SoundManager.instance.PlaySound("Button", 1);
        poinsToSpend = GameManager.instance.currentCoin;
        newPlayer.playerClass = new BaseHero();
        if (GameManager.instance.BaseATK != 0 || GameManager.instance.BaseDEF != 0 || GameManager.instance.BaseHeal != 0)
        {
            LoadInfo.LoadAllInfo();
            newPlayer.playerClass.baseATK = GameManager.instance.BaseATK;
            newPlayer.playerClass.baseDEF = GameManager.instance.BaseDEF;
            newPlayer.playerClass.baseHeal = GameManager.instance.BaseHeal;
        }
        newPlayer.BaseATK = newPlayer.playerClass.baseATK;
        newPlayer.BaseDEF = newPlayer.playerClass.baseDEF;
        newPlayer.BaseHeal = newPlayer.playerClass.baseHeal;
        UpdateUI();
    }

    void UpdateUI()
    {
        ATKText.text = newPlayer.BaseATK.ToString();
        DEFText.text = newPlayer.BaseDEF.ToString();
        HealText.text = newPlayer.BaseHeal.ToString();

        pointText.text = poinsToSpend.ToString();
    }
    public void SetBaseATK(int amount)
    {
        SoundManager.instance.PlaySound("Button", 1);
        if (newPlayer.playerClass != null)
        {
            float startBase = newPlayer.BaseATK;
            if(amount > 0 && poinsToSpend > 0)
            {
                newPlayer.BaseATK += amount;
                poinsToSpend -= 1;
                UpdateUI();
            }
            else if(amount<0 && newPlayer.BaseATK > newPlayer.playerClass.BaseATK)
            {
                newPlayer.BaseATK += amount;
                poinsToSpend += 1;
                UpdateUI();
            }
            else if(amount > 0 && poinsToSpend <= 0)
            {
                //cor_Stop = true;
                CorStop();
                StartCoroutine(StateInfoAppear_Cor(0));
            }
            else if (amount < 0 && startBase <= newPlayer.BaseATK)
            {
                //cor_Stop = true;
                CorStop();
                StartCoroutine(StateInfoAppear_Cor(1));
            }
        }
        else
        {
            Debug.Log("classなし");
        }
    }

    public void SetBaseDEF(int amount)
    {
        SoundManager.instance.PlaySound("Button", 1);
        if (newPlayer.playerClass != null)
        {
            float startBase = newPlayer.BaseDEF;

            if (amount > 0 && poinsToSpend > 0)
            {
                newPlayer.BaseDEF += amount;
                poinsToSpend -= 1;
                UpdateUI();
            }
            else if (amount < 0 && newPlayer.BaseDEF > newPlayer.playerClass.BaseDEF)
            {
                newPlayer.BaseDEF += amount;
                poinsToSpend += 1;
                UpdateUI();
            }
            else if (amount > 0 && poinsToSpend <= 0)
            {
                //StopAllCoroutines();
                //cor_Stop = true;
                CorStop();
                StartCoroutine(StateInfoAppear_Cor(0));
            }
            else if(amount < 0 && startBase <= newPlayer.BaseDEF)
            {
                //cor_Stop = true;
                CorStop();
                StartCoroutine(StateInfoAppear_Cor(1));
            }
        }
        else
        {
            Debug.Log("classなし");
        }
    }

    public void SetBaseHeal(int amount)
    {
        SoundManager.instance.PlaySound("Button", 1);
        if (newPlayer.playerClass != null)
        {
            float startBase = newPlayer.BaseHeal;
            if (amount > 0 && poinsToSpend > 0)
            {
                newPlayer.BaseHeal += amount;
                poinsToSpend -= 1;
                UpdateUI();
            }
            else if (amount < 0 && newPlayer.BaseHeal > newPlayer.playerClass.BaseHeal)
            {
                newPlayer.BaseHeal += amount;
                poinsToSpend += 1;
                UpdateUI();
            }
            else if (amount > 0 && poinsToSpend <= 0)
            {
                //StopAllCoroutines();
                //cor_Stop = true;
                CorStop();
                StartCoroutine(StateInfoAppear_Cor(0));
            }
            else if (amount < 0 && startBase <= newPlayer.BaseHeal)
            {
                //cor_Stop = true;
                CorStop();
                StartCoroutine(StateInfoAppear_Cor(1));
            }
        }
        else
        {
            Debug.Log("classなし");
        }
    }

    public void LoadStuff()
    {
        LoadInfo.LoadAllInfo();
    }

    public void CorStop()
    {
        StopAllCoroutines();
        cor_Start = false;
        if (stateText_Obj.activeSelf)
        {
            stateText_Obj.SetActive(false);
        }
    }


    public IEnumerator StateInfoAppear_Cor(int state, string QuestItem = null,string QuestName = null, int currentItemCount = 0, int maxItemCount = 0,float time =1.2f)
    {
        if (cor_Start)
        {
            yield break;
        }

        cor_Start = true;




        stateText_Obj.SetActive(true);
        if(state == 0)
        {
            stateText.text = "<color=yellow>" + "Coinが足りません。" + "</color>";
        }
        else if(state == 1)
        {
            stateText.text = "<color=yellow>" + "これ以上、下げることは出来ません。" + "</color>";
        }
        else if(state == 2)
        {
            stateText.text = "<color=yellow>" + QuestItem + "   " + currentItemCount.ToString() + "  /  " + maxItemCount.ToString() + "</color>";
        }
        else if(state == 3)
        {
            stateText.text = "<color=yellow>" + QuestName + "(Complete)\n" + QuestItem + "   " + currentItemCount.ToString() + "  /  " + maxItemCount.ToString() + "</color>";
        }


        yield return new WaitForSeconds(time);
        stateText_Obj.SetActive(false);

        cor_Start = false;

        
    }
}
