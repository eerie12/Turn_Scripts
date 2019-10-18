using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEventC : MonoBehaviour
{
    public GameObject[] eventOpject;
    public float eventSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < eventOpject.Length; i++)
        {
            eventOpject[i].SetActive(true);
            transform.Translate(Vector3.up * eventSpeed * Time.deltaTime);
            if(eventOpject[i].transform.position == new Vector3(eventOpject[i].transform.position.x, eventOpject[i].transform.position.y, eventOpject[i].transform.position.z))
            {
                eventSpeed = 0f;
            }

        }
    }
}
