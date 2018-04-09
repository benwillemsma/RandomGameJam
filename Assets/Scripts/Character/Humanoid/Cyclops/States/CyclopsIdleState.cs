using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopsIdleState : CyclopsState
{
    #region CyclopsIdleState Variables

    private float turnAngle;

    #endregion

    public CyclopsIdleState(CyclopsData characterData) : base(characterData) { }

    public override IEnumerator EnterState(BaseState prevState)
    {
        data.patrolIndex = Random.Range(0, data.patrolPoints.Length - 1);
        return base.EnterState(prevState);
    }

    public override IEnumerator ExitState(BaseState nextState)
    {
        return base.ExitState(nextState);
    }

    protected override void UpdateAnimator()
    {
        anim.SetFloat("Speed", agent.velocity.magnitude);
        anim.SetFloat("TurnAngle", turnAngle);
        turnAngle = 0;
    }

    protected override void UpdateAI()
    {
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
        Turn(point.position);
        base.MoveTo(point);
    }
    protected override void MoveTo(Vector3 point)
    {
        Turn(point);
        base.MoveTo(point);
    }

    private void Turn(Vector3 point)
    {
        turnAngle = Vector3.SignedAngle(rb.transform.forward, point - rb.transform.position, Vector3.up);
    }
}
