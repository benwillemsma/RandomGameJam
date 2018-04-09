using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopsIdleState : CyclopsState
{
    #region CyclopsIdleState Variables

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
    }

    protected override void UpdateAI()
    {
        if ((rb.transform.position - agent.destination).magnitude < 2)
        {
            data.patrolIndex = Random.Range(0, data.patrolPoints.Length - 1);
            data.StartCoroutine(GameManager.CallAfterDelay
                (() => MoveTo(data.patrolPoints[data.patrolIndex]),
                Random.Range(3, 10)));
        }
    }
}
