using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance : Steering
{
    public float maxAcceleration = 40f;
    public enum OBDetection { Raycast , Spherecast }
    public OBDetection wallDetection = OBDetection.Spherecast;
    public LayerMask castMask = Physics.DefaultRaycastLayers;

    public float wallAvoidDistance = 2.5f * 2f;
    public float mainWhiskerLen = 1.25f * 2f;
    public float sideWhiskerLen = 0.701f * 2f;
    public float sideWhiskerAngle =  45f;

    struct GenericCastHit
    {
        public Vector3 point;
        public Vector3 normal;
        public GenericCastHit(RaycastHit h)
        {
            point = h.point;
            normal = h.normal;
        }
    }

    public override void Initialize(EASteeringBehaviour steering)
    {
        base.Initialize(steering);
        castMask = LayerMask.GetMask("WALL","OBSTACLE");
    }

    public override Vector3 GetSteering()
    {
        if(steering.rb.velocity.magnitude > 0.005f)
        {
            return GetAvoidance(steering.rb.velocity);
        }
        else
        {
            return GetAvoidance(steering.agent.GetOrientationToVector());
        }  
    }

    public Vector3 GetAvoidance(Vector3 facingDir)
    {
        if(!FindObstacle(facingDir,out GenericCastHit hit)) return Vector3.zero;

        Vector3 targetPosition = hit.point + hit.normal * wallAvoidDistance;

        float angle = Vector3.Angle(steering.agent.GetVelocity(), hit.normal);

        if(angle > 165f)
        {
            Vector3 perp = new Vector3(-hit.normal.z, hit.normal.y, hit.normal.x);
            targetPosition = targetPosition + (perp * Mathf.Sin((angle - 165f) * Mathf.Deg2Rad) * 2f * wallAvoidDistance);
        }

        if (!steering.isCanFly) targetPosition.y = steering.tr.position.y;

        return steering.Seek(targetPosition, maxAcceleration);
    }

    bool FindObstacle(Vector3 facingDir,out GenericCastHit firstHit)
    {
        facingDir.y = 0f;
        facingDir.Normalize();

        Vector3[] dirs = new Vector3[3];
        dirs[0] = facingDir;

        float orientation = EAMathUtil.VectorToOrientation(facingDir);
        dirs[1] = EAMathUtil.OrientationToVector(orientation + sideWhiskerAngle * Mathf.Deg2Rad);
        dirs[2] = EAMathUtil.OrientationToVector(orientation - sideWhiskerAngle * Mathf.Deg2Rad);

        return CastWhiskers(dirs, out firstHit);
    }
    bool CastWhiskers(Vector3[] dirs,out GenericCastHit firstHit)
    {
        firstHit = new GenericCastHit();
        bool foundObs = false;
        for(int i = 0; i < dirs.Length;++i)
        {
            float dist = (i == 0) ? mainWhiskerLen : sideWhiskerLen;
            if(GenericCast(dirs[i],out GenericCastHit hit,dist))
            {
                foundObs = true;
                firstHit = hit;
                break;
            }
        }
        return foundObs;
    }
    bool GenericCast(Vector3 direction,out GenericCastHit hit,float distance = Mathf.Infinity)
    {
        bool result = false;
        RaycastHit h = default(RaycastHit);
        Vector3 origin = steering.agent.GetColliderPos();
        if(wallDetection == OBDetection.Raycast)
        {
            result = Physics.Raycast(origin, direction, out h, distance, castMask.value);
        }
        if(wallDetection == OBDetection.Spherecast)
        {
            result = Physics.SphereCast(origin, (steering.agent.GetBRadius() * 0.5f), direction, out h, distance, castMask.value);
        }

        hit = new GenericCastHit(h);
        if(!steering.isCanFly && result)
        {
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            if(angle < steering.slopeLimit)
            {
                hit.normal.y = 0f;
                hit.normal = hit.normal * 2f;
                hit.normal.Normalize();
                result = false;
            }
        }
        return result;
    }

    public override void DrawGizmo()
    {
        if (steering.rb.velocity.magnitude > 0.005f)
        {
            DrawObstacle(steering.rb.velocity);
        }
        else
        {
            DrawObstacle(steering.agent.GetOrientationToVector());
        }
    }

    void DrawObstacle(Vector3 facingDir)
    {
        facingDir.y = 0f;
        facingDir.Normalize();

        Vector3[] dirs = new Vector3[3];
        dirs[0] = facingDir;

        Vector3 origin = steering.agent.GetColliderPos();

        float orientation = EAMathUtil.VectorToOrientation(facingDir);
        dirs[1] = EAMathUtil.OrientationToVector(orientation + sideWhiskerAngle * Mathf.Deg2Rad);
        dirs[2] = EAMathUtil.OrientationToVector(orientation - sideWhiskerAngle * Mathf.Deg2Rad);

        for (int i = 0; i < dirs.Length; ++i)
        {
            float dist = (i == 0) ? mainWhiskerLen : sideWhiskerLen;
            Color color = (i == 0) ? Color.red : Color.blue;

            Color oldColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawRay(origin, dirs[i] * dist);
            Gizmos.color = oldColor;
        }
    }
}
