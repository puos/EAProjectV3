using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAActorAnimEvent : MonoBehaviour
{
    [SerializeField] public EAActorAnim actorAnim = null;

    public void AnimationEvent_Impact(string iter)
    {
        if (actorAnim != null) actorAnim.AnimationEvent_Impact(iter);
    }
}
