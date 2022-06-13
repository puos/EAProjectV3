using UnityEngine;

public class Wander2 : Steering
{
    private float wanderRadius = 3.2f;
    private float wanderDistance = 1f;
    private float wanderJitter   = 1000f;
    Vector3 wanderTarget = Vector3.zero;

    public override Vector3 GetSteering() 
    {
        float jitter = wanderJitter * Time.deltaTime;
        wanderTarget += new Vector3(Random.Range(-1f, 1f) * jitter, 0f, Random.Range(-1f, 1f) * jitter);
        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;
        Vector3 targetPosition = steering.tr.position + steering.tr.forward * wanderDistance + wanderTarget;
        if (!steering.isCanFly) targetPosition.y = steering.tr.position.y;
        return steering.Seek(targetPosition);
    }

    public override void DrawGizmo()
    {
        DebugExtension.DrawCircle(steering.tr.position + steering.tr.forward * wanderDistance, Vector3.up, Color.blue, wanderRadius);
        Vector3 targetPosition = steering.tr.position + steering.tr.forward * wanderDistance + wanderTarget;
        if (!steering.isCanFly) targetPosition.y = steering.tr.position.y;
        DebugExtension.DrawLineArrow(steering.tr.position, targetPosition, Color.green);
    }
}
