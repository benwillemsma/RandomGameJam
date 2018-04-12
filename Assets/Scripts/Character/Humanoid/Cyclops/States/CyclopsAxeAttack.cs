using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopsAxeAttack : CyclopsAttack
{
    #region ChargeAttack Variables

    #endregion

    public CyclopsAxeAttack(CyclopsData characterData, EffectManager attackCollider) : base(characterData, attackCollider) { }

    protected override void UpdateAI()
    {
        if (elapsedTime >= attackCollider.duration) stateManager.ChangeState(new CyclopsAttackState(data));
    }

    public override IEnumerator EnterState(BaseState prevState)
    {
        float direction = Vector3.Dot(data.transform.right, (player.transform.position - data.transform.position).normalized) * 2;

        anim.SetFloat("HackDirection", direction);
        anim.SetTrigger("HackAttack");
        return base.EnterState(prevState);
    }

    public override IEnumerator ExitState(BaseState prevState)
    {
        anim.ResetTrigger("HackAttack");
        return base.EnterState(prevState);
    }
}
