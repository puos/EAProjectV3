using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCube : EAObject
{
    public enum DIR { UP , DOWN , LEFT , RIGHT}

    [SerializeField] private CSubCube[] subCubes;

    MazeGen.CTileAttrib cube = new MazeGen.CTileAttrib();

    public void Initialize(MazeGen.CTileAttrib cube , int tileX, int tileY)
    {
        this.cube = cube;
        this.cube.cubeObject = this;

        Initialize();

        for(int i = 0; i < subCubes.Length; ++i)
        {
            int mX = this.cube.mX;
            int mY = this.cube.mY;

            if (i == (int)DIR.UP) mY = mY + 1;
            if (i == (int)DIR.DOWN) mY = mY - 1;
            if (i == (int)DIR.LEFT) mX = mX - 1;
            if (i == (int)DIR.RIGHT) mX = mX + 1;

            subCubes[i].Initialize(mX, mY);
        }

        subCubes[(int)DIR.UP].SetActive(!this.cube.top);
        subCubes[(int)DIR.DOWN].SetActive(!this.cube.bottom);
        subCubes[(int)DIR.LEFT].SetActive(!this.cube.left);
        subCubes[(int)DIR.RIGHT].SetActive(!this.cube.right);

        if (cube.mY == (tileY - 1)) subCubes[(int)DIR.UP].Lock(true);
        if (cube.mY == 0) subCubes[(int)DIR.DOWN].Lock(true);
        if (cube.mX == (tileX - 1)) subCubes[(int)DIR.RIGHT].Lock(true);
        if (cube.mX == 0) subCubes[(int)DIR.LEFT].Lock(true);
    }

    public void SetTriggerEvent(Action<Collider,EAObject> triggerEvent) 
    {
        Array.ForEach(subCubes, x =>
        {
            x.triggerEvent = triggerEvent;
        });
    }

    public List<CSubCube> IsOpenDoor() 
    {
        List<CSubCube> cubeList = new List<CSubCube>();
        Array.ForEach(subCubes, x =>
        {
            if (x.IsOpen()) cubeList.Add(x);
        });
        return cubeList;
    }

    public void DoorAllClose() 
    {
        Array.ForEach(subCubes, x =>
        {
            x.SetActive(true);
        });
    }


    public List<CSubCube> GetRayCubes(Vector3 start, Vector3 end)
    {
        Vector3 to = (end - start).normalized;
        float distance = (end - start).magnitude;

        Ray ray = new Ray(start, to);

        Debug.DrawLine(start, end, Color.red, 15f);

        List<CSubCube> cubeList = new List<CSubCube>();
        Array.ForEach(subCubes, x =>
        {
            if (x.IsOpen()) return;

            bool check = x.col.Raycast(ray, out RaycastHit hitInfo, distance);

            if (check) cubeList.Add(x);
        });
        return cubeList;
    }
}
