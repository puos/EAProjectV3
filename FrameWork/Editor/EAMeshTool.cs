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
        skeleton.renderers = new Renderer[renderList.Values.Count];

        int idx = 0;
        foreach (string key in transformList.Keys) 
         skeleton.BoneNames[idx++] = key; 
        
        idx = 0;
        foreach (Transform t in transformList.Values) 
         skeleton.Bones[idx++] = t; 

        idx = 0;
        foreach (Renderer r in renderList.Values) 
         skeleton.renderers[idx++] = r; 
    }

    [MenuItem("Assets/EACharacterInfo", false, 0)]
    private static void MakeCharacters()
    {
        if(Selection.objects.Length == 0)
        {
            Debug.Log("Not Character");
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
            Debug.Log("Not SkinnedMesh");
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
}
