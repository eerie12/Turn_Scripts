using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    [SerializeField] private GameObject box;
    [SerializeField] private Animator anim;
    [SerializeField] private ParticleSystem boxParticle;
    [SerializeField] private int fieldCoin;


    public void BoxOpen()
    {

        anim.SetTrigger("open");
        SoundManager.instance.PlaySound("BoxOpen", 1);
        GameManager.instance.AddFieldCoin(fieldCoin);
        boxParticle.Play();
        Destroy(box, 2.7f);
        Destroy(gameObject, 3f);


    }
}
