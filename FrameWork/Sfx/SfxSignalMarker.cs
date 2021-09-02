using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SfxSignalMarker : Marker, INotification
{
    [SerializeField] private string parameter;

    public string Param => parameter;
    public PropertyName id => new PropertyName();
}
