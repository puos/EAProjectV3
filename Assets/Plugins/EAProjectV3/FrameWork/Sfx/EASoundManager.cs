using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum EASOUND_TYPE { BGM , SFX, UI,VOICE, AMBIENT }

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

    EABGMGroup bGMGroup = null;

    // BGM , SFX volume
    private Action<float, float> onChangeVolume;

    public override GameObject GetSingletonParent()
    {
        return EAMainFrame.instance.gameObject;
    }
    protected override void Initialize()
    {
        base.Initialize();

        if(mainAudio == null)
        {
            mainAudio = gameObject.AddComponent<AudioSource>();
            subAudio = gameObject.AddComponent<AudioSource>();
            mainAudioOriVolume = mainAudio.volume;
            mainAudio.volume = GetVolume(EASOUND_TYPE.BGM) * mainAudio.volume;
        }

        EAMainFrame.instance.OnMainFrameFacilityCreated(MainFrameAddFlags.SoundMgr);
    }

    // Once the volume has been modified it can be called to call back
    public void OnAddListner_OnChangeVolume(Action<float,float> action)
    {
        onChangeVolume -= action;
        onChangeVolume += action;
    }
    public void RemoveListener_OnChangeVolume(Action<float,float> action)
    {
        onChangeVolume -= action;
    }
    public float GetVolume(EASOUND_TYPE type)
    {
        float result = 1;
        if (type == EASOUND_TYPE.BGM) result = GetBGMVolumeNormalized();
        if (type == EASOUND_TYPE.SFX) result = GetSFXVolumeNormalized();
        return result;
    }
    public float GetBGMVolumeNormalized() 
    {
        return 1f;
    }
    public float GetSFXVolumeNormalized() 
    {
        return 1f;
    }
    public void LoadBGM(string bgmPath)
    {
        if (bGMGroup != null) return;

        GameObject obj = GameResourceManager.instance.Load<GameObject>(bgmPath);

        if (obj == null) return;

        GameObject bgm = Instantiate(obj) as GameObject;

        bgm.name = "BGMGroup";
        bgm.transform.SetParent(transform);
        bgm.transform.localScale = Vector3.zero;
        bgm.transform.localPosition = Vector3.zero;

        bGMGroup = bgm.GetComponent<EABGMGroup>();
    }
    public void Play(AudioSource source,float desiredVolume,EASOUND_TYPE type)
    {
        SetVolume(source, desiredVolume, type);
        if (source != null) source.Play();
    }
    public void SetVolume(AudioSource source,float desiredVolume,EASOUND_TYPE type)
    {
        if (source != null) source.volume = GetVolume(type) * desiredVolume;
    }
}
