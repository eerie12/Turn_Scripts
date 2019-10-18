using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButton : MonoBehaviour
{
    public BaseAttacks magicAttacktoPerform;


    public void CastMagic()
    {

        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().Input4(magicAttacktoPerform);
            
    }
 
}
