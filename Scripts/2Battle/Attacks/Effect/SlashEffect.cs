using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem[] slashParticle;

    void ATK1_Particle_Start()
    {
        slashParticle[0].Play();
        SoundManager.instance.PlaySound("Slash1", 1);
    }

    void ATK1_Particle_End()
    {
        //SoundManager.instance.StopAllEffectSound();
        slashParticle[0].Stop();
        slashParticle[0].Clear();
    }

    void ATK2_Particle_Start()
    {
        slashParticle[1].Play();
        SoundManager.instance.PlaySound("Slash1", 1);

    }

    void ATK2_Particle_End()
    {
        //SoundManager.instance.StopAllEffectSound();
        slashParticle[1].Stop();
        slashParticle[1].Clear();
    }

    void ATK3_Particle_Start()
    {
        slashParticle[2].Play();
        //SoundManager.instance.PlaySound("Slash3", 1);
    }

    void ATK3_Particle_End()
    {
        //SoundManager.instance.StopAllEffectSound();
        slashParticle[2].Stop();
        slashParticle[2].Clear();
    }

    void CriticalSound()
    {
        SoundManager.instance.PlaySound("Slash1", 1);
    }

    void Dead_SoundStop()
    {
        SoundManager.instance.StopBGM();
        Invoke("Dead_EndSound",2.5f);
    }

    private void Dead_EndSound()
    {
        SoundManager.instance.PlaySound("Defeat", 0);
    }

}
