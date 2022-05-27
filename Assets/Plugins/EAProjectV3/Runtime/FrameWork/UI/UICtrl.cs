﻿using UnityEngine;
using System;

public class UICtrl : MonoBehaviour
{

    [HideInInspector] public GameObject m_CachedObject = null;
    [HideInInspector] public RectTransform m_CachedTransform = null;
    protected EAEventManager m_eventMgr = null;
    protected EASfxManager m_sfxMgr = null;
    protected EAUIManager m_uiMgr = null;

    private bool m_bInitialized = false;
    private bool m_bActive = false;

    public bool IsActivate { get { return (m_bActive && m_CachedObject.activeSelf) ? true : false; } }

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        if (m_bInitialized)
            m_CachedObject.SetActive(m_bActive);
    }

    public virtual void Initialize() 
    {
        if(!m_bInitialized)
        {
            m_bInitialized = true;
            m_bActive = false;
        }

        m_CachedObject = gameObject;
        m_CachedTransform = GetComponent<RectTransform>();
        m_eventMgr = EAEventManager.instance;
        m_sfxMgr = EASfxManager.instance;
        m_uiMgr = EAUIManager.instance;
    }

    public virtual void Open()
    {
        m_bActive = true;
        if (null != m_CachedObject) m_CachedObject.SetActive(m_bActive);
    }

    public virtual void Close()
    {
        m_bActive = false;
        if (null != m_CachedObject) m_CachedObject.SetActive(m_bActive);
    }

    public void SetAsFirstSibling() 
    {
        m_CachedTransform.SetAsFirstSibling();
    }
    
}
