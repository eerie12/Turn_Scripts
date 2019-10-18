using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBattle : MonoBehaviour
{
    public Transform attackCam;
    public Vector3 startCamPosition;
    public Transform camTarget;

    // Start is called before the first frame update
    void Start()
    {
        startCamPosition = new Vector3(attackCam.position.x, attackCam.position.y, attackCam.position.z);
        transform.position = startCamPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(camTarget);
    }
}
