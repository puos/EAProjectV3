using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeTest : MonoBehaviour
{
    [SerializeField] private Transform cubeParent = null;

    [SerializeField] private int tileX = 10;
    [SerializeField] private int tileZ = 10;

    private MazeGen mazeGen;

    // Start is called before the first frame update
    void Start()
    {
        MakeMaze();
    }

    private void MakeMaze()
    {
        float widthX = 10f;
        float widthZ = 10f;

        float hTileX = widthX * 0.5f;
        float hTileZ = widthZ * 0.5f;

        Vector3 offset = Vector3.zero;
        offset.x = tileX * hTileX;
        offset.z = tileZ * hTileZ;

        if (mazeGen == null) mazeGen = new MazeGen();
        MazeGen.CTileAttrib[] maze = mazeGen.CreateMap(tileX, tileZ);

        //for(int i = 0; i < maze.Length; ++i)
        //{
        //    MazeGen.CTileAttrib x = maze[i];
        //    CCube cube = cubeManager.Get();
        //    Vector3 pos = new Vector3(x.mX * widthX + hTileX, 0f, x.mY * widthZ + hTileZ);
        //    pos -= offset;
        //    cube.name = $"[{x.mX},{x.mY}]";
        //    cube.SetPos(pos);
        //    cube.Initialize(x, tileX, tileZ);
        //    cube.SetActive(true);
        //    cube.transform.SetParent(cubeParent);
        //}
    }
}
