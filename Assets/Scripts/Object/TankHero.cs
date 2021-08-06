using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[EASceneInfo(typeof(TankHero))]
public class TankHero : EASceneLogic
{
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

    //[SerializeField] private GameJoystick moveJoystick = null;
    //[SerializeField] private GameJoystick attackJoystick = null;
    //[SerializeField] private CameraFollow cameraFollow = null;
    //[SerializeField] private CubeManager cubeManager = null;
    //[SerializeField] private Transform cubeParent = null;

    private int _TileX = 5;
    private int _TileY = 5;

    //protected void Initialize()
    //{
    //    base.Initialize();

    //    ////fxManager.Initialize();
    //    //moveJoystick.Initialize();
    //    //moveJoystick.gameObject.SetActive(false);
    //    //moveJoystick.onEndEvent = () =>
    //    //{
    //    //    if (hero != null) hero.Stop();
    //    //};

    //    //attackJoystick.Initialize();
    //    //attackJoystick.gameObject.SetActive(false);
    //    //attackJoystick.onEndEvent = () =>
    //    //{
    //    //    //CFx fx = fxManager.StartFx(EFxTag.FireFx);
    //    //    //fx.SetPos(hero.GetPos());
    //    //    if (hero != null) hero.StartFire();
    //    //};

    //    //unitList.Clear();
    //    //enemies.Clear();
    //    //users.Clear();

    //    //ChangeState(State.Init);
    //}

    //protected override bool ChangeState(State newState)
    //{
    //    bool changed = base.ChangeState(newState);

    //    if (changed == false)
    //        return false;

    //    switch(state)
    //    {
    //        case State.Init:
    //            {
    //                unitList.Clear();

    //                GameObject obj = null;
                    
    //                //EACObjManager.instance
                    
    //                //hero = 

    //                //hero.triggerEvent = (Collider c, EAObject myObj) =>
    //                //{
    //                //    CBullet bullet = c.gameObject.GetComponent<CBullet>();

    //                //    if (bullet == null) return;
    //                //    if (bullet.ownerId == hero.Id) return;

    //                //    bullet.Release();
    //                //    myObj.Release();

    //                //    //CFx fx = fxManager.StartFx(EFxTag.HitTankFx);
    //                //    //fx.SetPos(hero.GetPos());

    //                //    myObj.Release();

    //                //    ChangeState(State.Result);
    //                //};

    //                //hero.collisionEvent = (Collision c, EAObject myObj) => 
    //                //{
    //                //    EAActor unit = c.gameObject.GetComponent<EAActor>();

    //                //    if (unit == null) return;

    //                //    hero.Stop();
    //                //};

    //                //unitList.Add(hero);
    //                //users.Add(hero);

    //                //obj = Instantiate(hqTemplate);
    //                //target = obj.GetComponent<CTarget>();
    //                //target.Initialize();
                
    //                //target.triggerEvent = (Collider c, EAObject myObj) => 
    //                //{
    //                //    CBullet bullet = c.gameObject.GetComponent<CBullet>();

    //                //    if (bullet == null) return;

    //                //    bullet.Release();
    //                //    myObj.Release();

    //                //    //CFx fx = fxManager.StartFx(EFxTag.HitTankFx);
    //                //    //fx.SetPos(hero.GetPos());

    //                //    myObj.SetObjState(eObjectState.CS_DEAD);

    //                //    users.Remove(target);
    //                //    unitList.Remove(target);

    //                //    ChangeState(State.Result);
    //                //};


    //                //unitList.Add(target);
    //                //users.Add(target);

    //                //if(cameraFollow != null) cameraFollow.Initialize(hero.transform);

    //                //MakeMaze(_TileX, _TileY);

    //                //MazeGen.CTileAttrib tile = SpawnUnit(target, false);
    //                //if (tile != null) tile.cubeObject.DoorAllClose();

    //                //SpawnUnit(hero);
    //                //ChangeState(State.Start);

    //            }
    //            break;

    //        case State.Start:
    //            {
    //                tankSpawnCount = 1;
    //                updateWaveWaitTime = 0;
    //                enemiesCount = 0;
    //                enemies.Clear();
    //            }
    //            break;


