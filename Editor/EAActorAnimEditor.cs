using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EAActorAnim))]
public class EAActorAnimEditor : Editor
{
    EAActorAnim _target;
    bool isSnapFoldShow = false;

    private void OnEnable()
    {
        _target = (EAActorAnim)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        base.serializedObject.Update();
        
        isSnapFoldShow = EditorGUILayout.Foldout(isSnapFoldShow, "AnimationEvent");

        if(isSnapFoldShow)
        {
            if(GUILayout.Button("Edit"))
            {
                EAAnimationEventEditor.EAAnimationEventEditorMenu(_target.GetComponent<Animator>());
            }
        }

        if(GUI.changed)
        {
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}
