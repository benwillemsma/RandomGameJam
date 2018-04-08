using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopsBeamAttack : CyclopsAttackState
{
    #region BeamAttack Variables

    #endregion

    public CyclopsBeamAttack(CyclopsData characterData, EffectManager attackCollider) : base(characterData, attackCollider) { }

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
