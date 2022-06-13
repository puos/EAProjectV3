using UnityEngine;

public interface EAAIObject
{
   public bool Tag { get; set; }

   public void UnTagging();
   public void Tagging();
   public float GetBRadius();
   public void SetBRadius(float r);
   public Vector3 GetPos();
   public void SetPos(Vector3 vPos);
   public Vector3 GetColliderPos();
   public Vector3 GetSide();
   public Quaternion GetRotation();
   public void SetRotation(Quaternion rot);
   public void SetRotation(Quaternion rot,float ratio);
}