    //        case State.Result:
    //            {
    //                for(int i = 0; i < enemies.Count; ++i)
    //                {
    //                    enemies[i].Release();
    //                    Destroy(enemies[i].gameObject);
    //                }

    //                for(int i = 0; i < users.Count; ++i)
    //                {
    //                    users[i].Release();
    //                }

    //                enemies.Clear();
    //                users.Clear();

    //                hero.Release();
    //                hero.SetActive(false);

    //                target.Release();
    //                target.SetActive(false);
    //            }
    //            break;
    //    }

    //    return true;
    //}

    //protected override void RunState()
    //{
    //    base.RunState();

    //    switch (state)
    //    {
    //        case State.Start:
    //            {
    //                if (moveJoystick != null) HeroMove();
    //                if (attackJoystick != null) HeroAttack();

    //                SpawnEnemies();
    //            }
    //            break;
    //    }
    //}

    //private void HeroMove()
    //{
    //    Vector3 direction = moveJoystick.GetInputDirection();
    //    direction.z = direction.y;
    //    direction.y = 0;
    //    direction.Normalize();

    //    Vector3 pos = hero.GetPos() + direction * 3.0f;

    //    bool changed = Vector3.Dot(hero.rb.velocity, direction) < 0 ? true : false;

    //    if (direction.magnitude > 0) Debug.DrawLine(hero.GetPos(), pos, Color.red);

    //    if (changed == true) hero.Stop();
    //    if (direction.magnitude > 0) hero.MoveTo(pos);
    //}

    //private void HeroAttack()
    //{
    //    Vector3 direction = attackJoystick.GetInputDirection();
    //    direction.z = direction.y;
    //    direction.y = 0;
    //    direction.Normalize();

    //    if (direction.magnitude > 0) hero.SetRotationSub(Quaternion.LookRotation(direction , Vector3.up));
    //}

    //private void MakeMaze(int tileX,int tileZ)
    //{
    //    while(0 != cubeParent.childCount)
    //    {
    //        Transform cubeChild = cubeParent.GetChild(0);
    //        CCube cube = cubeChild.GetComponent<CCube>();
    //        cube.Release();
    //    }

    //    float widthX = 10f;
    //    float widthZ = 10f;

    //    float hTileX = widthX * 0.5f;
    //    float hTileZ = widthZ * 0.5f;

    //    Vector3 offset = Vector3.zero;
    //    offset.x = tileX * hTileX;
    //    offset.z = tileZ * hTileZ;

    //    if (mazeGen == null) mazeGen = new MazeGen();
    //    maze = mazeGen.CreateMap(tileX, tileZ);

    //    //for (int i = 0; i < maze.Length; ++i)
    //    //{
    //    //    MazeGen.CTileAttrib x = maze[i];
    //    //    CCube cube = cubeManager.Get();
    //    //    Vector3 pos = new Vector3(x.mX * widthX + hTileX, 0f, x.mY * widthZ + hTileZ);
    //    //    pos -= offset;
    //    //    cube.name = $"[{x.mX},{x.mY}]";
    //    //    cube.SetPos(pos);
    //    //    cube.Initialize(x, tileX, tileZ);
    //    //    cube.SetActive(true);
    //    //    cube.transform.SetParent(cubeParent);

    //    //    cube.SetTriggerEvent((Collider c, EAObject myObj) =>
    //    //    {
    //    //        CBullet bullet = c.gameObject.GetComponent<CBullet>();

    //    //        if (bullet == null) return;

    //    //        bullet.Release();

    //    //        myObj.SetActive(false);

    //    //        CFx fx = fxManager.StartFx(EFxTag.HitWallFx);
    //    //        fx.SetPos(bullet.GetPos());

    //    //    });
    //    //}
    //}

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

            //GameObject obj = Instantiate(enemyTemplate);
            //CEnemy enemy = obj.GetComponent<CEnemy>();
            //enemy.Initialize();
            //enemy.SetSpeed(3.5f);
            //enemy.SetObjState(eObjectState.CS_SETENTITY);
            //enemy.SetActiveFire(true);

