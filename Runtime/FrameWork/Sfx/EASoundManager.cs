using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public enum EASOUND_TYPE { BGM , SFX, VOICE, AMBIENT }

public class EASoundManager : Singleton<EASoundManager>
{
    public const string bgmVolume = "BgmVolume";
    public const string sfxVolume = "SfxVolume";
    public const string voiceVolume = "voiceVolume";
    public const float bgmMaxVolume    = 100.0f;
    public const float sfxMaxVolume    = 100.0f;
    public const float voiceMaxVolume  = 100.0f;

    private AudioSource bgmAudio;
    private Queue<AudioSource> subAudios;
    private Queue<AudioSource> voiceAudios;
    private List<AudioSource> playSubAudio;
    private List<AudioSource> playVoiceAudio;

    private AudioMixer  audioMix;

    private Dictionary<string, AudioMixerGroup> dicAudioMixGroup;

    EABGMGroup bGMGroup = null;

    public override GameObject GetSingletonParent()
    {
        return EAMainFrame.instance.gameObject;
    }
    protected override void Initialize()
    {
        base.Initialize();

        if(bgmAudio == null) bgmAudio = gameObject.AddComponent<AudioSource>();
        if (dicAudioMixGroup == null) dicAudioMixGroup = new Dictionary<string, AudioMixerGroup>();
        bgmAudio.volume = GetVolume(EASOUND_TYPE.BGM);
        bgmAudio.playOnAwake = false;

        if (subAudios == null) subAudios = new Queue<AudioSource>();
        if (voiceAudios == null) voiceAudios = new Queue<AudioSource>();
        if (playSubAudio == null) playSubAudio = new List<AudioSource>();
        if (playVoiceAudio == null) playVoiceAudio = new List<AudioSource>();

        subAudios.Clear();
        for (int i = 0; i < 3; ++i) AddSubAudios();


        voiceAudios.Clear();
        for(int i = 0; i < 3; ++i) AddVoiceAudios();

        OptionManager.instance.RemoveListener(bgmVolume, ChangeBGMVolume);
        OptionManager.instance.AddListener(bgmVolume,ChangeBGMVolume);


        EAMainFrame.instance.OnMainFrameFacilityCreated(MainFrameAddFlags.SoundMgr);
    }

    protected override void Close()
    {
        base.Close();
        OptionManager.instance.RemoveListener(bgmVolume, ChangeBGMVolume);
    }

    private void ChangeBGMVolume()
    {
        if (bgmAudio == null) return;
        bgmAudio.volume = GetVolume(EASOUND_TYPE.BGM);
    }

    private void AddVoiceAudios()
    {
        var audio = gameObject.AddComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.loop = false;
        audio.volume = GetVolume(EASOUND_TYPE.VOICE);
        voiceAudios.Enqueue(audio);
    }

    private void AddSubAudios()
    {
        var audio = gameObject.AddComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.loop = false;
        audio.volume = GetVolume(EASOUND_TYPE.SFX);
        subAudios.Enqueue(audio);
    }

    private AudioSource GetSubAudios()
    {
        for(int i = 0; i < playSubAudio.Count; ++i)
        {
            var tmp = playSubAudio[i];
            if(tmp.isPlaying == false)
            {
                subAudios.Enqueue(tmp);
                playSubAudio.RemoveAt(i);
            }
        } 

        if (subAudios.Count <= 0) AddSubAudios();
        AudioSource audio = subAudios.Dequeue();
        playSubAudio.Add(audio);
        return audio;
    }

    private AudioSource GetVoiceAudios()
    {
        for(int i = 0; i < playVoiceAudio.Count; ++i)
        {
            var tmp = playVoiceAudio[i];
            if(tmp.isPlaying == false)
            {
                voiceAudios.Enqueue(tmp);
                playVoiceAudio.RemoveAt(i);
            }
        }
        if (voiceAudios.Count <= 0) AddVoiceAudios();
        AudioSource audio = voiceAudios.Dequeue();
        playVoiceAudio.Add(audio);
        return audio;
    }

    public float GetVolume(EASOUND_TYPE type)
    {
        float result = 1;
        if (type == EASOUND_TYPE.BGM) result = GetBGMVolumeNormalized();
        if (type == EASOUND_TYPE.SFX) result = GetSFXVolumeNormalized();
        if (type == EASOUND_TYPE.VOICE) result = GetVoiceVolumeNormalized();
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
    public float GetVoiceVolumeNormalized()
    {
        return OptionManager.instance.GetValueInRatio(voiceVolume, 0, voiceMaxVolume);
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
    public void LoadMixerGroup(string mixPath = "Sound/mix",string[] audioMixPath = null)
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
    public void Play(AudioSource source,EASOUND_TYPE type)
    {
        source.volume = GetVolume(type);
        if (source != null) source.Play();
    }
    public void PlaySFX(AudioClip clip, EASOUND_TYPE type , bool loop = false ,string mixKey = "Master" )
    {
        if (clip == null) return;

        if(type == EASOUND_TYPE.SFX)
        {
            AudioSource audio = GetSubAudios();
            audio.clip = clip;
            audio.loop = loop;
            if (dicAudioMixGroup.TryGetValue(mixKey, out AudioMixerGroup mix)) audio.outputAudioMixerGroup = mix;
            Play(audio, type);
        }
        if(type == EASOUND_TYPE.VOICE)
        {
            AudioSource audio = GetVoiceAudios();
            audio.clip = clip;
            audio.loop = loop;
            if (dicAudioMixGroup.TryGetValue(mixKey, out AudioMixerGroup mix)) audio.outputAudioMixerGroup = mix;
            Play(audio, type);
        }
    }
    
     // play bgm
    public void PlayBGM(string name,string mixKey = "Master/Main/BGM")
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
            if (dicAudioMixGroup.TryGetValue(mixKey, out AudioMixerGroup mix)) bgmAudio.outputAudioMixerGroup = mix;
            Play(bgmAudio, EASOUND_TYPE.BGM);
            return;
        }

        if (bgmAudio.isPlaying)
        {
            if (bgmAudio.clip.name.Equals(clip.name, StringComparison.Ordinal)) return;
        }

        bgmAudio.clip = clip;
        bgmAudio.loop = slot.loop;
        if (dicAudioMixGroup.TryGetValue(mixKey, out AudioMixerGroup mix2)) bgmAudio.outputAudioMixerGroup = mix2;
        Play(bgmAudio, EASOUND_TYPE.BGM);
    }
   
    public void StopBGM()
    {
        bgmAudio.Stop();
        bgmAudio.clip = null;
    }
    public void StopSfxSound()
    {
        for(int i = 0; i < playSubAudio.Count; ++i)
        {
            if (playSubAudio[i].isPlaying) playSubAudio[i].Stop();
        }
        
        for(int i = 0; i < playVoiceAudio.Count; ++i)
        {
            if (playVoiceAudio[i].isPlaying) playVoiceAudio[i].Stop();
        }
    }
    public void ChangeMixSnapShot(string scene)
    {
        if (audioMix == null) return;
        AudioMixerSnapshot snapShot = audioMix.FindSnapshot(scene);
        if (snapShot != null) audioMix.TransitionToSnapshots(new AudioMixerSnapshot[] { snapShot }, new float[] { 1f }, Time.deltaTime);
        else
        {
            snapShot = audioMix.FindSnapshot("Snapshot");
            audioMix.TransitionToSnapshots(new AudioMixerSnapshot[] { snapShot }, new float[] { 1f }, Time.deltaTime);
        }
    }
}
