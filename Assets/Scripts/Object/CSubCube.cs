using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSubCube : EAObject
{
    public int mX { get; private set; }
    public int mY { get; private set; }

    private bool bLock = false;

    public override void SetActive(bool Active) { if (bLock == false) gameObject.SetActive(Active); }

    public void Lock(bool bLock) { this.bLock = bLock; }

    public bool IsOpen() { return !gameObject.activeSelf; }

    public void Initialize(int mX, int mY)
    {
        base.Initialize();

        this.mX = mX;
        this.mY = mY;
    }
}
