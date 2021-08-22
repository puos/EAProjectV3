﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[EASceneInfo(typeof(TankHero))]
public class TankHero : EASceneLogic
{
    public enum State { None, Init, Start, End, Result, Exit }
    
    private CHero hero = null;
    private CTarget target = null;

    private List<EAActor> enemies = new List<EAActor>();
    private List<EAActor> users = new List<EAActor>();
    private List<EAActor> unitList = new List<EAActor>();

    private MazeGen mazeGen;
    private MazeGen.CTileAttrib[] maze = null;

    private float waveWaitTime = 3f;
    private float updateWaveWaitTime = 0f;
    private float tankSpawnCount = 0;
    private int enemiesCount = 0;
    private int myHeroId = 10000;

    public State state { get; private set; }

    private CameraFollow cameraFollow = null;
    private CubeManager cubeManager = null;
    private Transform cubeParent    = null;

    private IngameUI inGameUi = null;

    private int _TileX = 5;
    private int _TileY = 5;

    protected override void OnInit()
    {
        Input.multiTouchEnabled = true;

        hero = CHero.MainPlayerCreate();

        inGameUi = UIManager.instance.OpenPage<IngameUI>(TankUIPage.ingameUi);

        inGameUi.SetMoveEndEvent(() =>
        {
            if (hero != null) hero.Stop();
        });

        inGameUi.SetAttackEndEvent(() => 
        {
            if (hero != null) hero.StartFire();
        });

        unitList.Clear();
        enemies.Clear();
        users.Clear();

        {
            GameObject obj = GetData("CubeParent");
            if (obj != null) cubeParent = obj.transform;
        }

        {
            GameObject obj = GetData("CameraFollow");
            if (obj != null) cameraFollow = obj.GetComponent<CameraFollow>();
        }
        
    }

    protected override IEnumerator OnPostInit()
    {
        ChangeState(State.Init);
        yield return null;
    }

    protected override void OnUpdate()
    {
        RunState();
    }

    protected override void OnClose()
    {
    }

    protected bool ChangeState(State newState)
    {
        if (newState != State.Start && newState <= state)
            return false;

        state = newState;

        Debug.Log("ChangeState: " + state);

        switch (state)
        {
            case State.Init:
                {
                    unitList.Clear();
                                                        
                    unitList.Add(hero);
                    users.Add(hero);

                    //target = CTarget.Clone();
                    //unitList.Add(target);
                    //users.Add(target);

                    if(cameraFollow != null) cameraFollow.Initialize(hero.transform);

                    MakeMaze(_TileX, _TileY);

                    //MazeGen.CTileAttrib tile = SpawnUnit(target, false);
                    //if (tile != null) tile.cubeObject.DoorAllClose();

                    SpawnUnit(hero);
                    ChangeState(State.Start);

                }
                break;

            case State.Start:
                {
                    tankSpawnCount = 1;
                    updateWaveWaitTime = 0;
                    enemiesCount = 0;
                    enemies.Clear();
                }
                break;


            case State.Result:
                {
                    for (int i = 0; i < enemies.Count; ++i)
                    {
                        enemies[i].Release();
                        Destroy(enemies[i].gameObject);
                    }

                    for (int i = 0; i < users.Count; ++i)
                    {
                        users[i].Release();
                    }

                    enemies.Clear();
                    users.Clear();

                    hero.Release();
                    hero.SetActive(false);

                    target.Release();
                    target.SetActive(false);
                }
                break;
        }

        return true;
    }

    protected void RunState()
    {
        switch (state)
        {
            case State.Start:
                {
                    HeroMove();
                    HeroAttack();
                    SpawnEnemies();
                }
                break;
        }
    }

    private void HeroMove()
    {
        Vector3 direction = inGameUi.GetMoveDirection();
        direction.z = direction.y;
        direction.y = 0;
        direction.Normalize();

        Vector3 pos = hero.GetPos() + direction * 3.0f;

        bool changed = Vector3.Dot(hero.rb.velocity, direction) < 0 ? true : false;

        if (direction.magnitude > 0) Debug.DrawLine(hero.GetPos(), pos, Color.red);

        if (changed == true) hero.Stop();
        if (direction.magnitude > 0) hero.MoveTo(pos);
    }

    private void HeroAttack()
    {
        Vector3 direction = inGameUi.GetAttackDirection();
        direction.z = direction.y;
        direction.y = 0;
        direction.Normalize();

        if (direction.magnitude > 0) hero.SetRotationSub(Quaternion.LookRotation(direction, Vector3.up));
    }

    private void MakeMaze(int tileX, int tileZ)
    {
        while (0 != cubeParent.childCount)
        {
            Transform cubeChild = cubeParent.GetChild(0);
            CCube cube = cubeChild.GetComponent<CCube>();
            cube.Release();
        }

        float widthX = 10f;
        float widthZ = 10f;

        float hTileX = widthX * 0.5f;
        float hTileZ = widthZ * 0.5f;

        Vector3 offset = Vector3.zero;
        offset.x = tileX * hTileX;
        offset.z = tileZ * hTileZ;

        if (mazeGen == null) mazeGen = new MazeGen();
        maze = mazeGen.CreateMap(tileX, tileZ);

        for (int i = 0; i < maze.Length; ++i)
        {
            MazeGen.CTileAttrib x = maze[i];
            CCube cube = CCube.Clone();
            Vector3 pos = new Vector3(x.mX * widthX + hTileX, 0f, x.mY * widthZ + hTileZ);
            pos -= offset;
            cube.name = $"[{x.mX},{x.mY}]";
            cube.SetPos(pos);
            cube.Initialize(x, tileX, tileZ);
            cube.SetActive(true);
            cube.transform.SetParent(cubeParent);
        }
    }

