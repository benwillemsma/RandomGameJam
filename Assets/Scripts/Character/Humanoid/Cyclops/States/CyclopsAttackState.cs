using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopsAttackState : CyclopsWalkingState
{
    #region AttackState Variables

    #endregion

    public CyclopsAttackState(CyclopsData characterData) : base(characterData)
    {
    }

    protected override void SetDestination()
    {
        agent.speed = 8;
        agent.SetDestination(player.transform.position);

        if ((rb.transform.position - agent.destination).magnitude <= agent.stoppingDistance)
        {
            //attack();
        }
    }
}
