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

        EASoundManager.instance.PlaySFX(audioClip[curPlayIdx], EASOUND_TYPE.SFX);
    }
    private void PlayRandom()
    {
        int randomIdx = UnityEngine.Random.Range(0, audioClip.Count);
        Debug.Log("PlayOne Random idx :" + randomIdx);
        EASoundManager.instance.PlaySFX(audioClip[randomIdx], EASOUND_TYPE.SFX);
    }
    private void PlayAll() 
    {
        for(int i = 0;i < audioClip.Count; ++i)
        {
            EASoundManager.instance.PlaySFX(audioClip[i], EASOUND_TYPE.SFX);
        }
    }
}
