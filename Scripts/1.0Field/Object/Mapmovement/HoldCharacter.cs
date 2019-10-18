using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldCharacter : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {


        other.transform.parent = gameObject.transform;

    }

    private void OnTriggerExit(Collider other)
    {


        other.transform.parent = null;


    }

        


        
}
