using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EASteeringBehaviour : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 3.5f;
    [SerializeField] private float maxAcceleration = 10f;
    [SerializeField] private float turnSpeed = 20f;
    [SerializeField] private float targetRadius = 0.005f;
    [SerializeField] private float slowRadius = 1f;
    [SerializeField] private float timetoTarget = 0.1f;
    [SerializeField] private NearSensor sensor = null;
    [SerializeField] private bool canFly = false;
    [SerializeField] public  float slopeLimit = 80f;
    [SerializeField] public  float panicDist = 3.5f;
    [SerializeField] public  bool  decelerateOnStop = true;
     
    Dictionary<Steering.behaviour_type, Steering> steeringmap;

    public Rigidbody  rb      { get; private set; }
    public Transform  tr      { get; private set; }
    public EAAIAgent agent    { get; private set; }
    public bool isCanFly      { get; private set; }
    

    public NearSensor nearSensor 
    {
        get 
        {
          if (sensor == null)
          {
             var go = new GameObject("sensor", typeof(NearSensor));
             go.transform.SetParent(tr);
             sensor = go.GetComponent<NearSensor>();
          }
          return sensor; 
        } 
    }

    Steering.behaviour_type m_iflag = Steering.behaviour_type.none;

    public bool smoothing = true;
    public int numSamplesForSmoothing = 5;
    Queue<Vector3> velocitySamples = new Queue<Vector3>();

    public float GetMaxSpeed() => maxSpeed;
    public void SetMaxSpeed(float newSpeed) => maxSpeed = newSpeed;
    public float GetMaxAccel() => maxAcceleration;
    public void SetMaxAccel(float newAccel) => maxAcceleration = newAccel;

    public virtual void Initialize() 
    {
        tr     = transform;
        rb     = GetComponent<Rigidbody>();
        agent = GetComponent<EAAIAgent>();

        steeringmap = new Dictionary<Steering.behaviour_type, Steering>();

        foreach (Steering.behaviour_type t in System.Enum.GetValues(typeof(Steering.behaviour_type)))
        {
            if (t == Steering.behaviour_type.none) continue;
            if (steeringmap.TryGetValue(t, out Steering value)) continue;

            steeringmap.Add(t, Steering.Clone(t));
            steeringmap[t].Initialize(this);
        }

        m_iflag = Steering.behaviour_type.none;
    }

    public T Get<T>(Steering.behaviour_type t) where T : Steering
    {
        steeringmap.TryGetValue(t, out Steering value);
        return value as T;
    }
    
    public void Steer()
    {
        Vector3 accel = Vector3.zero;

        var it = steeringmap.GetEnumerator();

        while(it.MoveNext())
        {
            if (!IsOn(it.Current.Key)) continue;
            accel += it.Current.Value.GetWSteering();
        }

        if (accel.magnitude > maxAcceleration)
        {
            accel.Normalize();
            accel = accel * maxAcceleration;
        }

        Steer(accel);
    }

    public bool IsInFront(Vector3 target)
    {
        return IsFacing(target, 0);
    }

    public bool IsFacing(Vector3 target,float cosineValue)
    {
        Vector3 facing = tr.forward.normalized;

        Vector3 directionToTarget = (target - tr.position);
        directionToTarget.Normalize();
        return Vector3.Dot(facing, directionToTarget) >= cosineValue;
    }

    public void Steer(Vector3 linearAcceleration)
    {
        rb.velocity += linearAcceleration * Time.deltaTime;

        if (rb.velocity.magnitude > maxSpeed) rb.velocity = rb.velocity.normalized * maxSpeed;
    }

    public Vector3 Seek(Vector3 targetPosition,float maxSeekAccel)
    {
        Debug.DrawLine(transform.position, targetPosition, Color.magenta , 0f, false);

        Vector3 acceleration = (targetPosition - tr.position);
        acceleration.Normalize();

        acceleration = acceleration * maxSeekAccel;
        return acceleration;
    }

    public Vector3 Seek(Vector3 targetPosition)
    {
        return Seek(targetPosition, maxAcceleration);
    }

    public void LookWhereYourGoing() 
    {
        Vector3 direction = rb.velocity;

        if(smoothing)
        {
            if(velocitySamples.Count == numSamplesForSmoothing)
            {
                velocitySamples.Dequeue();
            }

            velocitySamples.Enqueue(rb.velocity);

            direction = Vector3.zero;

            foreach(Vector3 v in velocitySamples)
            {
                direction += v;
            }

            direction /= velocitySamples.Count;
        }

        LookAtDirection(direction);
    }

    public void LookAtDirection(Vector3 direction)
    {
        direction.Normalize();

        if (direction.sqrMagnitude > 0.001f)
        {
            float toRotation = -1 * (Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg - 90.0f);
            float rotation = Mathf.LerpAngle(rb.rotation.eulerAngles.y, toRotation, Time.deltaTime * turnSpeed);
            rb.rotation = Quaternion.Euler(0, rotation, 0);
        }
    }

    public Vector3 Arrive(Vector3 targetPosition)
    {
        Debug.DrawLine(transform.position, targetPosition, Color.cyan, 0f, false);

        Vector3 targetVelocity = targetPosition - rb.position;

        float dist = targetVelocity.magnitude;

        if (dist < targetRadius) return Vector3.zero;

        float targetSpeed = maxSpeed;
        if (dist <= slowRadius) targetSpeed = maxSpeed * (dist / slowRadius);

        targetVelocity.Normalize();
        targetVelocity *= targetSpeed;

        Vector3 acceleration = targetVelocity - rb.velocity;
        acceleration = acceleration * 1f / timetoTarget;

        if (acceleration.magnitude > maxAcceleration)
        {
            acceleration.Normalize();
            acceleration = acceleration * maxAcceleration;
        }

        return acceleration;
    }

    public Vector3 Interpose(EAAIAgent target1, EAAIAgent target2)
    {
        Vector3 midPoint = (target1.GetPos() + target2.GetPos()) * 0.5f;
        float timeToReachMidPoint = Vector3.Distance(midPoint, transform.position) / maxSpeed;

        Vector3 futureTarget1Pos = target1.GetPos() + target1.GetVelocity() * timeToReachMidPoint;
        Vector3 futureTarget2Pos = target2.GetPos() + target2.GetVelocity() * timeToReachMidPoint;

        midPoint = (futureTarget1Pos + futureTarget2Pos) * 0.5f;
        return Arrive(midPoint);
    }

    public Vector3 Flee(Vector3 targetPosition)
    {
        Vector3 acceleration = tr.position - targetPosition;

        if(acceleration.magnitude > panicDist)
        {
            if(decelerateOnStop && rb.velocity.magnitude > 0.001f)
            {
                acceleration = -rb.velocity / timetoTarget;

                if(acceleration.magnitude > maxAcceleration)
                {
                    acceleration.Normalize();
                    acceleration *= maxAcceleration;
                }
                return acceleration;
            }
            else
            {
                rb.velocity = Vector3.zero;
                return Vector3.zero;
            }
        }

        acceleration.Normalize();
        acceleration *= maxAcceleration;
        return acceleration;
    }

    #region AICommand

    public bool IsOn(Steering.behaviour_type bType) => IsOn(bType, m_iflag);
    private bool IsOn(Steering.behaviour_type bType, Steering.behaviour_type iflag) => (iflag & bType) == bType;
    Steering.behaviour_type Off(Steering.behaviour_type bType,Steering.behaviour_type iflag)
    {
        if (IsOn(bType, iflag)) iflag ^= bType;
        return iflag;
    }
    void Off(Steering.behaviour_type bType) => m_iflag = Off(bType, m_iflag);
    void On(Steering.behaviour_type bType) => m_iflag |= bType;
    public void DefaultOn()
    {
        On(Steering.behaviour_type.agent_avoidance);
        On(Steering.behaviour_type.obstacle_avoidance);
    }
    public void CohesionOn() => On(Steering.behaviour_type.cohesion);
    public void FleeOn() => On(Steering.behaviour_type.flee);
    public void ArriveOn() => On(Steering.behaviour_type.arrive);
    public void Wander1On() => On(Steering.behaviour_type.wander1);
    public void Wander2On() => On(Steering.behaviour_type.wander2);
    public void SeperationOn() => On(Steering.behaviour_type.seperation);
    public void AlignmentOn() => On(Steering.behaviour_type.alignment);
    public void PursuitOn() => On(Steering.behaviour_type.pursuit);
    public void PursuitOffsetOn() => On(Steering.behaviour_type.pursuitOffset);
    public void EvadeOn() => On(Steering.behaviour_type.evade);
    public void FollowOn() => On(Steering.behaviour_type.follow_path);
    public void Follow2On() => On(Steering.behaviour_type.follow_path_v2);
    public void AgentAvoidOn() => On(Steering.behaviour_type.agent_avoidance);
    public void HideOn() => On(Steering.behaviour_type.hide);

    public void CohesionOff() => Off(Steering.behaviour_type.cohesion);
    public void FleeOff() => Off(Steering.behaviour_type.flee);
    public void ArriveOff() => Off(Steering.behaviour_type.arrive);
    public void Wander1Off() => Off(Steering.behaviour_type.wander1);
    public void Wander2Off() => Off(Steering.behaviour_type.wander2);
    public void SeperationOff() => Off(Steering.behaviour_type.seperation);
    public void AlignmentOff() => Off(Steering.behaviour_type.alignment);
    public void PursuitOff() => Off(Steering.behaviour_type.pursuit);
    public void PursuitOffsetOff() => Off(Steering.behaviour_type.pursuitOffset);
    public void EvadeOff() => Off(Steering.behaviour_type.evade);
    public void FollowOff() => Off(Steering.behaviour_type.follow_path);
    public void Follow2Off() => Off(Steering.behaviour_type.follow_path_v2);
    public void AgentAvoidOff() => Off(Steering.behaviour_type.agent_avoidance);
    public void HideOff() => Off(Steering.behaviour_type.hide);

    #endregion

    public bool IsSteering() 
    {
        Steering.behaviour_type clonFlag = m_iflag;
        clonFlag = Off(Steering.behaviour_type.agent_avoidance, clonFlag);
        clonFlag = Off(Steering.behaviour_type.obstacle_avoidance, clonFlag);
        return ((int)clonFlag > 1) ? true : false;
    }

    public void ResetInfo()
    {
        DefaultOn();

        var it = steeringmap.GetEnumerator();
        while (it.MoveNext())
        {
            it.Current.Value.Reset();
        }
    }

    public void ZeroFlag() => m_iflag = 0;


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        var it = steeringmap.GetEnumerator();

        while (it.MoveNext())
        {
            if (!IsOn(it.Current.Key)) continue;
            it.Current.Value.DrawGizmo();
        }
    }
#endif
}
