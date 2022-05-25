using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUI : UICtrl
{
    [SerializeField] private UITween_TweenRuntime m_Tween = null;

    public override void Initialize()
    {
        base.Initialize();
        m_Tween.enabled = false;
    }

    public void OnClickEvent()
    {
        Debug.Log("TestUI OnClickEvent");
        m_Tween.enabled = true;
        m_Tween.Play();
    }

    public void OnClickComplete()
    {
        Debug.Log("TestUI OnClickComplete");
    }
}
