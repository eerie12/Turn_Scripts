using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin_Ani : MonoBehaviour
{
    Animation anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Coin_Add_Ani()
    {
        anim.Play("CoinGet_Ani");
        Debug.Log("coin");
    }
}
