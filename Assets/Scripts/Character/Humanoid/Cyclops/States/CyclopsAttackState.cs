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
        TryDetectPlayer();
        SetDestination();

        if ((rb.transform.position - agent.destination).magnitude <= agent.stoppingDistance)
        {
            float direction = Vector3.Dot(data.transform.forward, (player.transform.position - data.transform.position).normalized);
            if (direction >= 0.6f) stateManager.ChangeState(new CyclopsAxeAttack(data, data.AxeAttack));
        }

        else if (hasDetectedPlayer && beamCooldown <= 0 && (rb.transform.position - agent.destination).magnitude > 50)
        {
            //BeamAttack();
        }
    }

    protected override void SetDestination()
    {
        agent.stoppingDistance = 17;
        agent.speed = 7f;
        MoveTo(player.transform.position);
    }
}
