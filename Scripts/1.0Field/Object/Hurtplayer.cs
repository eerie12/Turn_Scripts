using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtplayer : MonoBehaviour
{
    public int damgeToGive;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {

            FindObjectOfType<HealthManager>().HurtPlayer(damgeToGive);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            FindObjectOfType<HealthManager>().HurtPlayer(damgeToGive);
        }
    }


}
