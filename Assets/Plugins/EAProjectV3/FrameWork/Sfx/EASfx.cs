using System;
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

public class EASfx
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
}
