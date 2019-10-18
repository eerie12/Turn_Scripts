using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest＿Jewel : MonoBehaviour
{
    public GameObject pickupEffect;
    public GameObject eventObject;
    public GameObject eventParticle;
    public GameObject warp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.instance.fieldMonName.Add(gameObject.name);
            //FindObjectOfType<GameManager>().AddGold(value);

            Instantiate(pickupEffect, transform.position, transform.rotation);
            Destroy(eventObject);
            Destroy(eventParticle);
            QuestManager.questManager.AddQuestItem("Jewel", 1);
            QuestObject[] currentQuestGuys = FindObjectsOfType(typeof(QuestObject)) as QuestObject[];
            GameManager.instance.eventStartCheck = true;
            foreach (QuestObject obj in currentQuestGuys)
            {
                obj.SetQuestMaker();
            }
            Invoke("WarpOn", 0.5f);
            Destroy(gameObject, 1f);
        }
    }
    private void WarpOn()
    {
        warp.SetActive(true);
    }
}
