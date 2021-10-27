using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EASteeringBehaviour
{
    float wayPointSeekDist = 1.0f;

    EAAIAgent m_entity = null;

    [Flags]
    private enum behaviour_type
    {
       none   = 1 << 0,
       seek   = 1 << 1,
       flee   = 1 << 2,
       arrive = 1 << 3,
       wander = 1 << 4,
       cohesion = 1 << 5,
       separation = 1 << 6,
       alignment  =  1 << 7,
       obstacle_avoidance = 1 << 8,
       wall_avoidance = 1 << 9,
       follow_path = 1 << 10,
       pursuit = 1 << 11,
       evade = 1 << 12,
       interpose = 1 << 13,
       hide = 1 << 14,
       flock = 1 << 15,
       offset_pursuit = 1 << 16,
    }

    // these can be used to keep track of friends , pursuers , or prey
    private EAAIAgent m_pTargetAgent1 = null;
    private EAAIAgent m_pTargetAgent2 = null;

    float m_fDBoxLength;
    
    // a vertex buffer to contain the feelers rqd for wall avoidance
    private List<Vector3> m_feelers = new List<Vector3>();

    float m_fWallDetectionFeelerLength = 0;

    Vector3 m_vWanderTarget;

    float m_fWanderJitter = 50.0f;
    float m_fWanderRadius = 10.0f;
    float m_fWanderDistance = 25.0f;

    
    //how far the agent can 'see'
    private float m_fViewDistance;

    //pointer to any current path
    private EAAIPath m_pPath = new EAAIPath();

    private float m_fWaypointSeekDistSq = 1.0f;

    // any offset used for formations or offset 
    private Vector3 m_vOffset;

    behaviour_type m_iflag = behaviour_type.none;

    // Gravity constant
    private float m_fGravityConstant = 4.9f;

    // Landing height
    private float m_fheight = 0;

    private enum Deceleration
    {
        fast = 1,
        normal = 2,
        slow = 3,
    }

    Deceleration m_deceleration = Deceleration.normal;

    float minDetectionBoxLength = 20.0f;

    public EASteeringBehaviour(EAAIAgent entity)
    {
        m_entity = entity;
    }
    public void SetWanderDistance(float fWanderDistance)
    {
        this.m_fWanderDistance = fWanderDistance;
    }
    public void SetWaypointSeekDist(float wayPointSeekDist)
    {
        this.wayPointSeekDist = wayPointSeekDist;
        m_fWaypointSeekDistSq = wayPointSeekDist * wayPointSeekDist;
    }
    public void SetLandHeight(float height)
    {
        m_fheight = height;
    }
    public void SetPath(List<Vector3> paths,bool isLoop = false)
    {
        m_pPath.Set(paths);
        if (isLoop == true) m_pPath.LoopOn();
        if (isLoop == false) m_pPath.LoopOff();
    }
    bool IsOn(behaviour_type bType) { return IsOn(bType,m_iflag); }
    bool IsOn(behaviour_type bType, behaviour_type iflag) { return ((iflag & bType) == bType); }
    behaviour_type Off(behaviour_type bType, behaviour_type iflag)
    {
        if (IsOn(bType, iflag)) iflag ^= bType;
        return iflag;
    }
    void Off(behaviour_type bType) => Off(bType, m_iflag); 
    void On(behaviour_type bType) =>  m_iflag |= bType; 
    public void DefaultOn()
    {
        m_iflag = 0;
        On(behaviour_type.obstacle_avoidance);
        On(behaviour_type.wall_avoidance);
    }
    public void CohesionOn() => On(behaviour_type.cohesion);
    public void FleeOn() => On(behaviour_type.flee);
    public void ArriveOn() => On(behaviour_type.arrive);
    public void WanderOn() => On(behaviour_type.wander);
    public void AlignmentOn() => On(behaviour_type.alignment);
    public void FollowPathOn() => On(behaviour_type.follow_path);
   
    public void CohesionOff() => Off(behaviour_type.cohesion);
    public void FleeOff() => Off(behaviour_type.flee);
    public void ArriveOff() => Off(behaviour_type.arrive);
    public void WanderOff() => Off(behaviour_type.wander);
    public void AlignmentOff() => Off(behaviour_type.alignment);
    public void FollowPathOff() => Off(behaviour_type.follow_path);
    public bool IsSteering()
    {
        behaviour_type clonFlag = m_iflag;
        clonFlag = Off(behaviour_type.obstacle_avoidance,clonFlag);
        clonFlag = Off(behaviour_type.wall_avoidance, clonFlag);
        return ((int)clonFlag > 1) ? true : false;
    }
    // Creates the antenna utilized by WallAvoidance
    private void CreateFleers()
    {
        m_feelers.Clear();
        //feeler pointing straight in front
        m_feelers.Add(m_entity.GetPos() + m_entity.GetHeading() * m_fWallDetectionFeelerLength);

        //feeler to left
        Vector3 temp = m_entity.GetHeading();

        Quaternion rotate = Quaternion.Euler(0, 90.0f, 0);

        Vector3 temp2 = rotate * temp;

        m_feelers.Add(m_entity.GetPos() + temp2 * m_fWallDetectionFeelerLength / 2.0f);

        rotate = Quaternion.Euler(0, 45.0f, 0);

        temp2 = rotate * temp;

        //feeler to right
        m_feelers.Add(m_entity.GetPos() + temp2 * m_fWallDetectionFeelerLength / 2.0f);
    }
    /**
   * Given a target, this behavior returns a steering force which will
   *  direct the agent towards the target
   */
    private Vector3 Seek(Vector3 targetPos)
    {
        Vector3 desiredVelocity = (targetPos - m_entity.GetPos()).normalized;
        return desiredVelocity - m_entity.GetVelocity();
    }

    /**
   *   Move along the specified path                                            
   */
    private Vector3 FindRoute(Vector3 targetPos)
    {
        Vector3 toTarget = targetPos - m_entity.GetPos();

        //calculate the distance to the target
        float dist = toTarget.magnitude;

        //because Deceleration is enumerated as an int, this value is required
        //to provide fine tweaking of the deceleration..
        float decelerationTweaker = 0.3f;

        //calculate the speed required to reach the target given the desired
        //deceleration
        float speed = dist / (0.5f * decelerationTweaker);

        //make sure the velocity does not exceed the max
        speed = Mathf.Min(speed, m_entity.GetMaxSpeed());

        //from here proceed just like Seek except we don't need to normalize 
        //the ToTarget vector because we have already gone to the trouble
        //of calculating its length: dist. 
        // speed  = distance / time    
        // arrival time = (arrival speed/arrival distance)

        Vector3 desiredVelocity = toTarget * (speed / dist);

        return desiredVelocity - m_entity.GetVelocity();
    }
    private Vector3 Flee(Vector3 targetPos)
    {
        Vector3 desiredVelocity = (m_entity.GetPos() - targetPos).normalized * m_entity.GetMaxSpeed();
        return desiredVelocity - m_entity.GetVelocity();
    }
    /**
     * This behavior is similar to seek but it attempts to arrive at the
     *  target with a zero velocity
     */
    private Vector3 Arrive(Vector3 targetPos, Deceleration deceleration)
    {
        Vector3 toTarget = targetPos - m_entity.GetPos();

        //calculate the distance to the target
        float dist = toTarget.magnitude;
        float epsilon = 0.01f;

        if (dist > epsilon)
        {
            //because Deceleration is enumerated as an int, this value is required
            //to provide fine tweaking of the deceleration..
            float decelerationTweeker = 0.3f;

            float speed = dist / decelerationTweeker;
            speed = Mathf.Min(speed, m_entity.GetMaxSpeed());

            Vector3 desiredVelocity = toTarget * (speed / dist);
            desiredVelocity.y = m_entity.GetVelocity().y;
            return desiredVelocity;
        }
        return Vector3.zero;
    }

    /**
   *  this behavior creates a force that steers the agent towards the 
   *  evader
   */
    private Vector3 Pursuit(EAAIAgent evader)
    {
        if(evader == null)
        {
            Debug.Assert(false, "Pursuit evader is not assigned");
            return Vector3.zero;
        }

        //if the evader is ahead and facing the agent then we can just seek
        //for the evader's current position.
        Vector3 toEvader = evader.GetPos() - m_entity.GetPos();

        float relativeHeading = Vector3.Dot(m_entity.GetHeading(), evader.GetHeading());

        if(Vector3.Dot(toEvader,m_entity.GetHeading()) > 0 && (relativeHeading < 0.95f)) //acos(0.95)=18 degs
        {
            return Seek(evader.GetPos());
        }

        //Not considered ahead so we predict where the evader will be.

        //the lookahead time is propotional to the distance between the evader
        //and the pursuer; and is inversely proportional to the sum of the
        //agent's velocities
        float lookAheadTime = toEvader.magnitude / (m_entity.GetMaxSpeed() + m_entity.GetSpeed());

        //now seek to the predicted future position of the evader
        return Seek(evader.GetPos() + evader.GetVelocity() * lookAheadTime);
    }
    /**
  *  similar to pursuit except the agent Flees from the estimated future
  *  position of the pursuer
  */
    private Vector3 Evade(EAAIAgent pursuer)
    {
        if(pursuer == null)
        {
            Debug.Assert(false, "evade pursuer not assigned");
            return Vector3.zero;
        }   

        // Not necessary to include the check for facing direction this time
        Vector3 toPursuer = pursuer.GetPos() - m_entity.GetPos();

        //uncomment the following two lines to have Evade only consider pursuers 
        //within a 'threat range'
        float threatRange = 100.0f;
        if (toPursuer.sqrMagnitude > threatRange * threatRange) return Vector3.zero;

        //the lookahead time is propotional to the distance between the pursuer
        //and the pursuer; and is inversely proportional to the sum of the
        //agents' velocities
        float lookAheadTime = toPursuer.magnitude / (m_entity.GetMaxSpeed() + pursuer.GetSpeed());

        //now flee away from predicted future position of the pursuer
        return Flee(pursuer.GetPos() + pursuer.GetVelocity() * lookAheadTime);
    }

    /**
   * This behavior makes the agent wander about randomly
   */
    private Vector3 Wander() 
    {
        //this behavior is dependent on the update rate, so this line must
        //be included when using time independent framerate.
        float jitterThisTimeSlice = m_fWanderJitter * m_entity.GetTimeElapsed();

        //first, add a small random vector to the target's position
        m_vWanderTarget = m_vWanderTarget + new Vector3(EAMathUtil.RandomClamped() * jitterThisTimeSlice, 0.0f,
          EAMathUtil.RandomClamped() * jitterThisTimeSlice);

        //reproject this new vector back on to a unit circle
        m_vWanderTarget.Normalize();

        //increase the length of the vector to the same as the radius
        //of the wander circle
        m_vWanderTarget = m_vWanderTarget * m_fWanderRadius;

        //move the target into a position WanderDist in front of the agent
        Vector3 target = m_vWanderTarget + m_fWanderDistance * Vector3.forward;

        //project the target into world space
        target = EAMathUtil.PointToWorldSpace(target, m_entity.GetHeading(), m_entity.GetSide(), m_entity.GetPos());

        // and steer towards it
        return target - m_entity.GetPos();
    }
    private bool CheckNeighbors(EAAIAgent neighbor)
    {
        return (neighbor != m_entity) && neighbor.Tag && (neighbor != m_pTargetAgent1);
    }
    /**
   * this calculates a force repelling from the other neighbors
   */
    Vector3 Separation(List<EAAIAgent> neighbors)
    {
        Vector3 steeringVelocity = Vector3.zero;

        for(int a = 0; a < neighbors.Count; ++a)
        {
            //make sure this agent isn't included in the calculations and that 
            //the agent beging examined is close enough. ***also make sure it doesn't
            //include the evade target ***
            if(CheckNeighbors(neighbors[a]))
            {
                Vector3 toAgent = m_entity.GetPos() - neighbors[a].GetPos();

                //scale the force inversely proportional to the agents distance
                //from its neighbor
                steeringVelocity += (toAgent.normalized / toAgent.magnitude);
            }
        }
        return steeringVelocity;
    }

    /**
     * returns a force that attempts to align this agents heading with that
     * of its neighbors
     */
    private Vector3 Alignment(List<EAAIAgent> neightbors)
    {
        //used to record the average heading of the neighbors
        Vector3 averageHeading = Vector3.zero;

        //used to count the number of vehicles in the neighborhood
        int neighborCount = 0;

        //iterate through all the tagged vehicles and sum their heading vectors 
        for(int a = 0; a < neightbors.Count; ++a)
        {
            // make sure this agent isn't included in the calculations and that
            //the agent being examined  is close enough also make sure it doesn't
            //include any evade target
            if(CheckNeighbors(neightbors[a]))
            {
                averageHeading += neightbors[a].GetHeading();
                ++neighborCount;
            }
        }

        //if the neighborhood contained one or more vehicles, average their
        //heading vectors.
        if(neighborCount > 0)
        {
            averageHeading = averageHeading / neighborCount;
        }
        return averageHeading;
    }

    /**
    * This returns a steering force that will keep the agent away from any
    *  walls it may encounter
    */
    private Vector3 WallAvoidance(List<Wall> walls)
    {
        //the feelers are contained in a std::vector, m_Feelers
        CreateFleers();

        float distToThisIP = 0f;
        float distToClosetIP = float.MaxValue;

        //this will hold an index into the vector of walls
        int closetWall = -1;

        Vector3 steeringForce = Vector3.zero,
                point = Vector3.zero, //used for storing temporary info
                closetPoint = Vector3.zero; //holds the closest intersection point

        //examine each feeler in turn
        for(int flr = 0; flr < m_feelers.Count; ++flr)
        {
            //run through each wall checking for any interesction points
            for(int w = 0; w< walls.Count; ++w)
            {
                bool check = EAMathUtil.LineIntersectionXZ(m_entity.GetPos(),
                      m_feelers[flr],
                      walls[w].From(),
                      walls[w].To(), out distToThisIP);

                if(check)
                {
                    //is this the closest found so far? If so keep a record
                    if(distToThisIP < distToClosetIP)
                    {
                        distToClosetIP = distToThisIP;
                        closetWall = w;
                        closetPoint = point;
                    }
                }
            } //next wall

            //if an intersection point has been detected, calculate a force  
            //that will direct the agent away
            if(closetWall >= 0)
            {
                //calculate by what distance the projected position of the agent
                //will overshoot the wall
                Vector3 overShoot = m_feelers[flr] - closetPoint;

                //create a force in the direction of the wall normal, with a 
                //magnitude of the overshoot
                steeringForce = walls[closetWall].Normal() * overShoot.magnitude;
            }
        } //next feeler

        return steeringForce;
    }

    /**
    * returns a steering force that attempts to move the agent towards the
    * center of mass of the agents in its immediate area
    */
    private Vector3 Cohesion(List<EAAIAgent> neighbors)
    {
        //first find the center of mass of all the agents
        Vector3 centerOfMass = Vector3.zero;
        Vector3 steeringForce = Vector3.zero;

        int neighborCount = 0;

        //iterate through the neighbors and sum up all the position vectors
        for(int a = 0; a < neighbors.Count; ++a)
        {
            //make sure this agent isn't included in the calculations and that
            //the agent being examined is close enough also make sure it doesn't
            //include the evade target
            if(CheckNeighbors(neighbors[a]))
            {
                centerOfMass += neighbors[a].GetPos();
                ++neighborCount;
            }
        }
        if(neighborCount > 0)
        {
            //the center of mass is the average of the sum of positions
            centerOfMass = centerOfMass / (float)neighborCount;
            //now seek towards that position
            steeringForce = Seek(centerOfMass);
        }
        //the magnitude of cohesion is usually much larger than separation of
        //alignment so it usually helps to normalize it.
        return steeringForce.normalized;
    }

    /**
    *  Given a vector of obstacles, this method returns a steering force
    *  that will prevent the agent colliding with the closest obstacle
    */
    private Vector3 ObstacleAvoidance(List<EAAIObject> obstacles)
    {
        //the detection box length is proportional to the agent's velocity
        m_fDBoxLength = minDetectionBoxLength + 
                        (m_entity.GetSpeed() / m_entity.GetMaxSpeed()) * minDetectionBoxLength;

        //tag all obstacles within range of the box for processing
        m_entity.World().TagObstablesWithinViewRange(m_entity, m_fDBoxLength);

        //this will keep track of the closest intersecting obstacle (CIB)
        EAAIObject closetIntersectingObstacle = null;

        //this will be used to track the distance to the CIB
        float distToClosetIP = float.MaxValue;

        //this will record the transformed local coordinates of the CIB
        Vector3 localPosOfClosetObstacle = Vector3.zero;

        IEnumerator<EAAIObject> it = obstacles.GetEnumerator();

        while(it.MoveNext())
        {
            //if the obstacle has been tagged within range proceed
            EAAIObject curOb = it.Current;

            if(curOb.Tag)
            {
                //calculate this obstacle's position in local space
                Vector3 localPos = EAMathUtil.PointToWorldSpace(curOb.GetPos(),
                    m_entity.GetHeading(),
                    m_entity.GetSide(),
                    m_entity.GetPos());

                // entity Heading and LocalPos is 
                if(Vector3.Dot(localPos,Vector3.forward) > 0)
                {
                    //than its radius + half the width of the detection box then there
                    //is a potential intersection
                    float expandedRadius = curOb.GetBRadius() + m_entity.GetBRadius();

                    if(Mathf.Abs(localPos.x) < expandedRadius)
                    {
                        //test to see if this is the closet so far. If it is keep a
                        //record of the obstacle and its local coordinates
                        if(localPos.magnitude < distToClosetIP)
                        {
                            distToClosetIP = localPos.magnitude;
                            closetIntersectingObstacle = curOb;
                            localPosOfClosetObstacle = localPos;
                        }
                    }
                }
            }
        }

        //if we have found an intersecting obstacle, calculate a steering
        //force away from it
        Vector3 steeringForce = Vector3.zero;

        if(closetIntersectingObstacle != null)
        {
            //should be
            //the closer the agent is to an object, the stronger the 
            //steering force should be
            float multiplier = 1.0f + (m_fDBoxLength - localPosOfClosetObstacle.z) / m_fDBoxLength;

            //calculate the lateral force
            steeringForce.x = (closetIntersectingObstacle.GetBRadius() - localPosOfClosetObstacle.x) * multiplier;

            //apply a braking force proportional to the obstacles distance from
            //the vehicle
            float brakingWeight = 0.2f;

            steeringForce.z = (closetIntersectingObstacle.GetBRadius() - localPosOfClosetObstacle.z) * brakingWeight;
        }

        //finally, convert the steering vector from local to world space
        return EAMathUtil.VectorToWorldSpace(steeringForce, m_entity.GetHeading(), m_entity.GetSide());
    }

    /**
    * Given two agents, this method returns a force that attempts to 
    * position the vehicle between them
    */
    private Vector3 Interpose(EAAIAgent agentA,EAAIAgent agentB)
    {
        if(agentA == null || agentB == null)
        {
            Debug.Assert(false, "Interpose agents not assigned");
            return Vector3.zero;
        }

        //first we need to figure out where the two agents are going to be at
        //time T in the future. This is approximated by determining the time
        //taken to reach the mid way point at the current time at max speed.
        Vector3 midPoint = (agentA.GetPos() + agentB.GetPos()) / 2.0f;

        float timeToReachMidPoint = (midPoint - m_entity.GetPos()).sqrMagnitude / m_entity.GetMaxSpeed();

        //now we have T, we assume that agent A and agent B will continue on a
        //straight trajectory and extrapolate to get their future positions
        Vector3 aPos = agentA.GetPos() + (agentA.GetVelocity() * timeToReachMidPoint);
        Vector3 bPos = agentB.GetPos() + (agentB.GetVelocity() * timeToReachMidPoint);

        //calculate the mid point of these predicted positions
        midPoint = (aPos + bPos) / 2.0f;

        //then steer to arrive at it
        return Arrive(midPoint, Deceleration.fast);
    }

    private Vector3 Hide(EAAIAgent hunter,List<EAAIObject> obstacles)
    {
        float distToCloset = float.MaxValue;
        Vector3 bestHidingSpt = Vector3.zero;
        EAAIObject closet;

        for(int i = 0; i < obstacles.Count; ++i)
        {
            EAAIObject curOb = obstacles[i];

            Vector3 hidingSpot = GetHidingPosition(curOb.GetPos(), curOb.GetBRadius(), hunter.GetPos());

            float dist = (m_entity.GetPos() - hidingSpot).sqrMagnitude;
            distToCloset = dist;
            bestHidingSpt = hidingSpot;
            if (dist < distToCloset) closet = curOb;
        }

        //if no suitable obstacles found then evade the hunter
        if (distToCloset == float.MaxValue) return Evade(hunter);

        //else use Arrive on the hiding spot
        return Arrive(bestHidingSpt, Deceleration.fast);
    }

    /**
   *  Given the position of a hunter, and the position and radius of
   *  an obstacle, this method calculates a position DistanceFromBoundary 
   *  away from its bounding radius and directly opposite the hunter
   */
    private Vector3 GetHidingPosition(Vector3 posOb,float radiusOb,Vector3 posHunter)
    {
        //calculate how far away the agent is to be from the chosen obstacle's
        //bounding radius
        float distanceFromBoundary = 30.0f;
        float distAway = radiusOb + distanceFromBoundary;

        //calculate the heading toward the object from the hunter
        Vector3 toOb = (posOb - posHunter).normalized;

        //scale it to size and add to the obstacles position to get
        //the hiding spot.
        return (toOb * distAway) + posOb;
    }
    private Vector3 FollowPath() 
    {
        //move to next target if close enough to current target (working in
        //distance squared space)
        Vector3 wayPoint = m_pPath.CurrentWayPoint();
        
        if((wayPoint - m_entity.GetPos()).sqrMagnitude < m_fWaypointSeekDistSq)
        {
            m_pPath.SetNextWayPoint();
        }

        if (!m_pPath.Finished()) return FindRoute(m_pPath.CurrentWayPoint());
        
        return Arrive(m_pPath.CurrentWayPoint(), Deceleration.normal);
    }

    /**
     * calculates the accumulated steering force according to the method set
     *  in m_SummingMethod
     */
    public Vector3 Calculate() 
    {
        // reset the steering force
        Vector3 steeringForce = Vector3.zero;

        //tag neighbors if any of the following 3 group behaviors are switched on
        if(IsOn(behaviour_type.separation) || IsOn(behaviour_type.alignment) || IsOn(behaviour_type.cohesion))
        {
            m_entity.World().TagObstablesWithinViewRange(m_entity, m_fViewDistance);
        }

        if (IsOn(behaviour_type.wall_avoidance)) steeringForce += WallAvoidance(m_entity.World().Walls());
        if (IsOn(behaviour_type.obstacle_avoidance)) steeringForce += ObstacleAvoidance(m_entity.World().Obstacles());
        if (IsOn(behaviour_type.evade)) steeringForce += Evade(m_pTargetAgent1);

        //these next tree can be combined for flocking behavior (wander is
        //also a good behavior to add into this mix
        if (IsOn(behaviour_type.separation)) steeringForce += Separation(m_entity.GetAIGroup().Agents());
        if (IsOn(behaviour_type.alignment)) steeringForce += Alignment(m_entity.GetAIGroup().Agents());
        if (IsOn(behaviour_type.cohesion)) steeringForce += Cohesion(m_entity.GetAIGroup().Agents());
        if (IsOn(behaviour_type.wander)) steeringForce += Wander();
        if (IsOn(behaviour_type.seek)) steeringForce += Seek(m_entity.VTarget());
        if (IsOn(behaviour_type.flee)) steeringForce += Flee(m_entity.VTarget());
        if (IsOn(behaviour_type.arrive)) steeringForce += Arrive(m_entity.VTarget(), m_deceleration);
        if (IsOn(behaviour_type.pursuit)) steeringForce += Pursuit(m_pTargetAgent1);
        if (IsOn(behaviour_type.interpose)) steeringForce += Interpose(m_pTargetAgent1, m_pTargetAgent2);
        if (IsOn(behaviour_type.hide)) steeringForce += Hide(m_pTargetAgent1, m_entity.World().Obstacles());
        if (IsOn(behaviour_type.follow_path)) steeringForce += FollowPath();

        steeringForce = EAMathUtil.Truncate(steeringForce, m_entity.GetMaxSpeed());
        return steeringForce;
    }

    public void OnDebugRender() 
    {
        if(IsOn(behaviour_type.wander))
        {
            Vector3 m_vTCC = EAMathUtil.PointToWorldSpace(m_fWanderDistance * Vector3.forward,
                m_entity.GetHeading(),
                m_entity.GetSide(),
                m_entity.GetPos());

            //draw the wander circle
            DebugExtension.DrawCircle(m_vTCC, Color.green, m_fWanderRadius);

            Vector3 target = (m_vWanderTarget + m_fWanderDistance * Vector3.forward);

            target = EAMathUtil.PointToWorldSpace(target, m_entity.GetHeading(), m_entity.GetSide(), m_entity.GetPos());
            DebugExtension.DrawLineArrow(m_entity.GetPos(), target);
            DebugExtension.DrawCircle(target, Color.red, 3);
        }

        if(IsOn(behaviour_type.arrive) || IsOn(behaviour_type.flee))
        {
            DebugExtension.DrawLineArrow(m_entity.GetPos(), m_entity.VTarget());
            DebugExtension.DrawCircle(m_entity.VTarget(), Color.red, 3);
        }

        if (IsOn(behaviour_type.follow_path)) m_pPath.OnDebugRender();

        if(IsOn(behaviour_type.obstacle_avoidance))
        {
            float length = minDetectionBoxLength + (m_entity.GetSpeed() / m_entity.GetMaxSpeed()) * minDetectionBoxLength;

            Matrix4x4 matTransform = Matrix4x4.TRS(m_entity.GetPos(), Quaternion.LookRotation(m_entity.GetHeading(),Vector3.up),Vector3.one);

            Vector3 size = Vector3.zero;
            size.x = m_entity.GetBRadius();
            size.y = 0f;
            size.z = length;

            Vector3 center = Vector3.zero;
            center.x = 0f;
            center.y = 0f;
            center.z = length * 0.5f;

            DebugExtension.DrawLocalCube(matTransform, size, Color.black, center);
        }
    }
}
