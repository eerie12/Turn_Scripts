using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BaseAttacks : MonoBehaviour
{
    public string attackName;
    public string attackDiscription;
    public float attackDamage;//基本ダメージ 15, lv 10 stamina 35 = basedmg +stamina +lv = 60
    public float attackCost;// 注文クエスト
    public float attackBuff;
    public float attackHeal;



}
