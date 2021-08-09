using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EAActorMover))]
public class EAActor : EAObject
{
    private    EA_CCharBPlayer m_pDiaCharBase = new EA_CCharBPlayer();
    protected  EAActorMover actorMover = null;

    private Dictionary<int, Transform> transformList = new Dictionary<int, Transform>();
    private Renderer[] renderers = null;
    private string[] m_PartTblId = new string[(int)eCharParts.CP_MAX];
    
    protected Dictionary<int, System.Action> states = new Dictionary<int, System.Action>();
    protected Dictionary<int, System.Action> updates = new Dictionary<int, System.Action>();

    public int curState { get; private set; }
    public uint Id { get { return (m_pDiaCharBase != null) ? m_pDiaCharBase.GetObjID() : CObjGlobal.InvalidObjID; } }

    public override void Initialize()
    {
        base.Initialize();

        if(cachedCollider == null) cachedCollider = gameObject.AddComponent<CapsuleCollider>();

        states.Clear();
        updates.Clear();
        transformList.Clear();

        actorMover = GetComponent<EAActorMover>();
        actorMover.Initialize();
    }
    public override void Release()
    {
        base.Release();

        ReleaseParts();
    }
    // Works after SetItemAttachment function
    public virtual void DoAttachItem(eAttachType attachType)
    {       
    }
    public virtual void OnAction(params object[] parms)
    {
    }

    // Works before DoSwitchWeapon
    public virtual bool SetItemAttachment(eAttachType attachType , EAObject gameObject)
    {
        return true; 
    }
    protected override void UpdatePerFrame()
    {
        base.UpdatePerFrame();
        actorMover.UpdateMove();
    }
    public void FSMUpdate()
    {
        if (updates.TryGetValue(curState, out Action value)) value();
    }
    public void ChangeFSMState(int newState)
    {
        if (curState == newState) return;

        curState = newState;
        Debug.Log("id : " + Id + " State : " + curState);

        if (states.TryGetValue(curState, out Action value)) value();
    }
    public void SetCharBase(EA_CCharBPlayer pDiaCharBase)
    {
        m_pDiaCharBase = pDiaCharBase;
    }

    public EA_CCharBPlayer GetCharBase()  { return m_pDiaCharBase; }

    public override void SetObjState(eObjectState state)
    {
        m_pDiaCharBase.GetObjInfo().m_eObjState = state;
    }
    public Transform GetObjectInActor(string strObjectName)
    {
        Transform t = GetTransform(strObjectName);
        if (t != null) return t;
        return null;
    }
    public Transform GetTransform(string key)
    {
        int nkey = CRC32.GetHashForAnsi(key);
        transformList.TryGetValue(nkey, out Transform outValue);
        return outValue;
    }
    private void ReleaseParts()
    {
        Transform mesh = GetTransform("mesh");

        for (int i = 0; i < m_PartTblId.Length; ++i)
        {
            if (!string.IsNullOrEmpty(m_PartTblId[i])) ReleasePart(mesh, m_PartTblId[i]);

            m_PartTblId[i] = string.Empty;
        }
    }
    private void ReleasePart(Transform mesh, string parts)
    {
        Transform mesh_part = EAFrameUtil.FindChildRecursively(mesh, parts);
        if (mesh_part == null) return;
        GameResourceManager.instance.ReleaseObject(mesh_part.gameObject);
    }
    private void FindParts(string[] PartTblId)
    {
        Transform mesh = GetTransform("mesh");

        for (int i = 0; i < PartTblId.Length; ++i)
        {
            if (m_PartTblId[i].Equals(PartTblId[i],StringComparison.Ordinal)) continue;

            ReleasePart(mesh, m_PartTblId[i]);
            AddPart(mesh, i , PartTblId[i]);
        }
    }
    public virtual void AddPart(Transform mesh, int idx, string parts)
    {
        m_PartTblId[idx] = parts;
    }
}
