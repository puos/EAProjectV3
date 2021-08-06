using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCue : MonoBehaviour
{
    protected AudioSource audioSouce = null;
    public int clipIdx = 0;

    [SerializeField] private AudioClip[] audioClip = null;

    public void Initialize()
    {
        audioSouce = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        audioSouce.PlayOneShot(audioClip[clipIdx], 1f);
    }

}