            //enemy.triggerEvent = (Collider c, EAObject myObj) => 
            //{
            //    CBullet bullet = c.gameObject.GetComponent<CBullet>();

            //    if (bullet == null) return;
            //    if (bullet.ownerId == enemy.Id) return;

            //    bullet.Release();

            //    CFx fx = fxManager.StartFx(EFxTag.HitTankFx);
            //    fx.SetPos(enemy.GetPos());

            //    if (bullet.ownerId != myHeroId) return;

            //    myObj.SetActive(false);
            //    myObj.SetState(EAObject.eObjectState.DEAD);

            //    enemies.Remove(enemy);
            //    unitList.Remove(enemy);

            //    Destroy(enemy.gameObject);

            //    if (enemiesCount == tankSpawnCount)
            //    {
            //        if (enemies.Count == 0) ChangeState(State.Result); 
            //    }
            //};

            //enemy.collisionEvent = (Collision c, EAObject myObj) =>
            //{
            //    EAActor unit = c.gameObject.GetComponent<EAActor>();

            //    if (unit == null) return;

            //    enemy.Stop();
            //};

            //Move(enemy);
            //Chasing(enemy);
            //Attack(enemy);

            ////enemy.SetId(enemies.Count);
            //SpawnUnit(enemy);
            //enemy.ChangeFSMState(CEnemy.EFSMState.Move);
            //enemies.Add(enemy);
            //unitList.Add(enemy);

