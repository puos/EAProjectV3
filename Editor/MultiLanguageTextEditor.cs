using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MultiLanguageText))]
public class MultiLanguageTextEditor : Editor
{
    MultiLanguageText curtarget;
    bool isSnapFoldShow = false;

    static public List<MultiLanguageText.ChangeRectTransform> transformList = new List<MultiLanguageText.ChangeRectTransform>()
    { 
       new MultiLanguageText.ChangeRectTransform(Vector3.zero,Vector3.zero,24),
       new MultiLanguageText.ChangeRectTransform(Vector3.zero,new Vector2(400,25),16),
       new MultiLanguageText.ChangeRectTransform(Vector3.zero,new Vector2(125,100),30),
       new MultiLanguageText.ChangeRectTransform(Vector3.zero,Vector3.zero , 20)
    };

    public enum TransformType
    {
        type1 = 0,
        type2,
        type3,
        type4,
    }

    private void OnEnable()
    {
        curtarget = (MultiLanguageText)target;
    }
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Multi Language", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();

        curtarget.uiID = EditorGUILayout.TextField("UI ID", curtarget.uiID);
        curtarget.langType = (LANGUAGE_TYPE)EditorGUILayout.EnumPopup("language:", curtarget.langType);
        curtarget.uiType = (UI_TEXT_TYPE)EditorGUILayout.EnumPopup("type:", curtarget.uiType);
        isSnapFoldShow = EditorGUILayout.Foldout(isSnapFoldShow, "snapshot");

        bool isChangeSnapShow = false;
        bool isDelete = false;

        if(isSnapFoldShow)
        {
            LANGUAGE_TYPE deleteLang = LANGUAGE_TYPE.KOREAN;
           
            var it = curtarget.posSnaps.GetEnumerator();

            while(it.MoveNext())
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.SelectableLabel(it.Current.Key.ToString());
                if(GUILayout.Button("-"))
                {
                    isDelete = true;
                    deleteLang = it.Current.Key;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (isDelete)
            {
                curtarget.posSnaps.Remove(deleteLang);
            }

            // Buttons for setting snapshots and default positions
            EditorGUILayout.BeginHorizontal();

            if(GUILayout.Button("Snapshot"))
            {
                MultiLanguageText.ChangeRectTransform t = new MultiLanguageText.ChangeRectTransform();
                t.RestoreRect(curtarget);
                curtarget.posSnaps[curtarget.langType] = t;
                isChangeSnapShow = true;
            }
            
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            foreach(TransformType t in (TransformType[])Enum.GetValues(typeof(TransformType)))
            {
                if(GUILayout.Button(t.ToString()))
                {
                    curtarget.posSnaps[curtarget.langType] = new MultiLanguageText.ChangeRectTransform(transformList[(int)t]);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        if(isChangeSnapShow || isDelete)
        {
            curtarget.langList = new LANGUAGE_TYPE[curtarget.posSnaps.Keys.Count];
            curtarget.posSnaps.Keys.CopyTo(curtarget.langList, 0);

            curtarget.rectTransformList = new MultiLanguageText.ChangeRectTransform[curtarget.posSnaps.Values.Count];

        } 

        base.OnInspectorGUI();
    }
}
