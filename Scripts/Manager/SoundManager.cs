using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] Sound[] effectSounds;
    [SerializeField] AudioSource[] effectPlayer;

    [SerializeField] Sound[] bgmSounds;
    [SerializeField] AudioSource bgmPlayer;

    //[SerializeField] AudioSource voicePlayer;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void PlayBGM(string p_name)
    {
        for(int i = 0; i< bgmSounds.Length; i++)
        {
            if(p_name == bgmSounds[i].name)
            {
                bgmPlayer.clip = bgmSounds[i].clip;
                bgmPlayer.Play();
                if (bgmSounds[i].name == "Victory" || bgmSounds[i].name == "Defeat")
                {
                    bgmPlayer.loop = false;
                }
                else
                {
                    bgmPlayer.loop = true;
                }
                return;
            }
        }
        Debug.LogError(p_name + "に該当するbgmがありません");
    }

    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

    void PauseBGM()
    {
        bgmPlayer.Pause();
    }

    void UnPauseBGM()
    {
        bgmPlayer.UnPause();
    }

    void PlayEffectSound(string p_name)
    {
        
        for(int i = 0; i < effectSounds.Length; i++)
        {
            if(p_name == effectSounds[i].name)
            {
                for(int j = 0; j < effectPlayer.Length; j++)
                {
                    if (!effectPlayer[j].isPlaying)
                    {
                        effectPlayer[j].clip = effectSounds[i].clip;
                        effectPlayer[j].Play();
                        return;
                    }
                }
                Debug.LogError("すべてのplayerが使用中です。");
                return;
            }
        }
        Debug.LogError(p_name + "に該当するseがありません");
    }

    void PlayEffectSoundShot(string p_name)
    {

        for (int i = 0; i < effectSounds.Length; i++)
        {
            if (p_name == effectSounds[i].name)
            {
                for (int j = 0; j < effectPlayer.Length; j++)
                {
                    if (!effectPlayer[j].isPlaying)
                    {
                        effectPlayer[j].clip = effectSounds[i].clip;
                        effectPlayer[j].Play();
                        //Debug.Log("play");
                        return;
                    }
                    else
                    {
                        effectPlayer[j].clip = effectSounds[i].clip;
                        effectPlayer[j].PlayOneShot(effectSounds[i].clip);
                        //Debug.Log("play");
                        return;
                    }
                }
                Debug.LogError("すべてのplayerが使用中です。");

                return;
            }
        }
        Debug.LogError(p_name + "に該当するseがありません");
    }





    public void StopAllEffectSound()
    {
        for(int i = 0; i < effectPlayer.Length; i++)
        {
            effectPlayer[i].Stop();
        }
    }

    ////
    //p_Type : 0 -> bgm再生
    //p_Type : 1 -> se再生
    ////
    public void PlaySound(string p_name,int p_Type)
    {
        if (p_Type == 0)PlayBGM(p_name);
        else if (p_Type == 1)PlayEffectSound(p_name);
        else if (p_Type == 2) PlayEffectSoundShot(p_name);



    }



    
}
