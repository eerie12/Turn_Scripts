using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICamera : MonoBehaviour
{
    [SerializeField] private Camera[] basic_Camera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + basic_Camera[0].transform.rotation * Vector3.back, basic_Camera[0].transform.rotation * Vector3.up);
    }
}
