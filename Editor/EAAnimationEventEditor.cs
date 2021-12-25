using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;

public class EAAnimationEventEditor : EditorWindow
{
    public static void EAAnimationEventEditorMenu(Animator animator) 
    {
        EditorWindow.GetWindow(typeof(EAAnimationEventEditor));
        sourceAnimator = animator;
    }

    public class EAAnimationEventItem
    {
        public AnimationEvent animationEvent;
        
        public EAAnimationEventItem(AnimationEvent animationEvent , string funcName , string param)
        {
            this.animationEvent = animationEvent;
            this.animationEvent.functionName = funcName;
            this.animationEvent.stringParameter = param;
        }

        public EAAnimationEventItem(AnimationEvent animationEvent) 
        {
            this.animationEvent = animationEvent;
        }
    }

    Vector2  scrollPos;
    int selectedIndex;
    int prevIndex = -1;

    public static Animator sourceAnimator;
    private AnimationClip currentClip;
    private List<EAAnimationEventItem> listAnimEventItem;

    private void OnGUI()
    {
        EditorGUILayout.ObjectField("Animator Object", sourceAnimator, typeof(Animator), false);

        if (sourceAnimator == null)
        {
            listAnimEventItem = null;
            return;
        }

        List<string> listClipName = new List<string>();

        foreach(AnimationClip clip in sourceAnimator.runtimeAnimatorController.animationClips)
        {
            listClipName.Add(clip.name);
        }
       
        selectedIndex = EditorGUILayout.Popup(selectedIndex, listClipName.ToArray());

        if (selectedIndex < 0) selectedIndex = 0;
        if (selectedIndex >= sourceAnimator.runtimeAnimatorController.animationClips.Length) return;

        if (selectedIndex != prevIndex) ShowAnimationEvent();
        if (listAnimEventItem == null) return;
        if (currentClip == null) return;

        if (GUILayout.Button("Add Event"))
        {
            listAnimEventItem.Add(new EAAnimationEventItem(new AnimationEvent(),"AnimationEvent_Impact",string.Empty));
        }

        float frameTime = Mathf.Round(1000f/currentClip.frameRate)/1000f;
        float endFrame = currentClip.length / frameTime;
        EditorGUILayout.LabelField("FrameTime=" + frameTime);


        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        foreach(EAAnimationEventItem item in listAnimEventItem)
        {
            AnimationEvent animEvent = item.animationEvent;
            int frame = (int)Mathf.Round(animEvent.time / frameTime);

            //Debug.Log(animEvent.stringParameter + " time : " + animEvent.time + " frame Time" + frameTime + "aniLength " + currentClip.length);

            EditorGUILayout.PrefixLabel("Frame " + frame);
          
            bool isRemove = false;

            EditorGUI.indentLevel++;

            float curFrame = EditorGUILayout.IntSlider(frame, 0, (int)endFrame);       
            animEvent.time = curFrame * frameTime;
            EditorGUILayout.LabelField("Time", animEvent.time.ToString() + " / " + currentClip.length.ToString());
            animEvent.stringParameter = EditorGUILayout.TextField("params", animEvent.stringParameter);
            if (GUILayout.Button("Remove",GUILayout.Width(70)))
            {
                listAnimEventItem.Remove(item);
                isRemove = true;
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            if (isRemove) break;
        }

        EditorGUILayout.EndScrollView();

        GUI.color = Color.green;
        if(GUILayout.Button("save"))
        {
            SaveAnimation();
        }
        GUI.color = Color.white;
    }

    private void ShowAnimationEvent() 
    {
        prevIndex = selectedIndex;
        
        AnimationClip tmpClip = sourceAnimator.runtimeAnimatorController.animationClips[selectedIndex];
        string pathSrc = AssetDatabase.GetAssetPath(tmpClip);
        currentClip = (AnimationClip)AssetDatabase.LoadAssetAtPath(pathSrc, typeof(AnimationClip));
        Debug.Log("currentClip=" + currentClip);
        listAnimEventItem = new List<EAAnimationEventItem>();
        foreach (AnimationEvent animEvent in currentClip.events) listAnimEventItem.Add(new EAAnimationEventItem(animEvent));
    }

    private void SaveAnimation()
    {
        if (currentClip == null) return;
        if (listAnimEventItem == null) return;
        List<AnimationEvent> tmpList = new List<AnimationEvent>();
        foreach (EAAnimationEventItem item in listAnimEventItem) tmpList.Add(item.animationEvent);
        AnimationUtility.SetAnimationEvents(currentClip, tmpList.ToArray());

        string pathSrc = AssetDatabase.GetAssetPath(currentClip);
        ModelImporter modelImporter = AssetImporter.GetAtPath(pathSrc) as ModelImporter;
        SerializedObject so = new SerializedObject(modelImporter);
        SerializedProperty clips = so.FindProperty("m_ClipAnimations");
        List<AnimationEvent[]> animationEvents = new List<AnimationEvent[]>(modelImporter.clipAnimations.Length);

        for(int i = 0; i < modelImporter.clipAnimations.Length; ++i)
        {
            if(clips.GetArrayElementAtIndex(i).displayName.Equals(currentClip.name,StringComparison.Ordinal))
            {
                SetEvents(clips.GetArrayElementAtIndex(i), tmpList.ToArray());
                break;
            }
        }

        so.SetIsDifferentCacheDirty();
        so.ApplyModifiedProperties();

        modelImporter.SaveAndReimport();
        Debug.Log("Save: currentClip=" + currentClip);
    }

    public void SetEvents(SerializedProperty sp , AnimationEvent[] newEvents)
    {
        SerializedProperty serializedProperty = sp.FindPropertyRelative("events");
        if (serializedProperty == null) return;
        if (newEvents == null) return;

        serializedProperty.ClearArray();
        for(int i = 0; i < newEvents.Length; ++i)
        {
            AnimationEvent animationEvent = newEvents[i];
            serializedProperty.InsertArrayElementAtIndex(serializedProperty.arraySize);
            SerializedProperty eventProperty = serializedProperty.GetArrayElementAtIndex(i);
            eventProperty.FindPropertyRelative("floatParameter").floatValue = animationEvent.floatParameter;
            eventProperty.FindPropertyRelative("functionName").stringValue = animationEvent.functionName;
            eventProperty.FindPropertyRelative("intParameter").intValue = animationEvent.intParameter;
            eventProperty.FindPropertyRelative("objectReferenceParameter").objectReferenceValue = animationEvent.objectReferenceParameter;
            eventProperty.FindPropertyRelative("data").stringValue = animationEvent.stringParameter;
            eventProperty.FindPropertyRelative("time").floatValue = animationEvent.time / currentClip.length;
        }
    }

    private void OnLostFocus()
    {
        SaveAnimation();
    }
}
