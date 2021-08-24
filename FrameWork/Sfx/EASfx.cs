﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SfxSignalEmit : ParameterizedEmitter<string> { }

public class ParameterizedEmitter<T> : SignalEmitter
{ public T parameter; }

public interface SfxParamSlotListener
{
    void OnSfxParamSlotAssigned(int slotIdx, EASfx.SfxParamSlot slot);
}

public class NotiReciever : MonoBehaviour , INotificationReceiver
{
    private EASfx sfx = null;
    public void Initialize(EASfx sfx)
    { this.sfx = sfx; }

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if(notification is ParameterizedEmitter<string> emitter)
        {
            
        }
    }
}

public enum SfxEventType
{
    None,
    Impact,
    Decay,
    Destroy,
}

public delegate void SfxEventCallback(EASfx sfx, SfxEventType eventType, string slotName);

public delegate void SfxParamSlotCallback(GameObject targetObj);

public class EASfx : MonoBehaviour
{
    public enum SfxCategory
    {
        sfxCommon = 1,
        sfxWorld  = 2,
        sfxUi     = 3,
    }

    public enum SfxType
    {
        sfxTypeParticles,
        sfxTypeTimeLine,
        sfxTypeAnimator,
    }

    [System.Serializable]
    public class SfxParamSlot
    {
        public string name;
        public SfxParamSlotCallback callback; // Callback to handle connection
        public GameObject innerObj;           // [Optional] Internal Object to connect
    }

    private SfxCategory sfxCategory = SfxCategory.sfxCommon;

    [SerializeField] private SfxParamSlot[] m_paramSlots;
    [SerializeField] private PlayableDirector[] m_anims = null;

    private int timeLineId = 0;

    [SerializeField] private Animator m_anim = null;
    [SerializeField] private string animationName = string.Empty;
    [SerializeField] private ParticleSystem[] m_particles = null;
    [SerializeField] SfxType m_sfxType = SfxType.sfxTypeParticles;
    [System.NonSerialized] public SfxEventCallback eventCallback;

    public void StartFx()
    {
        switch(m_sfxType)
        {
            case SfxType.sfxTypeTimeLine: StartFxTimeLine(); break;
            case SfxType.sfxTypeParticles: StartParticles();  break;
            case SfxType.sfxTypeAnimator: StartAnimator();  break;
        }
    }

    public void StopFx() 
    {
        switch(m_sfxType)
        {
            case SfxType.sfxTypeTimeLine: StartFxTimeLine(false); break;
            case SfxType.sfxTypeParticles: StartParticles(false); break;
            case SfxType.sfxTypeAnimator: StartAnimator(false); break;
        }
    }

    public void SkipFx()
    {
        switch(m_sfxType)
        {
            case SfxType.sfxTypeTimeLine: break;
            case SfxType.sfxTypeParticles: break;
            case SfxType.sfxTypeAnimator: break;
        }
    }

    private void StartFxTimeLine(bool start = true)
    {
        int index = timeLineId;

        if (m_anims == null) m_anims = GetComponentsInChildren<PlayableDirector>();
        if (m_anims == null) return;

        if(start == false)
        {
            if (m_anims.Length > index) m_anims[index].Stop();
            return;
        }

        for(int i = 0; i < m_anims.Length; ++i)
        {
            NotiReciever receiver = m_anims[i].GetComponent<NotiReciever>();
            if (receiver == null) receiver = m_anims[i].gameObject.AddComponent<NotiReciever>();
            receiver.Initialize(this);
        }

        if (m_anims.Length > index) m_anims[index].Play();
    }
    private void StartParticles(bool start = true)
    {
        for(int i = 0; i < m_particles.Length; ++i)
        {
            ParticleSystem s = m_particles[i];
            if (s == null) continue;

            if (start == true) s.Play();
            if (start == false) s.Stop();
        }
    }
    private void StartAnimator(bool start = true)
    {
        if (m_anim == null) m_anim = gameObject.GetComponentInChildren<Animator>();
        if (m_anim == null) return;

        if(start == false)
        {
            m_anim.Rebind();
            return;
        }

        m_anim.Rebind();
        m_anim.speed = 1;
        m_anim.Play(animationName);
    }
    public void AnimEvent_Impact(string iter)
    {
        SendEventToOwner(SfxEventType.Impact, iter);
    }
    void SendEventToOwner(SfxEventType type,string param)
    {
        if (eventCallback != null) eventCallback(this, type, param);
    }
    public void SetParam(int slotIdx,SfxParamSlotCallback cb)
    {
        if(slotIdx >= 0 && m_paramSlots != null && slotIdx < m_paramSlots.Length)
        {
            SfxParamSlot slot = m_paramSlots[slotIdx];
            slot.callback = cb;
            if (slot.innerObj != null) cb(slot.innerObj);
        }
    }
}
