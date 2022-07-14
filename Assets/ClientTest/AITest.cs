using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[EASceneInfo(typeof(AITest))]
public class AITest : EASceneLogic
{
    private List<EAPathMaker> pathMakers = new List<EAPathMaker>();
    private SpawnPoint spawns = null;
    private int unitNumber = 2;
    private float floorHeight = -1f;
    private readonly List<DemoUnit> units = new List<DemoUnit>();

    protected override void OnInit()
    {
        pathMakers.AddRange(FindObjectsOfType<EAPathMaker>(true));
        for (int i = 0; i < pathMakers.Count; ++i) pathMakers[i].Initialize();

        spawns = FindObjectOfType<SpawnPoint>();
        spawns.Initialize();

        unitNumber = dataAdapter.GetInt(nameof(unitNumber));
        floorHeight = dataAdapter.GetFloat(nameof(floorHeight));

        for (int i = 0; i < unitNumber; ++i)
        {
            Vector3 pos = spawns.GetRandomPoint();

            IEnumerator<EAAIObject> obstacles = units.GetEnumerator();

            for(int j = 0; j < 10; ++j)
            {
                if (!EAGameAIPhysicWorld.instance.Overlapped(pos, 1f, obstacles, 2f)) break;
                pos = spawns.GetRandomPoint();
            }

            DebugExtension.DebugPoint(pos, Color.black, 1, 15f);

            pos.y = floorHeight;
            var instance = DemoUnit.Clone();
            instance.SetPos(pos);
            instance.Name = $"unit {i}";
            units.Add(instance);
        }

    }

    private void Flee(int index,int tindex)
    {
        if (index >= units.Count) return;
        if (tindex >= units.Count) return;

        EASteeringBehaviour steering = units[index].GetComponent<EASteeringBehaviour>();
        steering.Get<Flee>(Steering.behaviour_type.flee).SetVTarget(units[tindex].GetPos());
        steering.FleeOn();
        steering.ResetInfo();
    }

    private void Evade(int index,int tindex)
    {
        if (index >= units.Count) return;
        if (tindex >= units.Count) return;

        EASteeringBehaviour steering = units[index].GetComponent<EASteeringBehaviour>();
        steering.Get<Evade>(Steering.behaviour_type.evade).SetTAgent(units[tindex]);
        steering.EvadeOn();
        steering.ResetInfo();
        steering.Get<CollisionAvoidance>(Steering.behaviour_type.agent_avoidance).SetBeweenDist(2f);
    }

    private void UnitsStop()
    {
        for(int i = 0; i < units.Count; ++i) units[i].Stop();
    }

    private void FollowPath(EAAIPath aiPath)
    {
        for(int i = 0; i < units.Count; ++i)
        {
            EASteeringBehaviour steering = units[i].GetComponent<EASteeringBehaviour>();
            steering.Get<FollowPath>(Steering.behaviour_type.follow_path).SetAIPath(aiPath);
            steering.FollowOn();
            steering.ResetInfo();
            steering.AgentAvoidOff();
        }
    }

    private void FollowPath2(EAAIPath aiPath)
    {
        for(int i = 0; i < units.Count; ++i)
        {
            EASteeringBehaviour steering = units[i].GetComponent<EASteeringBehaviour>();
            steering.Get<FollowPathV2>(Steering.behaviour_type.follow_path_v2).SetAIPath(aiPath);
            steering.Get<FollowPathV2>(Steering.behaviour_type.follow_path_v2).LoopOn();
            steering.Follow2On();
            steering.ResetInfo();
        }
    }

    private void Pursuit(int index,int tindex)
    {
        if (index >= units.Count) return;
        if (tindex >= units.Count) return;

        EASteeringBehaviour steering = units[index].GetComponent<EASteeringBehaviour>();
        steering.Get<Pursue>(Steering.behaviour_type.pursuit).SetTAgent(units[tindex]);
        steering.PursuitOn();
        steering.ResetInfo();
    }

