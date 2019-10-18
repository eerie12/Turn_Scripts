using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseClass
{
    public string theName;

    public float baseHP;
    public float curHP;

    public float baseMP;
    public float curMP;

    public float baseATK;
    public float curATK;
    public float baseDEF;
    public float curDEF;
    public float baseHeal;
    public float curHeal;

    public List<BaseAttacks> attacks = new List<BaseAttacks>(); 

    public float BaseATK
    {
        get { return baseATK; }
        set { baseATK = value; }
    }

    public float BaseDEF
    {
        get { return baseDEF; }
        set { baseDEF = value; }
    }

    public float BaseHeal
    {
        get { return baseHeal; }
        set { baseHeal = value; }
    }


}
