using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class CyclopsBaseState<Data> : CharacterState<Data> where Data: CyclopsData
{
    #region CyclopsState Variables

    protected NavMeshAgent agent;

    #endregion

    public CyclopsBaseState(Data characterData) : base(characterData) { }

    protected abstract void UpdateNavAgent();

    protected void MoveTo(Transform point)
    {
        agent.SetDestination(point.position);
    }

    protected void MoveTo(Vector3 point)
    {
        agent.SetDestination(point);
    }
}
