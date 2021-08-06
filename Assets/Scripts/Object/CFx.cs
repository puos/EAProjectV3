using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFx : EAObject
{
    [SerializeField] private SoundCue soundCue = null;
    [SerializeField] private ParticleSystem particle = null;

    public override void Initialize()
    {
        base.Initialize();
        if (soundCue != null) soundCue.Initialize();
    }

    public void PlaySound(int index = 0) 
    {
        if (soundCue == null) return;

        soundCue.clipIdx = index;
        soundCue.PlaySound();
    }

    public void StartFx()
    {
        if (particle != null) particle.Play(true);
    }

    public override void Release()
    {
        base.Release();
        if (particle != null) particle.Stop(true);
    }
}
