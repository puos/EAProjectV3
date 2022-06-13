using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAPathMaker : MonoBehaviour
{
    public string key;
    public Transform[] objects = new Transform[]{ };
    public Color color;

    private bool bInitialize = false;

    public void Initialize()
    {
        List<Vector3> nodes = new List<Vector3>();
        for(int i = 0; i < objects.Length;++i)
        {
            Vector3 position = objects[i].transform.position;
            nodes.Add(position);
        }
        if(nodes.Count > 0) AIPathManager.instance.AddPath(key, nodes.ToArray());
        bInitialize = true;
    }

    public void Generate()
    {
        List<Transform> transforms = new List<Transform>(GetComponentsInChildren<Transform>());
        transforms.RemoveAll(x => x.tag.Equals("Hidden"));
        objects = transforms.ToArray();
        bInitialize = false;
    }

    private void OnDrawGizmos()
    {
        if (string.IsNullOrEmpty(key)) return;
        EAAIPath aiPath = AIPathManager.instance.GetPath(key);
        if(aiPath != null) aiPath.DrawGizmo(color);
    }
}
