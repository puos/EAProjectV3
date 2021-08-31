using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAObject : MonoBehaviour
{
    protected bool initialized = false;
    
    private Rigidbody rigidBody = null;
    private Transform cachedTransform = null;
    protected Collider cachedCollider = null;
   
    public Rigidbody rb { get{ return rigidBody; } }
    public Collider col { get { return cachedCollider;  } }
    public Transform tr { get { return cachedTransform; } }

    public System.Action<Collider, EAObject> triggerEvent = null;
    public System.Action<Collision, EAObject> collisionEvent = null;

    public string Name { get; set; }

    public virtual void Initialize()
    {
        initialized = true;
        cachedTransform = transform;
        rigidBody = GetComponent<Rigidbody>();
        cachedCollider = GetComponent<Collider>();

        if (rigidBody != null) rigidBody.velocity = Vector3.zero;
        if (rigidBody != null) rigidBody.angularVelocity = Vector3.zero;
        
        if (cachedTransform != null) cachedTransform.localPosition = Vector3.zero;
        if (cachedTransform != null) cachedTransform.localRotation = Quaternion.identity;

            EAMainFrame.onUpdate.Remove(OnUpdate);
        EAMainFrame.onUpdate.Add(OnUpdate);
    }

    public virtual void Release()
    {
        EAMainFrame.onUpdate.Remove(OnUpdate);
    }

    protected virtual void UpdatePerFrame() { }

    public virtual void SetActive(bool bActive)
    {
        gameObject.SetActive(bActive);
    }

    public void SetPos(Vector3 pos)
    {
        cachedTransform.position = pos;      
    }

    public Vector3 GetPos()
    {
        return cachedTransform.transform.position;
    }

    public void SetRotation(Quaternion rot, bool isSmooth = false,float ratio = 1.0f)
    {
        if(isSmooth == true)
        {
            cachedTransform.rotation = Quaternion.Lerp(cachedTransform.rotation, rot, ratio);
            return;
        }

        cachedTransform.rotation = rot;
    }

    public virtual void SetObjState(eObjectState state)  { }

    public void SetParent(Transform tf)
    {
        cachedTransform.parent = tf;
    }

    public float GetBRadius() 
    {
        if (col == null) return 1f;

        return col.bounds.extents.magnitude;
    }

    public Vector3 GetCenterPos()
    {
        return GetPos() + GetBRadius() * Vector3.up;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (triggerEvent != null) triggerEvent(collider, this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collisionEvent != null) collisionEvent(collision, this);
    }

    private void OnUpdate()
    {
        if (!initialized) return;

        UpdatePerFrame();
    }
}
