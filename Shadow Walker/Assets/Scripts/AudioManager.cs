using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Audio;
using System;

[System.Serializable]
public class Audio
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(-3f, 3f)]
    public float pitch = 1f;
    [Range(0f, 0.5f)]
    public float randomVolume = 0.1f;
    [Range(0f, 0.5f)]
    public float randomPitch = 0.1f;
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
            Debug.Log("Audio : " + aud.name + " not found");
            return;
        }
        aud.aS.volume = aud.volume * (1 + UnityEngine.Random.Range(-aud.randomVolume / 2f, aud.randomVolume / 2));
        aud.aS.pitch = aud.pitch * (1 + UnityEngine.Random.Range(-aud.randomPitch / 2f, aud.randomPitch / 2));
        if (!aud.aS.isPlaying)
        {
            aud.aS.Play();
        }
    }

    public void Mute(string p_name)
    {
        StartCoroutine(MuteSound(p_name));
    }

    IEnumerator MuteSound(string p_name)
    {
        Audio aud = Array.Find(soundFX, Audio => Audio.name == p_name);
        if (aud == null)
        {
            Debug.Log("Audio : " + name + " not found");
            yield break ;
        }
        float totalFadingTime = 0.5f;
        float currentFadingTime = 0;
        while (aud.aS.volume > 0)
        {
            currentFadingTime += Time.deltaTime;
            aud.aS.volume = Mathf.Lerp(1, 0, currentFadingTime / totalFadingTime);
            yield return null;
        }
        if(aud.aS.volume <= 0.01f)
        {
            Stop(p_name);
        }
    }

    public void Stop(string p_name)
    {
        Audio aud = Array.Find(soundFX, Audio => Audio.name == p_name);
        if (aud == null)
        {
            Debug.Log("Audio : " + name + " not found");
            return;
        }
        if (aud.aS.isPlaying)
        {
            aud.aS.Stop();
        }
        
    }

    //  FindObjectOfType<AudioManager>().Play("Name of the clip");
}
