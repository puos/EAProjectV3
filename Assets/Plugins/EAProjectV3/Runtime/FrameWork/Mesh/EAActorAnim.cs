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

    // animation information
    [Serializable]
    public class PlayAnimParam
    {
        public enum Type { Trigger, Integer , Float , Boolean }
        public string aniName;
        public Type   type;
        public float value;
        public int paramId;
    }

    [Serializable]
    public class AnimState
    {
        public string key;

        [SerializeField]
        public List<PlayAnimParam> playAnimParams = new List<PlayAnimParam>();
        
        public PlayAnimParam GetAnimParams(string paramName)
        {
            int idx = playAnimParams.FindIndex(x => x.aniName.Equals(paramName, StringComparison.Ordinal));
            if (idx == -1) return null;
            return playAnimParams[idx];
        }
    }

    [SerializeField] public AnimState[] animState = null;
    [SerializeField] public Animator m_anim = null;

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

    public PlayAnimParam GetAnimParams(string sKey,string paramName)
    {
        AnimState currState = GetAnimState(sKey);
        if (currState == null) return null;
        return currState.GetAnimParams(paramName);
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
            float value = animParams[i].value;
            if(type == PlayAnimParam.Type.Trigger)
            {
                m_anim.SetTrigger(animParams[i].paramId);
                aniTemp += "/" + aniName;
            }
            if(type == PlayAnimParam.Type.Integer)
            {
                m_anim.SetInteger(animParams[i].paramId, (int)value);
                aniTemp += "/" + aniName + " : " + (int)value;
            }
            if(type == PlayAnimParam.Type.Float)
            {
                m_anim.SetFloat(animParams[i].paramId,value);
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
        if(m_anim == null) m_anim = GetComponent<Animator>();
        ClearAnimation();
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
        StopAnimation();

        AnimatorControllerParameter[] animParams = m_anim.parameters;

        for (int i = 0; i < animParams.Length; ++i)
        {
            AnimatorControllerParameterType paramType = animParams[i].type;
            switch (paramType)
            {
                case AnimatorControllerParameterType.Bool:
                    m_anim.SetBool(animParams[i].nameHash, animParams[i].defaultBool);
                    break;
                case AnimatorControllerParameterType.Int:
                    m_anim.SetInteger(animParams[i].nameHash, animParams[i].defaultInt);
                    break;
                case AnimatorControllerParameterType.Float:
                    m_anim.SetFloat(animParams[i].nameHash, animParams[i].defaultFloat);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    m_anim.ResetTrigger(animParams[i].nameHash);
                    break;
            }
        }
    }
    
    public void StopAnimation()
    {
        if (m_anim == null) return;

        m_anim.Rebind();
        
        EAAniStateBehaviour stateMachine = m_anim.GetBehaviour<EAAniStateBehaviour>();
        if (stateMachine != null) stateMachine.Init(this);
    }

    public void PlayAnim(int stateNameHash)
    {
        m_anim.Play(stateNameHash);
    }
}
