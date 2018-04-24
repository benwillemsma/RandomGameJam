using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopsWalkingState : CyclopsState
{
    #region CyclopsIdleState Variables

    private float turnAngle;

    #endregion

    public CyclopsWalkingState(CyclopsData characterData) : base(characterData) { }

    public override IEnumerator EnterState(BaseState prevState)
    {
        data.patrolIndex = Random.Range(0, data.patrolPoints.Length - 1);
        return base.EnterState(prevState);
    }

    protected override void UpdateAnimator()
    {
        float speed = Vector3.Dot(agent.velocity, data.transform.forward);
        Debug.Log(speed);
        anim.SetFloat("Speed", speed);
        anim.SetFloat("TurnAngle", turnAngle);
    }

    protected override void UpdateAI()
    {
        SetDestination();
        base.UpdateAI();
    }

    protected virtual void SetDestination()
    {
        agent.stoppingDistance = 5;
        agent.speed = 5;

        if ((rb.transform.position - agent.destination).magnitude <= agent.stoppingDistance)
        {
            data.patrolIndex = Random.Range(0, data.patrolPoints.Length - 1);
            MoveTo(data.patrolPoints[data.patrolIndex]);
        }
    }

    protected override void MoveTo(Transform point)
    {
        base.MoveTo(point);
        Turn(point.position);
    }

    protected override void MoveTo(Vector3 point)
    {
        base.MoveTo(point);
        Turn(point);
    }

    protected void Turn(Vector3 nextPoint)
    {
        if (agent.path.corners.Length > 1)
            nextPoint = agent.path.corners[1];

        Debug.DrawLine(data.transform.position, nextPoint, Color.red, 0.5f);
        turnAngle = Vector3.SignedAngle(data.transform.forward, nextPoint - data.transform.position, Vector3.up);
    }
}
