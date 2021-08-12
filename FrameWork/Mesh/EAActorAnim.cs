using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;

public enum AnimationEventType
{
    None,
    Impact,
    Sfx,
    PlaySound,
}


// animation management class
public class EAActorAnim : StateMachineBehaviour
{
    Dictionary<int, AnimState> dic_animStates = new Dictionary<int, AnimState>();

    protected Queue<AnimState> animStateQueue = new Queue<AnimState>();

    //  animation event callback
    public delegate void AnimEventCallback(EAActorAnim anim, AnimationEventType eventType, string slotName);

    // animation param slot callback
    public delegate void AnimParamSlotCallback(GameObject targetObj);

    // animation information
    [Serializable]
    public class PlayAnimParam
    {
        public enum Type { Trigger, Integer }
        public string aniName;
        public Type type;
        public int value;

        [HideInInspector] public int paramId;

        public void Initialize(){  this.paramId = Animator.StringToHash(aniName);   }
    }

    [Serializable]
    public class AnimParamSlot
    {
        public string name;
        public AnimParamSlotCallback callback;
        public GameObject innerObj;
    }

    [Serializable]
    public class AnimState
    {
        public string key;

        [SerializeField]
        public AnimParamSlot[] m_paramSlots;

        [SerializeField]
        public List<PlayAnimParam> playAnimParams = new List<PlayAnimParam>();

        public void SetParam(string slotName,AnimParamSlotCallback cb)
        {
            int slotIdx = FindParamSlotIdxByName(slotName);
            if(slotIdx != -1)
            {
                AnimParamSlot slot = m_paramSlots[slotIdx];
                slot.callback = cb;
                if (slot.innerObj != null) cb(slot.innerObj);
            }
        }

        public void SetInnerObject(string slotName, GameObject obj)
        {
            int slotIdx = FindParamSlotIdxByName(slotName);
            if (slotIdx != -1)
            {
                AnimParamSlot slot = m_paramSlots[slotIdx];
                slot.innerObj = obj;
            }
        }

        int FindParamSlotIdxByName(string name)
        {
            for (int i = 0; i < m_paramSlots.Length; i++)
                if (m_paramSlots[i].name.Equals(name))
                    return i;
            return -1;
        }
               
    }

    [SerializeField]
    private AnimState[] animState = null;

    private Animator m_anim = null;

    // Change animations and events
    public void ResetAnimState(AnimState[] animStates)
    {
       
    }

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineEnter(animator, stateMachinePathHash);
        m_anim = animator;
        ResetAnimState(animState);
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);           
    }

}
