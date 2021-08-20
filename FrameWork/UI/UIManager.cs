using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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

    private EUIPage m_eCurPage = EUIDefault.PageEnd;
    private EUIPopup m_eCurPopup = EUIDefault.PoupEnd;

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
            int uiLayer = LayerMask.NameToLayer("UI");

            goRoot = new GameObject(UI_ROOT_NAME,typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));

            Canvas c = goRoot.GetComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler cs = goRoot.GetComponent<CanvasScaler>();
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            cs.referenceResolution = new Vector2(EAMainFrame.screenX, EAMainFrame.screenY);

            GraphicRaycaster ray = goRoot.GetComponent<GraphicRaycaster>();
            goRoot.layer = uiLayer;
            
            GameObject page = new GameObject(UI_ROOT_PAGE, typeof(RectTransform) , typeof(SafeRect));
            GameObject above = new GameObject(UI_ROOT_ABOVE, typeof(RectTransform) , typeof(SafeRect));
            GameObject popup = new GameObject(UI_ROOT_POPUP, typeof(RectTransform) , typeof(SafeRect));

            page.layer  = uiLayer;
            above.layer = uiLayer;
            popup.layer = uiLayer;

            EAFrameUtil.SetParent(page.transform , goRoot.transform);
            EAFrameUtil.SetParent(above.transform, goRoot.transform);
            EAFrameUtil.SetParent(popup.transform, goRoot.transform);

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
         m_eCurPage = EUIDefault.PageEnd;
         m_eCurPopup = EUIDefault.PoupEnd;

        uiPage.Clear();
        uiPopup.Clear();
        uiCompo.Clear();
    }

    private UICtrl LoadPage(EUIPage ePage)
    {

        GameObject goPage = m_ResMgr.Create(EResourceGroup.UIPage, ePage.ToString());
        if (goPage == null) return null;
        EAFrameUtil.SetParent(goPage.transform, m_tRootPage);
       
        var key = (int)ePage.Id;
        UICtrl uiCtrl = goPage.GetComponent<UICtrl>();
        if (uiPage.ContainsKey(key))
            uiPage.Remove(key);

        uiPage.Add(key, uiCtrl);
        return uiCtrl;
    }

    private UICtrl LoadAbove(EUIPage ePage)
    {
        GameObject goPage = m_ResMgr.Create(EResourceGroup.UIPage, ePage.ToString());
        if (goPage == null) return null;
        EAFrameUtil.SetParent(goPage.transform, m_tRootAbove);

        var key = (int)ePage.Id;
        UICtrl uiCtrl = goPage.GetComponent<UICtrl>();
        if (uiPage.ContainsKey(key))
            uiPage.Remove(key);

        uiPage.Add(key, uiCtrl);
        return uiCtrl;
    }

    private UICtrl LoadPopup(EUIPopup ePopup)
    {
        GameObject goPopup = m_ResMgr.Create(EResourceGroup.UIPopup, ePopup.ToString());
        if (goPopup == null) return null;
        EAFrameUtil.SetParent(goPopup.transform, m_tRootPopup);

        var key = (int)ePopup.Id;
        UICtrl uiCtrl = goPopup.GetComponent<UICtrl>();
        if (uiPopup.ContainsKey(key))
            uiPopup.Remove(key);

        uiPopup.Add(key, uiCtrl);
        return uiCtrl;
    }

    public T LoadComponent<T>(Transform tRoot,EUIComponent eCompo)
    {
        GameObject goComp = m_ResMgr.Create(EResourceGroup.UIComponent, eCompo.ToString());

        if (goComp != null) EAFrameUtil.SetParent(goComp.transform,tRoot);
        if (goComp != null) return goComp.GetComponent<T>();
        
        return default;
    }

    public void OpenPage(EUIPage ePage)
    {
        if (ePage != m_eCurPage) HidePage(m_eCurPage);
        m_eCurPage = ePage;

        UICtrl uiDlg = GetPage(ePage);
        if (uiDlg != null) uiDlg.Open();
    }

    public T OpenPage<T>(EUIPage ePage) where T : UICtrl
    {
        if (ePage != m_eCurPage) HidePage(m_eCurPage);
        m_eCurPage = ePage;

        UICtrl uiDlg = GetPage(ePage);
        if (uiDlg != null) uiDlg.Open();
        return uiDlg as T;
    }

    public void OpenAbove(EUIPage ePage)
    {
        var key = (int)ePage.Id;
        if (!uiPage.TryGetValue((int)key, out UICtrl uiDlg))
            LoadAbove(ePage);
        uiDlg.Open();
    }
    public T OpenPopup<T>(EUIPopup ePopup) where T : UICtrl
    {
        if (ePopup != m_eCurPopup) HidePopup(m_eCurPopup);
        m_eCurPopup = ePopup;
        UICtrl UiDlg = GetPopup<T>(ePopup);
        if(UiDlg != null) { UiDlg.Open(); }
        return UiDlg as T;
    }

    public void HidePage(EUIPage ePage)
    {
        var key = (int)ePage.Id;
        if (uiPage.ContainsKey(key))
            uiPage[key].Close();
    }

    public void HidePopup(EUIPopup ePopup)
    {
        var key = (int)ePopup.Id;
        if (uiPopup.ContainsKey(key))
            uiPopup[key].Close();
    }

    public UICtrl GetPage(EUIPage ePage)
    {
        var key = (int)ePage.Id;
        if (uiPage.TryGetValue((int)key, out UICtrl uiDlg)) 
            return uiDlg;
        return LoadPage(ePage);
    }

    public UICtrl GetPage<T>(EUIPage ePage) where T : UICtrl
    {
        return GetPage(ePage) as T;
    }

    public UICtrl GetAbove(EUIPage ePage)
    {
        var key = (int)ePage.Id;
        if (uiPage.TryGetValue((int)key, out UICtrl uiDlg))
            return uiDlg;
        return LoadAbove(ePage);
    }
    public T GetPopup<T>(EUIPopup ePopup) where T : UICtrl
    {
        var key = (int)ePopup.Id;
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
