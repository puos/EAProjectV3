using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EASoundManager : Singleton<EASoundManager>
{
    public const string bgmVolume = "BgmVolume";
    public const string sfxVolume = "SfxVolume";
    public const float bgmMaxVolume = 100.0f;
    public const float sfxMaxVolume = 100.0f;

    private AudioSource mainAudio;
    private AudioSource subAudio;
    float mainAudioOriVolume;
    AudioClip defaultClip;

    AudioLowPassFilter lowPassFilter = null;

    public override GameObject GetSingletonParent()
    {
        return EAMainFrame.instance.gameObject;
    }

    protected override void Initialize()
    {
        base.Initialize();

        EAMainFrame.instance.OnMainFrameFacilityCreated(MainFrameAddFlags.SoundMgr);
    }
}
