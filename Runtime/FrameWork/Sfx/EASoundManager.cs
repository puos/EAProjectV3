using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public enum EASOUND_TYPE { BGM , SFX, UI,VOICE, AMBIENT }

public class EASoundManager : Singleton<EASoundManager>
{
    public const string bgmVolume = "BgmVolume";
    public const string sfxVolume = "SfxVolume";
    public const float bgmMaxVolume = 100.0f;
    public const float sfxMaxVolume = 100.0f;

    private AudioSource bgmAudio;
    private AudioSource subAudio;
    private AudioMixer audioMix;

    private Dictionary<string, AudioMixerGroup> dicAudioMixGroup;

    float bgmAudioOriVolume;
      
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

        if(bgmAudio == null) bgmAudio = gameObject.AddComponent<AudioSource>();
        if(subAudio == null) subAudio = gameObject.AddComponent<AudioSource>();
        if (dicAudioMixGroup == null) dicAudioMixGroup = new Dictionary<string, AudioMixerGroup>();
        bgmAudioOriVolume = bgmAudio.volume;
        bgmAudio.volume = GetVolume(EASOUND_TYPE.BGM) * bgmAudio.volume;
        bgmAudio.playOnAwake = false;
        subAudio.playOnAwake = false;

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
    public void LoadBGM(string bgmPath = "Sound/BGM/BGMGroup")
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
    public void LoadMixerGroup(string mixPath = "Sound/Mix",string[] audioMixPath = null)
    {
        if (audioMix != null) return;

        dicAudioMixGroup.Clear();
        audioMix = GameResourceManager.instance.Load<AudioMixer>(mixPath);
        AudioMixerGroup[] mixGroup = audioMix.FindMatchingGroups("Master");
        if (mixGroup.Length > 0) dicAudioMixGroup.Add("Master", mixGroup[0]);
        if (audioMixPath == null) return;
        for(int i = 0; i < audioMixPath.Length; ++i)
        {
            var mix = audioMix.FindMatchingGroups(audioMixPath[i]);
            if(mix != null)
            {
                Debug.Log("success mixPath : " + audioMixPath[i]);
                dicAudioMixGroup.Add(audioMixPath[i], mix[0]);
            }
        } 
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
    public void PlayBGM(string name)
    {
        if (bGMGroup == null) LoadBGM();
        if (bGMGroup == null) return;

        EABGMGroup.BGMSlot slot = bGMGroup.GetBGM(name);

        if (slot == null) return;
        if (slot.audioClip == null) return;

        AudioClip clip = slot.audioClip;

        if(bgmAudio.clip == null)
        {
            bgmAudio.clip = clip;
            bgmAudio.loop = slot.loop;
            Play(bgmAudio, slot.volume, EASOUND_TYPE.BGM);
            return;
        }

        if (bgmAudio.isPlaying)
        {
            if (bgmAudio.clip.name.Equals(clip.name, StringComparison.Ordinal)) return;
        }

        bgmAudio.clip = clip;
        bgmAudio.loop = slot.loop;
        Play(bgmAudio, slot.volume, EASOUND_TYPE.BGM);
    }
   
    public void StopBGM()
    {
        bgmAudio.Stop();
        bgmAudio.clip = null;
    }
    public void StopSfxSound()
    {
        subAudio.Stop();
        subAudio.clip = null;
    }
}
