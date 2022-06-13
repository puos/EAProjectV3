using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class NearSensor : MonoBehaviour
{
    HashSet<EAAIAgent> _targets = new HashSet<EAAIAgent>();

    private void Awake()
    {
        Collider c  = GetComponent<BoxCollider>();
        if(!c.isTrigger) c.isTrigger = true;
    }

    public HashSet<EAAIAgent> targets
    {
        get
        {
            _targets.RemoveWhere((EAAIAgent r) => (r == null || r.Equals(null)));
            return _targets;
        }
    } 

    void TryToAdd(Component other)
    {
        EAAIAgent rb = other.GetComponent<EAAIAgent>();
        if(rb != null) _targets.Add(rb);
    }

    void TryToRemove(Component other)
    {
        EAAIAgent rb = other.GetComponent<EAAIAgent>();
        if (rb != null) _targets.Remove(rb);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryToAdd(other);
    }

    private void OnTriggerExit(Collider other)
    {
        TryToRemove(other);
    }
}
