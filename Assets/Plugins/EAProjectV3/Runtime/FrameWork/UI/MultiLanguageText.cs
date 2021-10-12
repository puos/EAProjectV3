using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

// Implementation of multilingual support method for UI

[ExecuteInEditMode]
public class MultiLanguageText : Text
{
    // A class that stores important information of rectTransform and restores it when the language is changed.
    [SerializeField]
    public class ChangeRectTransform
    {
        public Vector2 position;
        public Vector2 size;
        public int fontSize;

        public ChangeRectTransform() { }

        public ChangeRectTransform(Vector2 position ,Vector2 size, int fontSize)
        {
            this.position = position;
            this.size = size;
            this.fontSize = fontSize;
        }

        public ChangeRectTransform(ChangeRectTransform t)
        {
            position = t.position;
            size = t.size;
            fontSize = t.fontSize;
        }

        public void RestoreRect(MultiLanguageText text)
        {
            text.rectTransform.anchoredPosition = position;
            text.rectTransform.sizeDelta = size;
            text.fontSize = fontSize;
        }
    }

    object[] parms = new object[0];

    [HideInInspector] public LANGUAGE_TYPE langType;
    [HideInInspector] public UI_TEXT_TYPE uiType;
    [HideInInspector] public string uiID;
   
    [SerializeField] public LANGUAGE_TYPE[] langList;
    [SerializeField] public ChangeRectTransform[] rectTransformList;

    private Dictionary<LANGUAGE_TYPE, ChangeRectTransform> positionSnaps = new Dictionary<LANGUAGE_TYPE, ChangeRectTransform>();

    bool isTranslated = false;

    public override Color color
    { get => base.color; 
      set
      {
         canvasRenderer.SetAlpha(color.a);  
         base.color = value; 
      } 
    }
    public Dictionary<LANGUAGE_TYPE, ChangeRectTransform> posSnaps
    {
        get { return positionSnaps; }
    }

    protected override void OnEnable()
    {
        // In the actual play situation, display the value read from the option value
        // In the editor, it is displayed as the value set in the inspector.
        if(Application.isPlaying)
        {
            if (langType != CoreApplication.language) langType = CoreApplication.language;
        }

        positionSnaps.Clear();

        for(int i = 0; i < langList.Length; ++i)
        {
            positionSnaps.Add(langList[i], rectTransformList[i]);
        }

        if(!isTranslated) 

        base.OnEnable();
    }

    [ExecuteInEditMode]
    public void OnChangeLanguage(LANGUAGE_TYPE lang)
    {
        langType = lang;
        ResetLanguage(this.parms);
    }

    public bool ResetLanguage(params object[] parms)
    {
        if (string.IsNullOrEmpty(uiID)) return false;

        if(positionSnaps.Count == 0)
        {
            for (int i = 0; i < langList.Length; ++i)
            {
                positionSnaps.Add(langList[i], rectTransformList[i]);
            }
        }

        if(positionSnaps.TryGetValue(langType,out ChangeRectTransform trans))
        {
            trans.RestoreRect(this);
        }
      
        text = EADataManager.instance.TranslateKeyArgs(langType, uiType, uiID, parms);
        isTranslated = true;

        return true;
    }

    public ChangeRectTransform GetDefaultEssentialRT(LANGUAGE_TYPE defaultLanguage)
    {
        ChangeRectTransform result = null;
        int defaultLangIndex = langList.IndexOf(defaultLanguage);
        if (defaultLangIndex != -1) result = rectTransformList[defaultLangIndex];
        return result;
    }
#if UNITY_EDITOR
    [UnityEditor.MenuItem("GameObject/UI/MultiLanguageText")]
    public static void CreateMultiLanguageText() 
    {
        var go = new GameObject("Label", typeof(RectTransform));
        go.transform.SetParent(UnityEditor.Selection.activeTransform, false);
        go.AddComponent<MultiLanguageText>();
    }
#endif
}

