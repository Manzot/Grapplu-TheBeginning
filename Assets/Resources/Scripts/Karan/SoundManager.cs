using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Singleton
    public static SoundManager Instance = null;
    #endregion

    public Sound[] sounds;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            if (this)
            {
                s.source = this.gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.loop = s.loop;
            }

        }
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