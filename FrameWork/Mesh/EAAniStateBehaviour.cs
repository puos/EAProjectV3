using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EAAniStateBehaviour : StateMachineBehaviour
{
    private EAActorAnim actorAnim = null;

    public void Init(EAActorAnim actorAnim)
    {
        this.actorAnim = actorAnim;
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (actorAnim != null) actorAnim.StateEvent_Impact($"{stateInfo.fullPathHash}", "enter");
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (actorAnim != null) actorAnim.StateEvent_Impact($"{stateInfo.fullPathHash}", "exit");
    }
}
