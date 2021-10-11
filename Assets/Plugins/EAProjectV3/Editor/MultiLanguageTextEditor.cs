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

        base.OnInspectorGUI();
    }
}
