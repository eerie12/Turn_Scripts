using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Menu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume",volume);
    }
    public void SE(float volume)
    {
        audioMixer.SetFloat("SE",volume);
    }
    public void BGM(float volume)
    {
        audioMixer.SetFloat("BGM", volume);
    }

}
