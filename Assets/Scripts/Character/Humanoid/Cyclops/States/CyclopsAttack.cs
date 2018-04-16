using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CyclopsAttack : CyclopsAttackState
{
    #region AttackState Variables

    protected EffectManager attackCollider;
    protected CameraShaker cameraShaker;

    #endregion

    public CyclopsAttack(CyclopsData characterData, EffectManager attackCollider) : base(characterData)
    {
        if (attackCollider) attackCollider.enabled = true;
        this.attackCollider = attackCollider;
    }

    public override IEnumerator ExitState(BaseState nextState)
    {
        if (attackCollider) attackCollider.enabled = false;
        return base.ExitState(nextState);
    }
}
