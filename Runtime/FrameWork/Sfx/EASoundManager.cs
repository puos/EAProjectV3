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
    
    AudioLowPassFilter lowPassFilter = null;

    readonly private float lowPassDefault  = 22000f;
    readonly private float lowPassLowValue = 7000.0f;
    readonly private float lowPasLowTime = 2.3f;

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
            lowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();
            lowPassFilter.cutoffFrequency = 5000.0f;
            
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
        return OptionManager.instance.GetValueInRatio(bgmVolume, 0,bgmMaxVolume);
    }
    public float GetSFXVolumeNormalized() 
    {
        return OptionManager.instance.GetValueInRatio(sfxVolume, 0,sfxMaxVolume);
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
    public void PlayOneShot(AudioClip clip, float volume , EASOUND_TYPE type)
    {
        if (clip == null) return;
        if (subAudio == null) return;
        SetVolume(subAudio, volume, type);
        subAudio.PlayOneShot(clip, GetVolume(type));
    }
    public void SetVolume(AudioSource source,float desiredVolume,EASOUND_TYPE type)
    {
        if (source != null) source.volume = GetVolume(type) * desiredVolume;
    }

    // play bgm
    public void PlayBGM(string name,bool useLowPassFilter = false)
    {
        if (bGMGroup == null) return;

        EABGMGroup.BGMSlot slot = bGMGroup.GetBGM(name);

        if (slot == null) return;
        if (slot.audioClip == null) return;

        AudioClip clip = slot.audioClip;

        if(mainAudio.clip == null)
        {
            mainAudio.clip = clip;
            mainAudio.loop = slot.loop;
            lowPassFilter.cutoffFrequency = (useLowPassFilter == true) ? lowPassLowValue : lowPassDefault;
            Play(mainAudio, slot.volume, EASOUND_TYPE.BGM);
            return;
        }

        if (mainAudio.isPlaying)
        {
            if (mainAudio.clip.name.Equals(clip.name, StringComparison.Ordinal)) return;
        }

        mainAudio.clip = clip;
        mainAudio.loop = slot.loop;
        lowPassFilter.cutoffFrequency = (useLowPassFilter == true) ? lowPassLowValue : lowPassDefault;
        Play(mainAudio, slot.volume, EASOUND_TYPE.BGM);
    }
    public void SetLowPassFilter(bool useLowPassFilter,float desiredVolume = 1.0f)
    {
        float frequency = (useLowPassFilter == true) ? lowPassLowValue : lowPassDefault;

        float numFrom = lowPassFilter.cutoffFrequency;
        float numTo = frequency;

        SetVolume(mainAudio, desiredVolume, EASOUND_TYPE.BGM);

        if(useLowPassFilter == true)
        {
            lowPassFilter.cutoffFrequency = frequency;
            return;
        }

        var tweener = EANumberTween.Start(numFrom, numTo, 0, lowPasLowTime);
        tweener.onUpdate = (EANumberTween.Event e) => { lowPassFilter.cutoffFrequency = e.number; };
        tweener.onComplete = (EANumberTween.Event e) => { lowPassFilter.cutoffFrequency = frequency; };
    }

    public void StopBGM()
    {
        mainAudio.Stop();
        mainAudio.clip = null;
    }
    public void StopSfxSound()
    {
        subAudio.Stop();
        subAudio.clip = null;
    }
}
