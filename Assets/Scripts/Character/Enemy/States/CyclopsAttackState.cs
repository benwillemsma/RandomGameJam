using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CyclopsAttackState<Data> : CharacterState<Data> where Data: CyclopsData
{
    #region EnemyState Variables

    protected Collider attackCollider;

    #endregion

    public CyclopsAttackState(Data characterData, Collider attackCollider) : base(characterData)
    {
        this.attackCollider = attackCollider;
    }

    public override IEnumerator EnterState(BaseState prevState)
    {
        if (attackCollider) attackCollider.enabled = true;
        return base.EnterState(prevState);
    }

    public override IEnumerator ExitState(BaseState nextState)
    {
        if (attackCollider) attackCollider.enabled = false;
        return base.ExitState(nextState);
    }
}
