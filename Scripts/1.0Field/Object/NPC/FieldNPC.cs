using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldNPC : MonoBehaviour
{
    PlayerControllerrbody playerController;

    [SerializeField] private Transform player;
    [SerializeField] private GameObject quest_Mark;
    private Quaternion startRotation;
    private Quaternion destination_Rotation;

    //[SerializeField] private float m_spinSpeed = 0f;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerrbody>();
        startRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.eventStart && playerController.storyEvent && GameManager.instance.eventFlags[0])
        {
            if (quest_Mark)
            {
                if (quest_Mark.activeSelf)
                {
                    quest_Mark.SetActive(false);
                    GameManager.instance.fieldMonName.Add(quest_Mark.name);
                }

            }
            Vector3 dir = player.position - transform.position;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z)), 20f * Time.deltaTime * 5);






        }
        else
        {
            //transform.LookAt(null);
            //transform.rotation = startRotation;
            StartMoveRotation();
        }

    }


    private bool MoveTowardsEnemy(Vector3 target, float rot, float speed)
    {

        Vector3 dir = target - transform.position;//方向を求める
        dir = new Vector3(dir.x - 1.2f, dir.y, dir.z);//方向は敵      
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rot * Time.deltaTime);
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime));

    }

    private void StartMoveRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, startRotation, 20f * Time.deltaTime * 5);
    }
}
