using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander1 : Steering
{
    private float wanderOffset = 3.5f;
    private float wanderRadius = 4;
    private float wanderRate = 0.5f;

    private float wanderOrientation = 0;

    public override Vector3 GetSteering()
    {
        float characterOrientation = steering.rb.RotationInRadians();

        wanderOrientation += RandomBinomial() * wanderRate;

        float targetOrientation = wanderOrientation + characterOrientation;

        Vector3 targetPosition = steering.tr.position + (EAMathUtil.OrientationToVector(characterOrientation) * wanderOffset);

        targetPosition = targetPosition + (EAMathUtil.OrientationToVector(targetOrientation) * wanderRadius);

        return steering.Seek(targetPosition);
    }

    float RandomBinomial()
    {
        return Random.value - Random.value;
    }

    public override void DrawGizmo()
    {
        float characterOrientation = steering.rb.RotationInRadians();

        float targetOrientation = wanderOrientation + characterOrientation;

        Vector3 wanderPosition = steering.tr.position + (EAMathUtil.OrientationToVector(characterOrientation) * wanderOffset);

        DebugExtension.DrawLineArrow(steering.tr.position, wanderPosition, Color.black);

        Vector3 targetPosition = wanderPosition + (EAMathUtil.OrientationToVector(targetOrientation) * wanderRadius);

        DebugExtension.DrawCircle(targetPosition, Vector3.up, Color.blue, wanderRadius);
        DebugExtension.DrawLineArrow(wanderPosition, targetPosition, Color.white);
    }

}
