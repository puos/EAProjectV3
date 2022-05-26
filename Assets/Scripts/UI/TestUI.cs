using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUI : UICtrl
{
    [SerializeField] private UITween_TweenRuntime m_Tween = null;
    [SerializeField] private UITween_TweenRuntime m_Tween2 = null;
    [SerializeField] private UITween_TweenRuntime m_Tween3 = null;

    public override void Initialize()
    {
        base.Initialize();
        m_Tween.enabled = false;
        m_Tween2.enabled = false;
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
        if (m_Tween2.isPlaying) return;
        
        Debug.Log("TestUI OnClickEvent2");
        m_Tween2.enabled = true;
        m_Tween2.Play();
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

    public void OnClickComponent2()
    {
        Debug.Log("TestUI OnClickComplete2");
    }

    public void OnClickComponent3()
    {
        Debug.Log("TestUI OnClickComplete3");
    }
}
