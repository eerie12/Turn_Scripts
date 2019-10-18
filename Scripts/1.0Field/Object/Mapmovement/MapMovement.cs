using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMovement : MonoBehaviour
{
    [Range(1,10)]
    [SerializeField] float MapSpeed;

    [SerializeField] float maxX;

    [SerializeField] float minX;
    //[SerializeField] float delaytime;
    private int sign = 1;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * MapSpeed * Time.deltaTime*sign);

        if(transform.position.x <=minX || transform.position.x >= maxX)
        {
            sign *= -1;                        
            
        }
    }
    


}
