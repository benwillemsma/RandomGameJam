using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopsWalkingState : CyclopsState
{
    #region CyclopsIdleState Variables

    private float turnAngle;

    #endregion

    public CyclopsWalkingState(CyclopsData characterData) : base(characterData)
    {
    }

    public override IEnumerator EnterState(BaseState prevState)
    {
        data.patrolIndex = Random.Range(0, data.patrolPoints.Length - 1);
        return base.EnterState(prevState);
    }

    protected override void UpdateAnimator()
    {
        anim.SetFloat("Speed", agent.velocity.magnitude);
        anim.SetFloat("TurnAngle", turnAngle);
        turnAngle = 0;
    }

    protected override void UpdateAI()
    {
        SetDestination();
        base.UpdateAI();
    }

    protected virtual void SetDestination()
    {
        agent.speed = 5;

        if ((rb.transform.position - agent.destination).magnitude <= agent.stoppingDistance)
        {
            data.patrolIndex = Random.Range(0, data.patrolPoints.Length - 1);
            data.StartCoroutine(GameManager.CallAfterDelay
                (() => MoveTo(data.patrolPoints[data.patrolIndex]),
                Random.Range(3, 10)));
        }
    }

    protected override void MoveTo(Transform point)
    {
        float newAngle = Vector3.SignedAngle(rb.transform.forward, point.position - rb.transform.position, Vector3.up);
        turnAngle = Mathf.Lerp(turnAngle, newAngle, Time.deltaTime * agent.speed);
        base.MoveTo(point);
    }
    protected override void MoveTo(Vector3 point)
    {
        float newAngle = Vector3.SignedAngle(rb.transform.forward, point - rb.transform.position, Vector3.up);
        turnAngle = Mathf.Lerp(turnAngle, newAngle, Time.deltaTime * agent.speed);
        base.MoveTo(point);
    }
}
