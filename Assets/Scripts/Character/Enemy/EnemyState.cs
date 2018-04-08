using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyState<Data> : HumanoidState<Data> where Data: EnemyData
{
    #region PlayerState Variables

    protected NavMeshAgent agent;

    #endregion

    public EnemyState(Data enemyData) : base(enemyData)
    {
        agent = data.Agent;
    }

    protected override void UpdateState()
    {
        base.UpdateState();
        if (data.useAI) UpdateAI();
    }

    /// <summary>
    /// Cyclops InputUpdate, Called After StateUpdate;
    /// </summary>
    protected abstract void UpdateAI();

    protected void MoveTo(Transform point)
    {
        agent.SetDestination(point.position);
    }
    protected void MoveTo(Vector3 point)
    {
        agent.SetDestination(point);
    }

}
