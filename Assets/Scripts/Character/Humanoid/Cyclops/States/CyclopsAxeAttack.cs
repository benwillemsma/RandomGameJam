using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopsAxeAttack : CyclopsAttack
{
    #region ChargeAttack Variables

    #endregion

    public CyclopsAxeAttack(CyclopsData characterData, EffectManager attackCollider) : base(characterData, attackCollider) { }

    protected override void UpdateAI() { }

    public override IEnumerator EnterState(BaseState prevState)
    {
        anim.SetTrigger("HackAttack");
        return base.EnterState(prevState);
    }

    public override IEnumerator ExitState(BaseState prevState)
    {
        anim.ResetTrigger("HackAttack");
        return base.EnterState(prevState);
    }
}
