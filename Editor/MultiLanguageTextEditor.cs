using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using System.IO;

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
            curtarget.posSnaps.Values.CopyTo(curtarget.rectTransformList, 0);

            curtarget.OnChangeLanguage(curtarget.langType);
        } 

        base.OnInspectorGUI();
    }
}

public class LanguageChanger : Editor 
{
   [MenuItem("Language/Refresh language",false,20)]
   public static void LanguageChange()
   {
        string scriptFile = "Assets/Script/Editor/LanguageItemMenu.cs";

        if (!Directory.Exists(scriptFile)) Directory.CreateDirectory(scriptFile);

        string[] menuItems = Enum.GetNames(typeof(LANGUAGE_TYPE));

        // The class string
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("// This class is Auto-Generated");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("using UnityEditor;");
        sb.AppendLine("");
        sb.AppendLine("  public static class GeneratedMenuItems {");
        sb.AppendLine("");

        // loops though the array and generates the menu items
        for(int i = 0; i < menuItems.Length; ++i)
        {
            sb.AppendLine("   [MenuItem(\"Language/" + menuItems[i] + "\")]");
            sb.AppendLine("   private static void MenuItem" + i.ToString() + "(){");
            sb.AppendLine("     LanguageChanger.RebuildLanguage( LANGUAGE_TYPE." + menuItems[i] + ");");
            sb.AppendLine("   }");
            sb.AppendLine("");
        }

        sb.AppendLine("");
        sb.AppendLine("}");

        if (File.Exists(scriptFile)) System.IO.File.Delete(scriptFile);
        System.IO.File.WriteAllText(scriptFile, sb.ToString());
        AssetDatabase.SaveAssets();
        AssetDatabase.ImportAsset(scriptFile,ImportAssetOptions.ForceUpdate);
    }
}
