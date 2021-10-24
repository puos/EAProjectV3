using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EAActorMover : MonoBehaviour
{
    private EAActor actor = null;
    private float arriveEpsilon = 0.01f;
    private bool isMove = false;
    protected Vector3 targetPosition = Vector3.zero;
    protected System.Action onMoveComplete = null;
    private float decelerationTweeker = 0.3f;
    public bool isReachedSetPos { private get; set; }

    public void Initialize()
    {
        isReachedSetPos = true;
        actor = GetComponent<EAActor>();
    }

    public void UpdateMove()
    {
        if (isMove == false) return;

        Vector3 changedVelocity = Arrive(targetPosition, arriveEpsilon);
        Vector3 curDir = targetPosition - actor.GetPos();
        curDir.Normalize();

        actor.rb.velocity = changedVelocity;
        bool reached = false;

        if ((curDir.magnitude > 0.01f) && (Vector3.Dot(curDir, changedVelocity) < 0)) reached = true;
        if (changedVelocity.magnitude <= 0.01f && Vector3.Distance(actor.GetPos(), targetPosition) <= arriveEpsilon) reached = true;

        if (reached)
        {
            isMove = false;

            actor.rb.isKinematic = true;
            actor.rb.velocity = Vector3.zero;
            actor.rb.angularVelocity = Vector3.zero;
            actor.SetPos(targetPosition);
            actor.rb.isKinematic = false;

            if (onMoveComplete != null) onMoveComplete();
        }
    }

    private Vector3 Arrive(Vector3 targetPos, float epsilon = 0.01f)
    {
        Vector3 toTarget = targetPos - actor.GetPos();
        float dist = toTarget.magnitude;

        if (dist > epsilon)
        {
            float speed = dist / decelerationTweeker;
            speed = Mathf.Min(speed, actor.GetMaxSpeed());

            Vector3 desiredVelocity = toTarget * (speed / dist);
            desiredVelocity.y = actor.rb.velocity.y;
            return desiredVelocity;
        }

        return Vector3.zero;
    }

    public void MoveTo(Vector3 targetPosition, System.Action onMoveComplete = null)
    {
        this.targetPosition = targetPosition;
        this.onMoveComplete = onMoveComplete;

        isMove = true;
    }

    public void SetSpeed(float speed , float epsilon = 0.01f)
    {
        actor.SetMaxSpeed(speed);
        arriveEpsilon = epsilon;
    }

    public void Stop()
    {
        this.targetPosition = Vector3.zero;
        this.onMoveComplete = null;
        isMove = false;

        actor.rb.isKinematic = true;
        actor.rb.velocity = Vector3.zero;
        actor.rb.angularVelocity = Vector3.zero;
        actor.rb.isKinematic = false;
    }
}