using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface EAAIObject
{
    bool Tag { get; set; }

    void UnTagging();
    void Tagging();
    float GetBRadius();
    void SetBRadius(float r);
    Vector3 GetPos();
    void SetPos(Vector3 vPos);
    Vector3 GetHeading();
    void SetHeading(Vector3 newHeading);
}


