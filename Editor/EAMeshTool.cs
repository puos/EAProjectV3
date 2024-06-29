﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class EAMeshTool : Editor
{
    private static void MakeCharacterInfo(GameObject partsObject)
    {
        EACharacterInfo skeleton = partsObject.GetComponent<EACharacterInfo>();
        if (skeleton == null) skeleton = partsObject.AddComponent<EACharacterInfo>();

        Transform[] transforms = partsObject.GetComponentsInChildren<Transform>();

        Dictionary<string, Transform> transformList = new Dictionary<string, Transform>();
        Dictionary<string, Renderer> renderList = new Dictionary<string, Renderer>();

        for (int i = 0; i < transforms.Length; ++i)
        {

            if (partsObject.transform.GetHashCode() == transforms[i].GetHashCode()) continue;

            if (!transformList.TryGetValue(transforms[i].name, out Transform value))
                transformList.Add(transforms[i].name, transforms[i]);

            Renderer curRenderer = transforms[i].gameObject.GetComponent<Renderer>();

            if (curRenderer == null) continue;
            
            if (!renderList.TryGetValue(transforms[i].name, out Renderer renderer))
                renderList.Add(transforms[i].name, curRenderer);
        }

        skeleton.BoneNames = new string[transformList.Keys.Count];
        skeleton.Bones = new Transform[transformList.Keys.Count];
        skeleton.renders = new Renderer[renderList.Values.Count];

        int idx = 0;
        foreach (string key in transformList.Keys) 
         skeleton.BoneNames[idx++] = key; 
        
        idx = 0;
        foreach (Transform t in transformList.Values) 
         skeleton.Bones[idx++] = t; 

        idx = 0;
        foreach (Renderer r in renderList.Values) 
         skeleton.renders[idx++] = r; 
    }

    [MenuItem("Assets/EACharacterInfo", false, 0)]
    private static void MakeCharacters()
    {
        if(Selection.objects.Length == 0)
        {
            Debug.Log("Not Object");
            return;
        }

        for(int i = 0; i < Selection.objects.Length; ++i)
        {
            string pathSrc = AssetDatabase.GetAssetPath(Selection.objects[i]);
            string pathTarget = pathSrc.Remove(pathSrc.LastIndexOf('/') + 1);

            GameObject charObj = Instantiate<GameObject>(AssetDatabase.LoadAssetAtPath<GameObject>(pathSrc));
            MakeCharacterInfo(charObj);
            PrefabUtility.SaveAsPrefabAsset(charObj, pathTarget + Selection.objects[i].name + ".prefab", out bool success);
            DestroyImmediate(charObj);
        }
    }

    [MenuItem("Assets/EASkinInfo", false, 0)]
    private static void MakeParts()
    {
        if(Selection.objects.Length == 0)
        {
            Debug.LogError("Not Object");
            return;
        }

        for(int i = 0; i < Selection.objects.Length; ++i)
        {
            string pathSrc = AssetDatabase.GetAssetPath(Selection.objects[i]);
            MakePartsInfo(pathSrc);
        }
    }
    private static void MakePartsInfo(string pathSrc)
    {
        string pathTarget = pathSrc.Remove(pathSrc.LastIndexOf('/') + 1);

        GameObject partsObject = Instantiate<GameObject>(AssetDatabase.LoadAssetAtPath<GameObject>(pathSrc));
        SkinnedMeshRenderer[] parts = partsObject.GetComponentsInChildren<SkinnedMeshRenderer>();

        if(parts.Length == 0)
        {
            Debug.LogError($"{pathSrc} SkinnedMesh count zero"); 
            DestroyImmediate(partsObject);
            return;
        }

        for(int i = 0; i < parts.Length; ++i)
        {
            SkinnedMeshRenderer part = parts[i];
            MakePartInfo(part);
            PrefabUtility.SaveAsPrefabAsset(part.gameObject, pathTarget + part.name + i.ToString() + ".prefab", out bool success);
        }

        DestroyImmediate(partsObject);
    }
    private static void MakePartInfo(SkinnedMeshRenderer part)
    {
        EASkinInfo skinInfo = part.gameObject.GetComponent<EASkinInfo>();
        if (skinInfo != null) DestroyImmediate(skinInfo);
        skinInfo = part.gameObject.AddComponent<EASkinInfo>();
        skinInfo.RootboneName = part.rootBone.name;
        skinInfo.BoneNames = new string[part.bones.Length];

        for (int i = 0; i < part.bones.Length; ++i) skinInfo.BoneNames[i] = part.bones[i].name;
    }

    [MenuItem("Assets/EAActorAnim", false, 0)]
    private static void MakeAnim() 
    {
        if(Selection.objects.Length == 0)
        {
            Debug.LogError("Not Object");
            return;
        }

        for (int i = 0; i < Selection.objects.Length; ++i)
        {
            string pathSrc = AssetDatabase.GetAssetPath(Selection.objects[i]);
            string pathTarget = pathSrc.Remove(pathSrc.LastIndexOf('/') + 1);

            GameObject charObj = Instantiate<GameObject>(AssetDatabase.LoadAssetAtPath<GameObject>(pathSrc));
            Animator animator = charObj.GetComponentInChildren<Animator>();

            if (animator == null)
            {
                Debug.LogError($"{pathSrc} animator is nothing");
                DestroyImmediate(charObj);
                continue;
            }

            MakeAnimInfo(charObj);
            PrefabUtility.SaveAsPrefabAsset(charObj, pathTarget + Selection.objects[i].name + ".prefab", out bool success);
            DestroyImmediate(charObj);
        }
    }
    
    private static void MakeAnimInfo(GameObject charObj)
    {
        Animator animator = charObj.GetComponentInChildren<Animator>();

        EAActorAnim actorAnim = charObj.GetComponent<EAActorAnim>();
        if (actorAnim == null) actorAnim = charObj.AddComponent<EAActorAnim>();
        actorAnim.m_anim = animator;

        EAActorAnimEvent[] events = animator.GetComponents<EAActorAnimEvent>();
        
        if(events != null)
        {
            for (int i = 0; i < events.Length; ++i) DestroyImmediate(events[i]);
        }

        if (charObj.GetComponent<Animator>() == null)
        {
            EAActorAnimEvent e = animator.gameObject.AddComponent<EAActorAnimEvent>();
            e.actorAnim = actorAnim;
        }

        AnimatorControllerParameter[] animParams = animator.parameters;

        if(actorAnim.animState != null)
        {
            for(int i = 0; i < actorAnim.animState.Length; ++i)
            {
                List<EAActorAnim.PlayAnimParam> playAnimParams = actorAnim.animState[i].playAnimParams;
                List<EAActorAnim.PlayAnimParam> removeAnimParams = new List<EAActorAnim.PlayAnimParam>();

                for (int j = 0; j < playAnimParams.Count; ++j)
                {
                    int idx = animParams.FindIndex(x => x.name.Equals(playAnimParams[j].aniName));
                    if (idx == -1)
                    {
                        removeAnimParams.Add(playAnimParams[j]);
                        continue;
                    }
                    playAnimParams[j].paramId = animParams[idx].nameHash;

                    if ((int)animParams[idx].type == 4) playAnimParams[j].value = (animParams[idx].defaultBool) ? 1 : 0;
                    if ((int)animParams[idx].type == 3) playAnimParams[j].value = animParams[idx].defaultInt;
                    if ((int)animParams[idx].type == 1) playAnimParams[j].value = animParams[idx].defaultFloat;
                    if ((int)animParams[idx].type == 9) playAnimParams[j].value = 0;
                }

                for (int j = 0; j < removeAnimParams.Count; ++j)
                {
                    playAnimParams.Remove(removeAnimParams[j]);
                }
            }
            return;
        }

        actorAnim.animState = new EAActorAnim.AnimState[animParams.Length];
        actorAnim.animState[0] = new EAActorAnim.AnimState();
        PushAnimParams(animParams, actorAnim.animState[0]);
    }

    private static void PushAnimParams(AnimatorControllerParameter[] animParams, EAActorAnim.AnimState animState)
    {
        for (int i = 0; i < animParams.Length; ++i)
        {
            AnimatorControllerParameterType paramType = animParams[i].type;

            switch (paramType)
            {
                case AnimatorControllerParameterType.Bool:
                    animState.playAnimParams.Add(new EAActorAnim.PlayAnimParam()
                    {
                        type = EAActorAnim.PlayAnimParam.Type.Boolean,
                        aniName = animParams[i].name,
                        paramId = animParams[i].nameHash,
                        value = (animParams[i].defaultBool == true) ? 1 : 0
                    });
                    break;
                case AnimatorControllerParameterType.Int:
                    animState.playAnimParams.Add(new EAActorAnim.PlayAnimParam()
                    {
                        type = EAActorAnim.PlayAnimParam.Type.Integer,
                        aniName = animParams[i].name,
                        paramId = animParams[i].nameHash,
                        value = animParams[i].defaultInt
                    });
                    break;
                case AnimatorControllerParameterType.Float:
                    animState.playAnimParams.Add(new EAActorAnim.PlayAnimParam()
                    {
                        type = EAActorAnim.PlayAnimParam.Type.Float,
                        aniName = animParams[i].name,
                        paramId = animParams[i].nameHash,
                        value = animParams[i].defaultFloat
                    });
                    break;
                case AnimatorControllerParameterType.Trigger:
                    animState.playAnimParams.Add(new EAActorAnim.PlayAnimParam()
                    {
                        type = EAActorAnim.PlayAnimParam.Type.Trigger,
                        aniName = animParams[i].name,
                        paramId = animParams[i].nameHash
                    });
                    break;
            }
        }
    }
}
