using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiCamera : MonoBehaviour
{
    [Header("MainCamera")]
    [SerializeField] private GameObject[] playerCamera;
     private HeroStateMachine HSM;
    // Start is called before the first frame update
    void Start()
    {
        HSM = GameObject.FindGameObjectWithTag("Hero").GetComponent<HeroStateMachine>();
        playerCamera[0] = GameObject.FindGameObjectWithTag("MainCamera");
        playerCamera[1] = GameObject.FindGameObjectWithTag("WildCamera"); 
        playerCamera[2] = GameObject.FindGameObjectWithTag("AttackCamera");

    }

    // Update is called once per frame
    void Update()
    {
        if (HSM.battleCamera)
        {
            transform.LookAt(transform.position + playerCamera[2].transform.rotation * Vector3.back, playerCamera[2].transform.rotation * Vector3.up);
        }
        else if (HSM.wildCamera)
        {
            transform.LookAt(transform.position + playerCamera[1].transform.rotation * Vector3.back, playerCamera[1].transform.rotation * Vector3.up);
        }

        else
        {
            transform.LookAt(transform.position + playerCamera[0].transform.rotation * Vector3.back, playerCamera[0].transform.rotation * Vector3.up);
        }
        
    }
}
