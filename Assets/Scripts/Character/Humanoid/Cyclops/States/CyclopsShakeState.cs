using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopsShakeState : CyclopsState
{
    private float shakeDuration = 2;
    private float shakeDelay = 5;

    public CyclopsShakeState(CyclopsData cyclops) : base(cyclops) { }

    public override IEnumerator EnterState(BaseState prevState)
    {
        MoveTo(data.transform.position);
        return base.EnterState(prevState);
    }

    protected override void UpdateAI()
    {
        if (!data.beingClimbed) stateManager.ChangeState(new CyclopsAttackState(data));
    }
}
