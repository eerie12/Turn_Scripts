using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateChange : MonoBehaviour
{
    private PlayerControllerrbody PlayerController;
    private FieldCameraController theEventCam;
    public CapsuleCollider SwordColider;

    [SerializeField] private ParticleSystem fieldATK_Particle;
    void Start()
    {
      PlayerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerrbody>();
      theEventCam = FindObjectOfType<FieldCameraController>();
      SwordColider.enabled = false;

    }
    void PlayerStateChange()
    {
        PlayerController.StateMove();

    }

    void FieldATK_ParticleOn()
    {
        fieldATK_Particle.Play();
        SoundManager.instance.PlaySound("Slash1", 1);
    }

    void FieldATK_Start()
    {
        SwordColider.enabled = true;

       
    }

    void FieldATK_End()
    {
        SwordColider.enabled = false;
        fieldATK_Particle.Clear();
    }

    void WalkSound()
    {
        //if (theEventCam.camEvent)
        //{
            SoundManager.instance.PlaySound("FootSound", 1);
        //}
     
        
    }
}
