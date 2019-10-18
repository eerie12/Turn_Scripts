using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warp : MonoBehaviour
{
    public Transform warpPoint;
    public GameObject thePlayer;


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == thePlayer)
        {
            thePlayer.transform.position = warpPoint.transform.position;
        }
    }
}
