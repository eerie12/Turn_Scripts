using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    public float speed;
    public Quaternion startRotation;
    HeroStateMachine HSM;

    // Start is called before the first frame update
    void Start()
    {
        startRotation = transform.rotation;
        HSM = GameObject.FindObjectOfType<HeroStateMachine>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!HSM.buffCameraReset)
        {
            transform.rotation = startRotation;
            HSM.buffCameraReset = true;
        }
        else
        {
           
            transform.Rotate(0, speed * Time.deltaTime, 0);
        }

    }

   
}
