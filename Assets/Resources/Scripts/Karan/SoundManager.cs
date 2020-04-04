using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Singleton
    private static SoundManager instance=null;
    public SoundManager() { }
    public static SoundManager Instance { get { return instance ?? (instance = FindObjectOfType<SoundManager>()); } }

    #endregion

    public Sound[] sounds;

    public void Initialize()
    {
   
        foreach (Sound s in sounds)
        {
            if (this)
            {
                s.source = this.gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.loop = s.loop;
            }

        }

    }
    public void PostInitialize()
    {
        Play("Theme");
    }
    public void Play(string name)
    {
        if (this)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            s.source.pitch = s.pitch;
            s.source.volume = s.volume;
            if (s == null)
            {
                Debug.Log("Couldn't Find the Sound");
                return;
            }
            s.source.Play();
        }
    }
    public void StopPlaying(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Stop();
    }
}