    private void Hide(int index,int tindex)
    {
        if (index >= units.Count) return;
        if (tindex >= units.Count) return;

        EASteeringBehaviour steering = units[index].GetComponent<EASteeringBehaviour>();
        steering.Get<Hide>(Steering.behaviour_type.hide).SetTAgent(units[tindex]);
        steering.Get<Hide>(Steering.behaviour_type.hide).SetTactics(Steering.TacticsType.TACTICS_ALLY);
        steering.HideOn();
        steering.ResetInfo();
    }

    private void PursuitOffset(int index,int tindex,float offsetDist)
    {
        if (index >= units.Count) return;
        if (tindex >= units.Count) return;

        EASteeringBehaviour steering = units[index].GetComponent<EASteeringBehaviour>();
        steering.Get<PursueOffset>(Steering.behaviour_type.pursuitOffset).SetTAgent(units[tindex]);
        steering.Get<PursueOffset>(Steering.behaviour_type.pursuitOffset).SetOffset(Vector3.back * offsetDist);
        steering.PursuitOffsetOn();
        steering.ResetInfo();
    }

    private void Cohesion()
    {
        for(int i = 0; i < units.Count; ++i)
        {
            EASteeringBehaviour steering = units[i].GetComponent<EASteeringBehaviour>();
            steering.Get<Cohesion>(Steering.behaviour_type.cohesion).SetTactics(Steering.TacticsType.TACTICS_ALLY);
            steering.CohesionOn();
            steering.ResetInfo();
        }
    }

    private void Separation()
    {
        for(int i = 0; i < units.Count; ++i)
        {
            EASteeringBehaviour steering = units[i].GetComponent<EASteeringBehaviour>();
            steering.Get<Separation>(Steering.behaviour_type.seperation).SetTactics(Steering.TacticsType.TACTICS_ALLY);
            steering.Get<Separation>(Steering.behaviour_type.seperation).SepMaxDist(5f);
            steering.SeperationOn();
            steering.ResetInfo();
        }
    }

    public void OnGUI()
    {
        float width = 200;

        GUILayoutOption w = GUILayout.Width(width);
        GUILayoutOption h = GUILayout.Height(30);

        GUILayout.BeginArea(new Rect(Screen.width - width, 0, width, 50 * 20.0f));

        if(GUILayout.Button("Wander1",w,h))
        {
            UnitsStop();

            for(int i = 0; i < units.Count; ++i)
            {
                EASteeringBehaviour steering = units[i].GetComponent<EASteeringBehaviour>();
                steering.Wander1On();
            }
        }
        
        if(GUILayout.Button("Wander2",w,h))
        {
            UnitsStop();

            for(int i = 0; i < units.Count; ++i)
            {
                EASteeringBehaviour steering = units[i].GetComponent<EASteeringBehaviour>();
                steering.Wander2On();
            }
        }

        if(GUILayout.Button("flee",w,h))
        {
            UnitsStop();
            Flee(0, 1);
            Flee(1, 0);
        }

        if(GUILayout.Button("Evade",w,h))
        {
            UnitsStop();
            Evade(0, 1);
            Evade(1, 0);
        }

        if(GUILayout.Button("followPath",w,h))
        {
            UnitsStop();
            EAAIPath aiPath = AIPathManager.instance.GetPath("aitest01");
            if (aiPath != null) FollowPath(aiPath);
        }

        if(GUILayout.Button("followPath2",w,h))
        {
            UnitsStop();
            EAAIPath aiPath = AIPathManager.instance.GetPath("aitest01");
            if (aiPath != null) FollowPath2(aiPath);
        }

        if(GUILayout.Button("pursuit",w,h))
        {
            UnitsStop();
            Evade(0, 1);
            Pursuit(1, 0);
        }

        if(GUILayout.Button("pursuit_offset",w,h))
        {
            UnitsStop();
            Evade(0, 1);
            PursuitOffset(1, 0, 3f);
        }

        if(GUILayout.Button("cohesion",w,h))
        {
            UnitsStop();
            Cohesion();
        }

        if(GUILayout.Button("separation",w,h))
        {
            UnitsStop();
            Separation();
        }

        if(GUILayout.Button("hide",w,h))
        {
            UnitsStop();
            Hide(0, 1);
        }

        GUILayout.EndArea();
    }


    protected override void OnUpdate()
    {
    }

    protected override void OnClose()
    {
    }



}
