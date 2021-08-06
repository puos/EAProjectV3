using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBullet : EAObject
{
    private Vector3 vStartPos = Vector3.zero;
    private Vector3 vStartDir = Vector3.zero;
    private Transform target  = null;

    private float lifeTime = 0f;
    private float killDistance = 0f;

    public int ownerId { get; set; }

    public override void Initialize()
    {
        base.Initialize();
        col.isTrigger = true;
        target = null;
    }

    public void Move(Vector3 targetDir,float speed = 1f, Transform offset = null , float lifeTime = 0f, float killDistance = 0f)
    {
        vStartPos = (offset == null) ? cachedTransform.position : offset.position;
        vStartDir = targetDir;

        this.lifeTime = lifeTime;
        this.killDistance = killDistance;

        rb.velocity = vStartDir * speed;
        SetPos(vStartPos);
    }

    protected override void UpdatePerFrame()
    {
        base.UpdatePerFrame();

        if (killDistance == 0) return;

        if(Vector3.Distance(cachedTransform.position , vStartPos) > killDistance)
        {
            Release();
        }
    }

}
