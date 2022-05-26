using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUI : UICtrl
{
    [SerializeField] private UITween_TweenRuntime m_Tween = null;
    [SerializeField] private UITween_TweenRuntime m_Tween3 = null;
    [SerializeField] private EASfx uiSfx = null;

    public override void Initialize()
    {
        base.Initialize();
        m_Tween.enabled = false;
        m_Tween3.enabled = false;
    }

    public void OnClickEvent()
    {
       Debug.Log("TestUI OnClickEvent");
        m_Tween.enabled = true;
        m_Tween.Play();
    }

    public void OnClickEvent2() 
    {
        if (uiSfx.IsAlive()) return;

        Debug.Log("TestUI OnClickEvent2");

        uiSfx.StartFx();
        uiSfx.eventCallback = (EASfx sfx, SfxEventType eventType, string slotName) => 
        {
            if(string.Equals(slotName,"0")) Debug.Log("TestUI OnClickComplete2");
        };
    }

    public void OnClickEvent3()
    {
        if (m_Tween3.isPlaying) return;

        Debug.Log("TestUI OnClickEvent3");
        m_Tween3.enabled = true;
        m_Tween3.Play();
    }

    public void OnClickComplete()
    {
        Debug.Log("TestUI OnClickComplete");
    }

    public void OnClickComponent3()
    {
        Debug.Log("TestUI OnClickComplete3");
    }
}
