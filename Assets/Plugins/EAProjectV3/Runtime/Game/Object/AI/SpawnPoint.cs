using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public float randomRadius = 1f;

    public Transform[] trs;
    
    public readonly Dictionary<string, List<Transform>> spawnGroup = new Dictionary<string, List<Transform>>();

    public readonly Dictionary<string, Color> spawnColors = new Dictionary<string, Color>();

    public void Initialize()
    {
        var it = spawnGroup.GetEnumerator();
        while (it.MoveNext())
            it.Current.Value.Clear();

        spawnGroup.Clear();
        spawnColors.Clear();

        if (trs == null) return;

        for(int i= 0; i < trs.Length; ++i)
        {
            if (object.ReferenceEquals(transform, trs[i])) continue;

            string name = trs[i].name;

            if(!spawnGroup.TryGetValue(name,out List<Transform> values))
            {
                values = new List<Transform>();
                spawnGroup.Add(name, values);

                spawnColors.Add(name, UnityEngine.Random.ColorHSV());
            }
            values.Add(trs[i]);
        }
    }

    public void Generate()
    {
        List<Transform> transforms = new List<Transform>(GetComponentsInChildren<Transform>());
        transforms.RemoveAll(x => x.tag.Equals("Hidden"));
        trs = transforms.ToArray();
    }

    public Vector3 GetPoint(string pointName)
    {
        if (!spawnGroup.TryGetValue(pointName, out List<Transform> spawnpoints)) return Vector3.zero;
        return spawnpoints[0].position;
    }

    public Vector3 GetRandomPoint(string pointName)
    {
        if (!spawnGroup.TryGetValue(pointName, out List<Transform> spawnpoints)) return Vector3.zero;
        int spawnIndex = Random.Range(0, spawnpoints.Count);
        return spawnpoints[spawnIndex].position + UnityEngine.Random.onUnitSphere * randomRadius;
    }

    public Vector3 GetRandomPoint()
    {
        int randIdx = Random.Range(0, spawnGroup.Count);
        var it = spawnGroup.GetEnumerator();
        int idx = 0;
        while(it.MoveNext())
        {
            if (idx++ != randIdx) continue;
            return GetRandomPoint(it.Current.Key);
        }
        return Vector3.zero;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {

        var it = spawnGroup.GetEnumerator();

        while(it.MoveNext())
        {
            List<Transform> spawnPoints = it.Current.Value;
            if (spawnPoints == null) continue;
            Color c = spawnColors[it.Current.Key];
            for (int i = 0; i < spawnPoints.Count; ++i)
            {
                if (spawnPoints[i] == null) continue;
                DebugExtension.DrawCircle(spawnPoints[i].position, Vector3.up, c, randomRadius);
            }
        }
    }
#endif
}