            enemiesCount = enemiesCount + 1;
        }
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
            
            if(m == null)
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
           
            DebugCircle(enemy.GetCenterPos(), Vector3.up, Color.red, 10f, 1f);

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

    private void Chasing(CEnemy enemy)
    {
        enemy.StateAdd(CEnemy.EFSMState.Chasing, () =>
        {
            if(enemy.target == null)
            {
                enemy.ChangeFSMState((int)CEnemy.EFSMState.Move);
                return;
            }

            Debug.Log("enemy chase :" + enemy + " -> " + enemy.target.Id);

            Vector3 targetPos = enemy.target.GetPos();
            enemy.Stop();
            enemy.SetRotation(Quaternion.LookRotation((targetPos - enemy.GetPos()).normalized, Vector3.up));

            Debug.Log("before enemy pos : " + enemy.GetPos() + " target pos : " + targetPos);

            enemy.MoveTo(targetPos);
        });

        float updateTime = Time.time;

        enemy.StateAdd(CEnemy.EFSMState.Chasing, () =>
        {
            if (Time.time - updateTime <= 0) return;

            updateTime = Time.time + 2f;

            DebugCircle(enemy.GetCenterPos(), Vector3.up, Color.black , 10f, 2f);

            if (Vector3.Distance(enemy.target.GetPos(), enemy.GetPos()) > 12.0f)
            {
                enemy.ChangeFSMState(CEnemy.EFSMState.Move);
                return;
            }

            if (Vector3.Distance(enemy.target.GetPos(), enemy.GetPos()) > 10.0f)
            {
                return;
            }

            
            enemy.ChangeFSMState(CEnemy.EFSMState.Attack);

        });
    }
   
    private void Attack(CEnemy enemy)
    {
        float updateTime = Time.time;

        enemy.UpdateAdd(CEnemy.EFSMState.Attack, () =>
        {
             Vector3 dir = enemy.target.GetPos() - enemy.GetPos();
            dir.Normalize();

            enemy.Stop();
            enemy.SetRotationSub(Quaternion.LookRotation(dir, Vector3.up));
            enemy.StartFire();
        });

        enemy.UpdateAdd(CEnemy.EFSMState.Attack, () => 
        {
            if (Time.time - updateTime <= 0) return;
            updateTime = Time.time + 3f;

            if (Vector3.Distance(enemy.target.GetPos(), enemy.GetPos()) >= 10.0f)
            {
                enemy.ChangeFSMState((int)CEnemy.EFSMState.Chasing);
                return;
            }
                       
            enemy.CmdState(CEnemy.EFSMState.Attack);

        });
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

    public static void DebugCircle(Vector3 position,Vector3 up , Color color ,float radius = 1.0f, float duration = 0)
    {
        Vector3 _up = up.normalized * radius;
        Vector3 _forward = Vector3.Slerp(_up, -_up, 0.5f);
        Vector3 _right = Vector3.Cross(_up, _forward).normalized * radius;

        Matrix4x4 matrix = new Matrix4x4();

        matrix[0] = _right.x;
        matrix[1] = _right.y;
        matrix[2] = _right.z;

        matrix[4] = _up.x;
        matrix[5] = _up.y;
        matrix[6] = _up.z;

        matrix[8] = _forward.x;
        matrix[9] = _forward.y;
        matrix[10] = _forward.z;

        Vector3 _lastPoint = position + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)));
        Vector3 _nextPoint = Vector3.zero;

        color = (color == default(Color)) ? Color.white : color;

        for(var i = 0; i < 91; ++i)
        {
            _nextPoint.x = Mathf.Cos((i * 4) * Mathf.Deg2Rad);
            _nextPoint.z = Mathf.Sin((i * 4) * Mathf.Deg2Rad);
            _nextPoint.y = 0;

            _nextPoint = position + matrix.MultiplyPoint3x4(_nextPoint);

            Debug.DrawLine(_lastPoint, _nextPoint, color, duration, true);
            _lastPoint = _nextPoint;
        }
    }

    public void OnPointerMoveDown(BaseEventData eventData)
    {
        //if (moveJoystick.gameObject.activeSelf == false) moveJoystick.gameObject.SetActive(true);
        //moveJoystick.GetRectTransform().position = ((PointerEventData)eventData).position;
        //moveJoystick.Down((PointerEventData)eventData);
    }

    public void OnPointerMoveUp(BaseEventData eventData)
    {
        //moveJoystick.Up((PointerEventData)eventData);
        //moveJoystick.GetRectTransform().localPosition = Vector3.zero;
    }

    public void OnMoveDrag(BaseEventData eventData)
    {
        //moveJoystick.Drag((PointerEventData)eventData);
    }

    public void OnPointerAttackDown(BaseEventData eventData)
    {
        //if (attackJoystick.gameObject.activeSelf == false) attackJoystick.gameObject.SetActive(true);
        //attackJoystick.GetRectTransform().position = ((PointerEventData)eventData).position;
        //attackJoystick.Down((PointerEventData)eventData);
    }

    public void OnPointerAttackUp(BaseEventData eventData)
    {
        //attackJoystick.Up((PointerEventData)eventData);
        //attackJoystick.GetRectTransform().localPosition = Vector3.zero;
    }

    public void OnAttackDrag(BaseEventData eventData)
    {
        //attackJoystick.Drag((PointerEventData)eventData);
    }

    protected override void OnInit()
    {
        EA_CCharUser mainPlayer = EACObjManager.instance.GetMainPlayer();

        ObjectInfo obj = mainPlayer.GetObjInfo();
        obj.m_ModelTypeIndex = "Player";
        obj.m_objClassType = typeof(CHero);
        mainPlayer.ResetInfo(eObjectState.CS_SETENTITY);


        //                //hero = 

        //                //hero.triggerEvent = (Collider c, EAObject myObj) =>
        //                //{
        //                //    CBullet bullet = c.gameObject.GetComponent<CBullet>();

        //                //    if (bullet == null) return;
        //                //    if (bullet.ownerId == hero.Id) return;

        //                //    bullet.Release();
        //                //    myObj.Release();

        //                //    //CFx fx = fxManager.StartFx(EFxTag.HitTankFx);
        //                //    //fx.SetPos(hero.GetPos());

        //                //    myObj.Release();

        //                //    ChangeState(State.Result);
        //                //};

        //                //hero.collisionEvent = (Collision c, EAObject myObj) => 
        //                //{
        //                //    EAActor unit = c.gameObject.GetComponent<EAActor>();

        //                //    if (unit == null) return;

        //                //    hero.Stop();
        //                //};
    }

    protected override IEnumerator OnPostInit()
    {
        yield return null;
    }

    protected override void OnUpdate()
    {
       
    }

    protected override void OnClose()
    {
        
    }
}
