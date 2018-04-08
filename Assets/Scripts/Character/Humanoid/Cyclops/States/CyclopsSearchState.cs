using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopsSearchState : CyclopsState
{
    #region CyclopsSearchState Variables

    #endregion

    public CyclopsSearchState(CyclopsData characterData) : base(characterData) { }

    public override IEnumerator EnterState(BaseState prevState)
    {
        return base.EnterState(prevState);
    }

    public override IEnumerator ExitState(BaseState nextState)
    {
        return base.ExitState(nextState);
    }

    protected override void UpdateAI() { }
}
