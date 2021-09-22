using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;

public enum AnimationEventType
{
    AnimationEvent,
    StateEvent,
}


// animation management class
public class EAActorAnim : MonoBehaviour
{
    Dictionary<int, AnimState> dic_animStates = new Dictionary<int, AnimState>();

    //  animation event callback
    public delegate void AnimEventCallback(EAActorAnim anim, AnimationEventType eventType, string slotName , string slotValue);

    // animation param slot callback
    public delegate void AnimParamSlotCallback(GameObject targetObj);

    // animation information
    [Serializable]
    public class PlayAnimParam
    {
        public enum Type { Trigger, Integer , Boolean }
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
        public virtual void Init() 
        {
            for(int i = 0; i < playAnimParams.Count; ++i)
            {
                playAnimParams[i].Initialize();
            }
        }
    }

    [SerializeField] private AnimState[] animState = null;
    private Animator m_anim = null;

    [System.NonSerialized] public AnimEventCallback eventCallback;

    public void AnimationEvent_Impact(string iter)
    {
        SendEventToOwner(AnimationEventType.AnimationEvent, iter , string.Empty );
    }
    public void StateEvent_Impact(string iter,string state)
    {
        SendEventToOwner(AnimationEventType.StateEvent, iter , state);
    }
    public void SendEventToOwner(AnimationEventType type, string param , string param2)
    {
        if (eventCallback != null) eventCallback(this, type, param , param2);
    }
    // Change animations and events
    public void ResetAnimState(AnimState[] animStates)
    {
        dic_animStates.Clear();
        for(int i = 0; i < animStates.Length; ++i)
        {
            animState[i].Init();
            int key = CRC32.GetHashForAnsi(animStates[i].key);
            if(!dic_animStates.TryGetValue(key,out AnimState state))
            {
                dic_animStates.Add(key, animState[i]);
            }
            dic_animStates[key] = animState[i];
        }
    }

    public  AnimState GetAnimState(string skey)
    {
        int key = CRC32.GetHashForAnsi(skey);
        dic_animStates.TryGetValue(key, out AnimState state);
        return state;
    }

    // change animation state
    protected void ChangeAnim(AnimState state)
    {
        if (m_anim == null) 
        {
            Debug.Assert(m_anim != null, "EAActorAnim ChangeAnim m_anim is null");
            return; 
        }

        List<PlayAnimParam> animParams = state.playAnimParams;

        string aniTemp = "";

        for(int i = 0; i < animParams.Count; ++i)
        {
            PlayAnimParam.Type type = animParams[i].type;
            string aniName = animParams[i].aniName;
            int value = animParams[i].value;
            if(type == PlayAnimParam.Type.Trigger)
            {
                m_anim.SetTrigger(animParams[i].paramId);
                aniTemp += "/" + aniName;
            }
            if(type == PlayAnimParam.Type.Integer)
            {
                m_anim.SetInteger(animParams[i].paramId, value);
                aniTemp += "/" + aniName + " : " + value;
            }
            if(type == PlayAnimParam.Type.Boolean)
            {
                m_anim.SetBool(animParams[i].paramId, (value > 0) ? true : false);
            }
        }

        Debug.Log(gameObject.name + " # Play Anim # " + aniTemp);
    }

    public void Initialize()
    {
        m_anim = GetComponent<Animator>();
        ResetAnimState(animState);
        EAAniStateBehaviour stateMachine = null;
        if (m_anim != null) stateMachine = m_anim.GetBehaviour<EAAniStateBehaviour>();
        if (stateMachine != null) stateMachine.Init(this);
    }
   
    public void PushAnimation(string key)
    {
        AnimState state = GetAnimState(key);
        ChangeAnim(state);
    }
    
    public void ClearAnimation()
    {
        if (m_anim != null) m_anim.Rebind();
    }
}
