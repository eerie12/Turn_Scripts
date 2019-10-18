using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field_Potion : MonoBehaviour
{
    [SerializeField] private float rot_Speed;
    [SerializeField] private GameObject potion_Obj;
    [SerializeField] float currentCoolTime;
    [SerializeField] float maxCoolTime;
    private float start_Speed;
    private bool heal_Used;
    private bool rot_speed_back;

    // Start is called before the first frame update
    void Start()
    {
        start_Speed = rot_Speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (potion_Obj)
        {
            potion_Obj.transform.Rotate(Vector3.up * Time.deltaTime * rot_Speed);
            Rot_ReChargeTime();
            Rot_Recover();
        }
        

       
        //Debug.Log(heal_Used);
    }

    private void Rot_ReChargeTime()
    {
        if (heal_Used)
        {
            if (currentCoolTime < maxCoolTime)
                currentCoolTime++;
            else
                heal_Used = false;

        }
      
    }


    private void Rot_Recover()
    {
        if (!heal_Used && rot_Speed > start_Speed)
        {
            rot_speed_back = true;
            rot_Speed -= 10f;
        }
        else
        {
            rot_speed_back = false;

        }
    }


    public void DecreaseCooltime()
    {
        rot_Speed *= 10f;
        heal_Used = true;
        currentCoolTime = 0;


    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            
            if(start_Speed == rot_Speed)
            {
                DecreaseCooltime();
            }
            
        }
    }

}
