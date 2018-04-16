using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CyclopsAttackState : CyclopsWalkingState
{
    #region AttackState Variables

    protected static float beamCooldown;

    #endregion

    public CyclopsAttackState(CyclopsData characterData) : base(characterData) { }

    public override IEnumerator EnterState(BaseState prevState)
    {
        agent.updateRotation = false;
        return base.EnterState(prevState);
    }

    public override IEnumerator ExitState(BaseState nextState)
    {
        agent.updateRotation = true;
        return base.ExitState(nextState);
    }

    protected override void UpdateMovement()
    {
        Vector3 forward = player.transform.position - data.transform.position;
        forward.y = 0;
        data.transform.rotation = Quaternion.Slerp(data.transform.rotation, Quaternion.LookRotation(forward, Vector3.up), Time.deltaTime);
    }

    protected override void UpdateAI()
    {
        base.UpdateAI();

        if ((data.transform.position - player.transform.transform.position).magnitude < 20)
        {
            float angle = Vector3.SignedAngle(data.transform.forward, (player.transform.position - data.transform.position).normalized, Vector3.up);
            if (Mathf.Abs(angle) <= 60)
            {
                if (angle >= 0) stateManager.ChangeState(new CyclopsAxeAttack(data, data.rightAxeAttack));
                else if (angle <= 0) stateManager.ChangeState(new CyclopsAxeAttack(data, data.leftAxeAttack));
            }
        }

        else if (hasDetectedPlayer && beamCooldown <= 0 && (data.transform.position - player.transform.transform.position).magnitude > 50)
        {
            //BeamAttack();
        }
    }

    protected override void SetDestination()
    {
        agent.stoppingDistance = 1;
        agent.speed = 8f;

        Vector3 newPoint = player.transform.position - (player.transform.position - data.transform.position).normalized * 18;

        NavMeshHit hit;
        if (NavMesh.FindClosestEdge(newPoint, out hit, 0))
            MoveTo(hit.position);
        else MoveTo(newPoint);
    }
}
