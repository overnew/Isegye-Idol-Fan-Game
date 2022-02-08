using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private enum Sound
    {
        Effect
    }

    private float generalVolum = 0.5f;

    private AudioSource[] audioSourceList;
    public void Awake()
    {
        audioSourceList = gameObject.GetComponentsInChildren<AudioSource>();
    }

    public void Play(AudioClip audioClip)
    {
        AudioSource audioSource = audioSourceList[(int)Sound.Effect];
        audioSource.volume = generalVolum;
        audioSource.clip = audioClip;
        audioSource.Play();
    }

}