    private MazeGen.CTileAttrib SpawnUnit(EAActor unit, bool isRandom = true)
    {
        int tileX = _TileX;
        int tileZ = _TileY;

        int randomX = UnityEngine.Random.Range(0, tileX);
        int randomZ = UnityEngine.Random.Range(0, tileZ);

        MazeGen.CTileAttrib m = Array.Find(maze, x => { return (x.mX == randomX) && (x.mY == randomZ); });

        if(m != null)
        {
            EAActor overUnit = null;

            Vector3 offsetPos = m.cubeObject.GetPos();
            if (isRandom) offsetPos = offsetPos + UnityEngine.Random.onUnitSphere * 2f;

            float height = m.cubeObject.GetPos().y;
            offsetPos.y = height;

            overUnit = unitList.Find(x =>
            {
                Vector3 to = x.GetPos() - offsetPos;
                float range = x.GetBRadius() + unit.GetBRadius();
                return (to.magnitude < range);
            });

            float time = Time.time;

            while(overUnit != null)
            {
                randomX = UnityEngine.Random.Range(0, tileX);
                randomZ = UnityEngine.Random.Range(0, tileZ);

                m = Array.Find(maze, x => { return (x.mX == randomX) && (x.mY == randomZ); });

                offsetPos = m.cubeObject.GetPos();
                if (isRandom) offsetPos = offsetPos + UnityEngine.Random.onUnitSphere * 2f;
                overUnit = unitList.Find(x =>
                {
                    Vector3 to = x.GetPos() - offsetPos;
                    float range = x.GetBRadius() + unit.GetBRadius();
                    return (to.magnitude < range);
                });

                if (Time.time - time > 1.0f) break;
            }

            offsetPos.y = unit.GetPos().y;
            unit.SetPos(offsetPos);
        }

        return m;
    }

    private void SpawnEnemies()
    {
        if (enemiesCount >= tankSpawnCount) return;

        if(updateWaveWaitTime < Time.time)
        {
            updateWaveWaitTime += waveWaitTime;

            if (updateWaveWaitTime < Time.time) updateWaveWaitTime = Time.time;

            CEnemy enemy = CEnemy.Clone();
            
            Move(enemy);
            SpawnUnit(enemy);
            enemy.ChangeFSMState(CEnemy.EFSMState.Move);
            enemies.Add(enemy);
            unitList.Add(enemy);

            enemiesCount = enemiesCount + 1;
        }
    }
        
    public List<EAActor> WithinViewRange(List<EAActor> list, EAActor unit , float closetRadius = 0f)
    {
        List<EAActor> units = new List<EAActor>();
        IEnumerator<EAActor> it = list.GetEnumerator();

        while(it.MoveNext())
        {
            EAActor curUnit = it.Current;
            float dist  = Vector3.Distance(unit.GetPos(), curUnit.GetPos());
            float range = closetRadius + curUnit.GetBRadius();

            if (object.ReferenceEquals(unit, curUnit)) continue;
            if (dist > range) continue;

            units.Add(curUnit);
        }

        return units;
    }

    private void Move(CEnemy enemy)
    {
        enemy.StateAdd(CEnemy.EFSMState.Move, () =>
        {
            enemy.SetTarget(null);

            MazeGen.CTileAttrib m = Array.Find(maze, x =>
            {
                return x.cubeObject.col.bounds.Contains(enemy.GetPos());
            });

            if (m == null)
            {
                enemy.ChangeFSMState(CEnemy.EFSMState.None);
                return;
            }

            List<CSubCube> doors = m.cubeObject.IsOpenDoor();
            int r = UnityEngine.Random.Range(0, doors.Count);
            MazeGen.CTileAttrib m2 = Array.Find(maze, x => { return (x.mX == doors[r].mX) && (x.mY == doors[r].mY); });
            Vector3 pos = m2.cubeObject.GetPos() + UnityEngine.Random.onUnitSphere * 2f;
            pos.y = enemy.GetPos().y;

            Debug.Log("enemey id :" + enemy.Id + $"cur[{m.mX},{m.mY}] => next[{m.mX},{m.mY}]");

            enemy.SetRotation(Quaternion.LookRotation((pos - enemy.GetPos()).normalized, Vector3.up));

            enemy.MoveTo(pos, () =>
            {
                enemy.CmdState(CEnemy.EFSMState.Move);
            });
        });

        float updateTime = Time.time;

        enemy.UpdateAdd(CEnemy.EFSMState.Move, () =>
        {
            if (Time.time - updateTime <= 0) return;

            updateTime = Time.time + 1f;

            List<EAActor> unit = WithinViewRange(users, enemy, 10f);

            if (unit.Count <= 0) return;

            DebugExtension.DebugCircle(enemy.GetCenterPos(), Vector3.up, Color.red, 10f, 1f);

            if (unit[0].Id != myHeroId)
            {
                enemy.SetTarget(unit[0]);
                enemy.ChangeFSMState(CEnemy.EFSMState.Chasing);
                return;
            }

            MazeGen.CTileAttrib m = Array.Find(maze, x =>
            {
                List<CSubCube> doors = x.cubeObject.GetRayCubes(unit[0].GetCenterPos(), enemy.GetCenterPos());
                if (doors.Count > 0) return true;
                return false;
            });

            if (m != null)
            {
                return;
            }

            enemy.SetTarget(unit[0]);
            enemy.ChangeFSMState((int)CEnemy.EFSMState.Chasing);
        });
    }

}
