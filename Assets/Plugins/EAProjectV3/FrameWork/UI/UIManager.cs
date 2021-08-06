﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public enum UISpawntype
    {
        EUIPage,
        EUIAbove,
        EUIPopup,
    }

    public static string UI_ROOT_NAME = "UIRoot";
    public static string UI_ROOT_PAGE = "Page";
    public static string UI_ROOT_ABOVE = "Above";
    public static string UI_ROOT_POPUP = "Popup";

    private Dictionary<int, UICtrl> uiPage = new Dictionary<int,UICtrl>();
    private Dictionary<int,UICtrl> uiPopup = new Dictionary<int, UICtrl>();
    private Dictionary<int, GameObject> uiCompo = new Dictionary<int, GameObject>();
    private GameResourceManager m_ResMgr = null;

    private Transform m_tUIRoot = null;
    private Transform m_tRootPage = null;
    private Transform m_tRootAbove = null;
    private Transform m_tRootPopup = null;

    private string m_eCurPage  = string.Empty;
    private string m_eCurPopup = string.Empty;

    public override GameObject GetSingletonParent()
    {
        return EAMainFrame.instance.gameObject;
    }

    protected override void Initialize()
    {
        if (m_ResMgr == null) m_ResMgr = GameResourceManager.instance;

        EAMainFrame.instance.OnMainFrameFacilityCreated(MainFrameAddFlags.UiManager);

        GameObject goRoot = GameObject.Find(UI_ROOT_NAME);

        if (goRoot == null)
        {
            goRoot = new GameObject(UI_ROOT_NAME);

            GameObject page = new GameObject(UI_ROOT_PAGE);
            GameObject above = new GameObject(UI_ROOT_ABOVE);
            GameObject popup = new GameObject(UI_ROOT_POPUP);

            page.transform.SetParent(goRoot.transform);
            above.transform.SetParent(goRoot.transform);
            popup.transform.SetParent(goRoot.transform);
        }

        if (null != goRoot)
        {
            m_tUIRoot = goRoot.transform;
            m_tRootPage = m_tUIRoot.Find(UI_ROOT_PAGE);
            m_tRootAbove = m_tUIRoot.Find(UI_ROOT_ABOVE);
            m_tRootPopup = m_tUIRoot.Find(UI_ROOT_POPUP);
        }
    }

    public void Clear()
    {
         m_eCurPage = string.Empty;
         m_eCurPopup = string.Empty;

        uiPage.Clear();
        uiPopup.Clear();
        uiCompo.Clear();
    }

    private UICtrl LoadPage(string ePage)
    {

        GameObject goPage = m_ResMgr.Create(EResourceGroup.UIPage, ePage);
        if (goPage == null) return null;
        goPage.transform.SetParent(m_tRootPage);

        var key = CRC32.GetHashForAnsi(ePage);
        UICtrl uiCtrl = goPage.GetComponent<UICtrl>();
        if (uiPage.ContainsKey(key))
            uiPage.Remove(key);

        uiPage.Add(key, uiCtrl);
        return uiCtrl;
    }

    private UICtrl LoadAbove(string ePage)
    {
        GameObject goPage = m_ResMgr.Create(EResourceGroup.UIPage, ePage);
        if (goPage == null) return null;
        goPage.transform.SetParent(m_tRootAbove);

        var key = CRC32.GetHashForAnsi(ePage);
        UICtrl uiCtrl = goPage.GetComponent<UICtrl>();
        if (uiPage.ContainsKey(key))
            uiPage.Remove(key);

        uiPage.Add(key, uiCtrl);
        return uiCtrl;
    }

    private UICtrl LoadPopup(string ePopup)
    {
        GameObject goPopup = m_ResMgr.Create(EResourceGroup.UIPopup, ePopup);
        if (goPopup == null) return null;
        goPopup.transform.SetParent(m_tRootPopup);

        var key = CRC32.GetHashForAnsi(ePopup);
        UICtrl uiCtrl = goPopup.GetComponent<UICtrl>();
        if (uiPopup.ContainsKey(key))
            uiPopup.Remove(key);

        uiPopup.Add(key, uiCtrl);
        return uiCtrl;
    }

    public T LoadComponent<T>(Transform tRoot,string eCompo)
    {
        GameObject goComp = m_ResMgr.Create(EResourceGroup.UIComponent, eCompo);
        if (goComp != null) return goComp.GetComponent<T>();
        return default;
    }

    public void OpenPage(string ePage)
    {
        if (ePage != m_eCurPage) HidePage(m_eCurPage);
        m_eCurPage = ePage;

        UICtrl uiDlg = GetPage(ePage);
        if (uiDlg != null) uiDlg.Open();
    }

    public void OpenAbove(string ePage)
    {
        var key = CRC32.GetHashForAnsi(ePage);
        if (!uiPage.TryGetValue((int)key, out UICtrl uiDlg))
            LoadAbove(ePage);
        uiDlg.Open();
    }
    public T OpenPopup<T>(string ePopup) where T : UICtrl
    {
        if (ePopup != m_eCurPopup) HidePopup(m_eCurPage);
        UICtrl UiDlg = GetPopup<T>(ePopup);
        if(UiDlg != null)
        {
            m_eCurPopup = ePopup;
            UiDlg.Open();
        }
        return UiDlg as T;
    }

    public void HidePage(string ePage)
    {
        var key = CRC32.GetHashForAnsi(ePage);
        if (uiPage.ContainsKey(key))
            uiPage[key].Close();
    }

    public void HidePopup(string ePopup)
    {
        var key = CRC32.GetHashForAnsi(ePopup);
        if (uiPopup.ContainsKey(key))
            uiPopup[key].Close();
    }

    public UICtrl GetPage(string ePage)
    {
        var key = CRC32.GetHashForAnsi(ePage);
        if (uiPage.TryGetValue((int)key, out UICtrl uiDlg)) 
            return uiDlg;
        return LoadPage(ePage);
    }

    public UICtrl GetAbove(string ePage)
    {
        var key = CRC32.GetHashForAnsi(ePage);
        if (uiPage.TryGetValue((int)key, out UICtrl uiDlg))
            return uiDlg;
        return LoadAbove(ePage);
    }
    public T GetPopup<T>(string ePopup) where T : UICtrl
    {
        var key = CRC32.GetHashForAnsi(ePopup);
        if (uiPopup.TryGetValue(key, out UICtrl uiDlg))
            return uiDlg as T;
        return LoadPopup(ePopup) as T;
    }
    
    public void ShowUI(bool bShow)
    {
        m_tRootPage.gameObject.SetActive(bShow);
        m_tRootAbove.gameObject.SetActive(bShow);
        m_tRootPopup.gameObject.SetActive(bShow);
    }
}
