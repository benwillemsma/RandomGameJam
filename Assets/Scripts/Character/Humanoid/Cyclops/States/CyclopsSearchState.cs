using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopsSearchState : CyclopsWalkingState
{
    #region CyclopsSearchState Variables

    #endregion

    public CyclopsSearchState(CyclopsData characterData) : base(characterData)
    {
        data.detectionLevel = data.searchThreshold;
    }

    public override IEnumerator EnterState(BaseState prevState)
    {
        SetDestination();
        return base.EnterState(prevState);
    }

    protected override void SetDestination()
    {
        if (hasDetectedPlayer)
        {
            agent.speed = 5;
            MoveTo(player.transform);
        }
        else base.SetDestination();
    }
}
