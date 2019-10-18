using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseHero: BaseClass
{

    public BaseHero()
    {
        //Hero(battlepr)
        theName  = "Hero(battlepr)";
        baseHP   = 200f;
        curHP    = 200f;

        baseMP   = 100f;
        curMP    = 0;

        baseATK  = 5f;
        curATK   = 0f;
        baseDEF  = 5f;
        curDEF   = 3f;
        baseHeal = 10f;
        curHeal  = 0f;




}

    public List<BaseAttacks> MagicAttacks = new List<BaseAttacks>();
    public List<BaseAttacks> Heal = new List<BaseAttacks>();
    public List<BaseAttacks> WildAttack = new List<BaseAttacks>();

}
