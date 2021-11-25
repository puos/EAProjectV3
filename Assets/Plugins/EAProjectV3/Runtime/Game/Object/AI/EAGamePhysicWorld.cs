using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EAAIGroup
{
    private Vector3 m_vCrosshair = Vector3.zero;
    private List<EAAIAgent> m_aiAgents = new List<EAAIAgent>();
    private string name = string.Empty;
    public EAAIGroup(string id = "basic")
    {
        name = id;
        m_vCrosshair = Vector3.zero;
    }
    public List<EAAIAgent> Agents()
    {
        return m_aiAgents;
    }
    public Vector3 Crosshair()
    {
        return m_vCrosshair;
    }
    public void SetCrosshair(Vector3 p)
    {
        m_vCrosshair = p;
        for(int i = 0; i < m_aiAgents.Count;++i)
        {
            m_aiAgents[i].SetVTarget(p);
        }
    }
    public void AddAgent(EAAIAgent agent)
    {
        int idx = m_aiAgents.FindIndex(x => x.GetHashCode().Equals(agent.GetHashCode()));
        if(idx == -1)
        {
            m_aiAgents.Add(agent);
            agent.SetAIGroup(this);
        }
    }
    public void RemoveAgent(EAAIAgent agent)
    {
        m_aiAgents.Remove(agent);
    }
}

public class EAGamePhysicWorld
{
    static EAGamePhysicWorld _instance = null;

    public static EAGamePhysicWorld instance
    {
        get 
        {
            if (_instance == null) _instance = new EAGamePhysicWorld();
            return _instance;
        }
    }

    private List<EAAIObject> m_Obstacles = new List<EAAIObject>();
    private Dictionary<int, EAAIGroup> m_aiGroup = new Dictionary<int, EAAIGroup>();
    private List<Wall> m_walls = new List<Wall>();

    public List<Wall> Walls()
    {
        return m_walls;
    }

    public List<EAAIObject> Obstacles()
    {
        return m_Obstacles;
    }

    public void TagObstablesWithinViewRange(EAAIAgent entity,float radius)
    {
        IEnumerator<EAAIObject> it = m_Obstacles.GetEnumerator();

        while(it.MoveNext())
        {
            EAAIObject curEntity = it.Current;

            //first clear any current tag
            curEntity.UnTagging();

            Vector3 to = curEntity.GetPos() - entity.GetPos();

            //the bounding radius of the other is taken into account by adding it
            //to the range
            float range = radius + curEntity.GetBRadius();

            //if entity within range, tag for further consideration. (working in
            //distance-squared space to avoid sqrts)
            if(!object.ReferenceEquals(curEntity,entity) &&
                (to.sqrMagnitude < range * range))
            {
                curEntity.Tagging();
            }
        }
    }

    public void TagAIAgentWithinViewRange(EAAIAgent entity, float radius)
    {
        Dictionary<int, EAAIGroup>.Enumerator it = m_aiGroup.GetEnumerator();

        while(it.MoveNext())
        {
            List<EAAIAgent> entities = it.Current.Value.Agents();

            for (int i = 0; i < entities.Count; ++i)
            {
                EAAIAgent curEntity = entities[i];

                //first clear any current tag
                curEntity.UnTagging();

                Vector3 to = curEntity.GetPos() - entity.GetPos();

                //the bounding radius of the other is taken into account by adding it 
                //to the range
                float range = radius + curEntity.GetBRadius();

                //if entity within range, tag for further consideration. (working in
                //distance-squared space to avoid sqrts)
                if ((curEntity.GetHashCode() != entity.GetHashCode()) &&
                    (to.sqrMagnitude < range * range))
                {
                    curEntity.Tagging();
                }
            }
        }
    }

    public bool Overlapped(EAAIObject ob,List<EAAIObject> conOb,float minDistBetweenObstacles)
    {
        IEnumerator<EAAIObject> it = conOb.GetEnumerator();
        while(it.MoveNext())
        {
            EAAIObject tmp = it.Current;
            if (EAMathUtil.TwoCirclesOverlapped(ob.GetPos(),ob.GetBRadius() + minDistBetweenObstacles,tmp.GetPos(),tmp.GetBRadius())) return true;
        }
        return false;
    }

    public void AddAgent(string teamId,EAAIAgent agent)
    {
        int key = CRC32.GetHashForAnsi(teamId);
        if(!m_aiGroup.TryGetValue(key,out EAAIGroup group))
        {
            m_aiGroup.Add(key, new EAAIGroup(teamId));
        }
        RemoveAgent(agent);
        m_aiGroup[key].AddAgent(agent);
    }

    public void RemoveAgent(EAAIAgent agent)
    {
        agent.GetAIGroup().RemoveAgent(agent);
    }

    public void SetCrosshair(Vector3 p,string id="basic")
    {
        Vector3 proposedPosition = p;
        //make sure it's not inside an obstacle
        for(int i = 0; i < m_Obstacles.Count; ++i)
        {
            EAAIObject curOb = m_Obstacles[i];
            if (EAMathUtil.PointInCircle(curOb.GetPos(), curOb.GetBRadius(), proposedPosition)) return;
        }

        EAAIGroup aiGroup = GetAIGroup(id);
        if (aiGroup != null) aiGroup.SetCrosshair(p);
    }
    public EAAIGroup GetAIGroup(string id)
    {
        int key = CRC32.GetHashForAnsi(id);
        m_aiGroup.TryGetValue(key, out EAAIGroup aiGroup);
        return aiGroup;
    }
}
