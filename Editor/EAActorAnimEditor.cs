using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EAActorAnim))]
public class EAActorAnimEditor : Editor
{
    EAActorAnim _target;
    
    private void OnEnable()
    {
        _target = (EAActorAnim)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        base.serializedObject.Update();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("AnimationEvent");

        GUILayoutOption w = GUILayout.Width(150);
       
        if (GUILayout.Button("Edit",w))
        {
            EAAnimationEventEditor.EAAnimationEventEditorMenu(_target.m_anim);
        }

        EditorGUILayout.EndHorizontal();

        if (GUI.changed)
        {
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}
