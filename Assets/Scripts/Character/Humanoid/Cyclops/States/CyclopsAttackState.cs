using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopsAttackState : CyclopsWalkingState
{
    #region AttackState Variables

    protected static float beamCooldown;

    #endregion

    public CyclopsAttackState(CyclopsData characterData) : base(characterData) { }

    protected override void UpdateAI()
    {
        base.UpdateAI();
        if ((rb.transform.position - agent.destination).magnitude <= agent.stoppingDistance)
        {
            //AxeAttack();
        }
        else if (hasDetectedPlayer && beamCooldown <= 0 && (rb.transform.position - agent.destination).magnitude > 20)
        {
            //BeamAttack();
        }
    }

    protected override void SetDestination()
    {
        agent.speed = 7f;
        MoveTo(player.transform.position);
    }
}
