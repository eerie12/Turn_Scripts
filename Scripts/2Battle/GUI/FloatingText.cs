using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{


    public float DestroyTime = 3f;
    public Vector3 Offset = new Vector3(0, 1.7f, 0);
    public Quaternion RotationOffset = Quaternion.Euler(0, -180, 0);
    public Quaternion RotationWildOffset = Quaternion.Euler(0, 0, 0);
    public Vector3 RandomizeIntensity = new Vector3(0.2f, 0, 0);
    HeroStateMachine HSM;


    // Start is called before the first frame update
    void Start()
    {
        HSM = FindObjectOfType<HeroStateMachine>();
        Destroy(gameObject, DestroyTime);
        transform.localPosition += Offset;
        transform.localPosition += new Vector3(Random.Range(-RandomizeIntensity.x,RandomizeIntensity.x),
        Random.Range(-RandomizeIntensity.y,RandomizeIntensity.y),
        Random.Range(-RandomizeIntensity.z,RandomizeIntensity.z));

        if (HSM.wildAtkStart && !HSM.wildStun)
        {           
            transform.localRotation = RotationWildOffset;
        }
        else
        {
            transform.localRotation = RotationOffset;
        }
        
    }

    
}
