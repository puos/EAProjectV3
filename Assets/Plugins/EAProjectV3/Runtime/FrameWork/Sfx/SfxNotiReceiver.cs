using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SfxNotiReceiver : MonoBehaviour, INotificationReceiver
{
    private EASfx sfx = null;
    public void Initialize(EASfx sfx)
    {
        this.sfx = sfx;
    }
    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (notification is SfxSignalMarker marker)
        {
            sfx.AnimEvent_Impact(marker.Param);
        }
    }
}
