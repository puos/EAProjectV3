using UnityEngine;

public class Flee : Steering
{
    private Vector3 vTarget = Vector3.zero;
    public void SetVTarget(Vector3 vTarget)
    {
        this.vTarget = vTarget;
    }

    public override Vector3 GetSteering()
    {
        return steering.Flee(vTarget);
    }

    public override void DrawGizmo()
    {
        DebugExtension.DrawArrow(steering.tr.position, vTarget, Color.grey);
        DebugExtension.DrawPoint(vTarget, Color.grey, 3f);
    }
}