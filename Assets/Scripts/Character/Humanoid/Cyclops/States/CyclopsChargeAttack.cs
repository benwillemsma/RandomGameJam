using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopsChargeAttack : CyclopsAttackState
{
    #region ChargeAttack Variables

    #endregion

    public CyclopsChargeAttack(CyclopsData characterData, EffectManager attackCollider) : base(characterData, attackCollider) { }

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
