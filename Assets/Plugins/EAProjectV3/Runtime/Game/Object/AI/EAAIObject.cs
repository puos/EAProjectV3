using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
   public Vector3 GetHeading();
   public void SetHeading(Vector3 newHeading, bool isSmooth = false);
}


