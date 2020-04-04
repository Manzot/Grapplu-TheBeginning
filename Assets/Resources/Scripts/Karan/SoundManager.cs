using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public SoundManager() { }
    public static SoundManager Instance { get { return instance ?? (instance = new SoundManager()); } }
    PlayerController player;

    public Sound[] sounds;

    public void Initialize()
    {
        sounds = new Sound[5];
        foreach (Sound s in sounds)
        {
            if (this)
            {
                s.source = this.gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
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
}