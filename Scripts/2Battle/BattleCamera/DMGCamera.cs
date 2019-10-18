using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMGCamera : MonoBehaviour
{
    public Camera MainCMA;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = MainCMA.transform.position;
        transform.rotation = MainCMA.transform.rotation;
    }
}
