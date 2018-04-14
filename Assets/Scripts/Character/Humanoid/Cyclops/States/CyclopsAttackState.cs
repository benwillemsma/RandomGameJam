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
            float angle = Vector3.SignedAngle(data.transform.forward, (player.transform.position - data.transform.position).normalized, Vector3.up);
            if (Mathf.Abs(angle) <= 60)
            {
                if (angle >= 0)
                    stateManager.ChangeState(new CyclopsAxeAttack(data, data.rightAxeAttack));
                else if (angle <= 0)
                    stateManager.ChangeState(new CyclopsAxeAttack(data, data.leftAxeAttack));
            }
        }

        else if (hasDetectedPlayer && beamCooldown <= 0 && (rb.transform.position - agent.destination).magnitude > 50)
        {
            //BeamAttack();
        }
    }

    protected override void SetDestination()
    {
        agent.stoppingDistance = 19;
        agent.speed = 7f;
        MoveTo(player.transform.position);
    }
}
