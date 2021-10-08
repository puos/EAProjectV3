using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[ExecuteInEditMode]
public class EASoundCue : MonoBehaviour
{
    public enum eMethod { PlayOne , Random , PlayAll }

    [Header("Setting Sound Source")]
    [SerializeField] private List<AudioClip> audioClip = new List<AudioClip>();

    [HideInInspector] public int curPlayIdx = 0;

    [Header("Setting Sound Play")]
    [Range(-3.0f, 3.0f)]
    public float pitch = 1.0f;
    [Range(0.0f, 1.0f)]
    public float volume = 1.0f;
    public eMethod method = eMethod.PlayOne;

    public void PlaySound()
    {
        switch(method)
        {
            case eMethod.PlayOne: PlayOne(); break;
            case eMethod.Random: PlayRandom(); break;
            case eMethod.PlayAll: break;
        }
    }
    private void PlayOne() 
    {
        if (curPlayIdx >= audioClip.Count) return;
        if (curPlayIdx < 0) return;

        Debug.Log("PlayOne idx :" + curPlayIdx);

        EASoundManager.instance.PlayOneShot(audioClip[curPlayIdx], volume, EASOUND_TYPE.SFX);
    }
    private void PlayRandom()
    {
        int randomIdx = UnityEngine.Random.Range(0, audioClip.Count);
        Debug.Log("PlayOne Random idx :" + randomIdx);
        EASoundManager.instance.PlayOneShot(audioClip[randomIdx], volume, EASOUND_TYPE.SFX);
    }
    private void PlayAll() 
    {
        for(int i = 0;i < audioClip.Count; ++i)
        {
            EASoundManager.instance.PlayOneShot(audioClip[i], volume, EASOUND_TYPE.SFX);
        }
    }
}
