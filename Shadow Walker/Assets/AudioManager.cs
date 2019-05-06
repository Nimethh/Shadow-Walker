using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Audio
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(-3f, 3f)]
    public float pitch = 1f;
    public bool loop = false;
    public bool playOnAwake = false;
    [HideInInspector]
    public AudioSource aS;
}

public class AudioManager : MonoBehaviour
{
    public Audio[] soundFX;

    void Start()
    {
        Play("Background");
    }

    void Awake()
    {
        foreach (Audio aud in soundFX)
        {
            aud.aS = gameObject.AddComponent<AudioSource>();
            aud.aS.clip = aud.clip;
            aud.aS.volume = aud.volume;
            aud.aS.loop = aud.loop;
            aud.aS.pitch = aud.pitch;
            aud.aS.playOnAwake = aud.playOnAwake;
        }
    }

    public void Play(string p_name)
    {
        Audio aud = Array.Find(soundFX, Audio => Audio.name == p_name);
        if (aud == null)
        {
            Debug.Log("Audio : " + name + " not found");
            return;
        }
        aud.aS.Play();
    }

    //  FindObjectOfType<AudioManager>().Play("Name of the clip");
}
