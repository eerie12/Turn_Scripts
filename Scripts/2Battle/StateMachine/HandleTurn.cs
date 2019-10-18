using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurn
{
    public string Attacker;//攻撃する方の名前
    public string Type;
    public GameObject AttackersGameobject;//誰が攻撃するか
    public GameObject AttackersTarget;//誰が攻撃をされるか
    public List<GameObject> KillTarget = new List<GameObject>();
    public BaseAttacks choosenAttack;

 
   
}
