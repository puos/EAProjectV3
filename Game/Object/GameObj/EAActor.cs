using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EAActorMover))]
public class EAActor : EAObject
{
    private    EA_CCharBPlayer m_CharBase = new EA_CCharBPlayer();
    protected  EAActorMover actorMover = null;
    protected EAWeapon currWeapon = null;

    private Dictionary<int, Transform> bones = new Dictionary<int, Transform>();
    private Renderer[] renderers = null;
    private string[] m_PartTblId = new string[(int)eCharParts.CP_MAX];
    
    protected Dictionary<int, System.Action> states = new Dictionary<int, System.Action>();
    protected Dictionary<int, System.Action> updates = new Dictionary<int, System.Action>();

    public int curState { get; private set; }
   
    public uint Id { get { return (m_CharBase != null) ? m_CharBase.GetObjID() : CObjGlobal.InvalidObjID; } }

    public eObjectType objType { get { return (m_CharBase != null) ? m_CharBase.GetObjInfo().m_eObjType : eObjectType.CT_MAXNUM; } }

    public override void Initialize()
    {
        base.Initialize();

        if(cachedCollider == null) cachedCollider = gameObject.AddComponent<CapsuleCollider>();

        states.Clear();
        updates.Clear();

        SetSkeleton();
        SetRenderer();

        actorMover = GetComponent<EAActorMover>();
        actorMover.Initialize();
    }
    public override void Release()
    {
        base.Release();

        ReleaseParts();

        EA_ItemManager.instance.RemoveEquip(Id);
        EACObjManager.instance.DeleteGameObject(objType, Id);
    }
    // Works after SetItemAttachment function
    public virtual void DoAttachItem(eAttachType attachType, eItemType itemType)
    {
        if (itemType == eItemType.eIT_Weapon) RaiseWeapon();
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
    public void SetCharBase(EA_CCharBPlayer CharBase) 
    {
        m_CharBase = CharBase;
    }
    public EA_CCharBPlayer GetCharBase()  { return m_CharBase; }
    public override void SetObjState(eObjectState state)
    {
        m_CharBase.GetObjInfo().m_eObjState = state;
    }
    public eObjectState GetObjState()
    {
        return m_CharBase.GetObjInfo().m_eObjState;
    }
    public override uint GetObjId() { return Id; }
    public Transform GetObjectInActor(string strObjectName)
    {
        Transform t = GetTransform(strObjectName);
        if (t != null) return t;
        return null;
    }
    public Transform GetTransform(string key)
    {
        int nkey = CRC32.GetHashForAnsi(key);
        bones.TryGetValue(nkey, out Transform outValue);
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
    protected void RaiseWeapon()
    {
        EA_CCharBPlayer charBase = GetCharBase();

        //2017 1126 Consider even without weapons
        currWeapon = null;

        if (charBase == null) return;

        EA_Equipment equipment = EA_ItemManager.instance.GetPCEquipItem(charBase.GetObjID());

        if (equipment == null) return;

        EA_CItemUnit itemUnit = equipment.GetCurrentItem();

        if (itemUnit == null) return;
        if (itemUnit.GetItemObjectType() != eItemObjType.IK_WEAPON) return;

        EA_CItem pItem = EACObjManager.instance.GetGameObject(itemUnit.GetObjId()) as EA_CItem;

        if (pItem == null) return;

        if (pItem.GetLinkItem() != null) currWeapon = (EAWeapon)pItem.GetLinkItem() as EAWeapon;

        if (currWeapon == null) return;

        currWeapon.RaiseWeapon();
    }
    public void SetSkeleton()
    {
        if (bones.Count > 0) return;

        EACharacterInfo characterInfo = tr.GetComponent<EACharacterInfo>();

        if(characterInfo == null)
        {
            Transform[] tfs = tr.GetComponentsInChildren<Transform>();
            for(int i = 0; i < tfs.Length; ++i)
            {
                int key = CRC32.GetHashForAnsi(tfs[i].name);
                if (!bones.TryGetValue(key, out Transform value)) { bones.Add(key, tfs[i]); }
            }
            return;
        }

        Transform[] transforms = characterInfo.Bones;
        string[] BoneNames     = characterInfo.BoneNames;
        
        for(int i = 0; i < transforms.Length; ++i)
        {
            int key = CRC32.GetHashForAnsi(transforms[i].name);
            bones.Add(key, transforms[i]);
        }

        Transform mesh = GetTransform("mesh");

        if (mesh != null) mesh.transform.parent = null;
        if (mesh != null) GameObject.Destroy(mesh.gameObject);
        mesh = null;

        GameObject meshObj = EAFrameUtil.AddChild(gameObject, "mesh");
        if (meshObj != null) AddTransform("mesh", meshObj.transform);

    }
    public void SetRenderer()
    {
        EACharacterInfo characterInfo = tr.GetComponent<EACharacterInfo>();
        if (characterInfo == null) return;
        renderers = characterInfo.renderers;

    }
    public void AddTransform(string key, Transform obj)
    {
        int nKey = CRC32.GetHashForAnsi(key);
        if(!bones.TryGetValue(nKey,out Transform value))
        {
            bones.Add(nKey, obj);
            return;
        }

        bones[nKey] = obj;
    }
}
