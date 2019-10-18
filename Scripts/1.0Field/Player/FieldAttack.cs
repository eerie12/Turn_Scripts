using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldAttack : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "FieldEnemy" || other.gameObject.tag == "FieldEnemyB")
        {
            SoundManager.instance.PlaySound("Sturn", 1);
            other.gameObject.GetComponent<FieldEnemy>().Damage(1,transform.position);
        }
    }
}
