using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadInfo
{
   public static void LoadAllInfo()
    {
        GameManager.instance.PlayerName = PlayerPrefs.GetString("PLAYERNAME");
        GameManager.instance.BaseATK = PlayerPrefs.GetFloat("BASEATK");
        GameManager.instance.BaseDEF = PlayerPrefs.GetFloat("BASEDEF");
        GameManager.instance.BaseHeal = PlayerPrefs.GetFloat("BASEHEAL");
        Debug.Log("BaseATK" + GameManager.instance.BaseATK);

    }
    
}
