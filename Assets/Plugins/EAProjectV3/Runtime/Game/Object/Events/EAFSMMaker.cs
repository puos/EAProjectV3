using System;
using System.Collections.Generic;
using UnityEngine;

public class EAFSMMaker
{
    private Dictionary<uint, Action> states = new Dictionary<uint, Action>();
    private Dictionary<uint, Action> updates = new Dictionary<uint, Action>();

    private uint currState = 0;
    private uint uid = 0;

    public void Initialize(uint uid = 0) 
    {
        states.Clear();
        updates.Clear();
        currState =  0;
        this.uid = uid;
    }

    public void AddState<T>(T nState,Action stateA = null,Action updateA = null) where T : Enum
    {
        if (states.TryGetValue(System.Convert.ToUInt32(nState), out Action value)) return;

        states.Add(System.Convert.ToUInt32(nState), stateA);
        updates.Add(System.Convert.ToUInt32(nState), updateA);
    }

    public void ChangeState<T>(T nState) where T : Enum
    {
        uint newState = System.Convert.ToUInt32(nState);

        if (newState != 0 && currState == newState) return;

        currState = newState;

        Debug.Log($"id : {uid} State : {currState}");

        states.TryGetValue(currState, out Action stateA);
        if (stateA != null) stateA();
    }
    
    public void OnUpdate()
    {
        updates.TryGetValue(currState, out Action updateA);
        if (updateA != null) updateA();
    }
}
