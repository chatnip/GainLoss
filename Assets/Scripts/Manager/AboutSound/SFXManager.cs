using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    [Header("*Clip")]
    [SerializeField] AudioClip clip_1;
    [SerializeField] AudioClip clip_2;
    [SerializeField] AudioClip clip_3;
    [SerializeField] AudioClip clip_4;
    [SerializeField] AudioClip clip_5;

    public void AudioPlay(int value)
    {
        switch (value)
        {
            case 1:
                audioSource.clip = clip_1;
                audioSource.Play();
                break;
            case 2:
                audioSource.clip = clip_2;
                audioSource.Play();
                break;
            case 3:
                audioSource.clip = clip_3;
                audioSource.Play();
                break;
            case 4:
                audioSource.clip = clip_4;
                audioSource.Play();
                break;
            case 5:
                audioSource.clip = clip_5;
                audioSource.Play();
                break;
            default:
                break;
        }

    }
}
