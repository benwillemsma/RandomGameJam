﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopsIdleState<Data> : CyclopsBaseState<Data> where Data : CyclopsData
{
    #region CyclopsIdleState Variables

    #endregion

    public CyclopsIdleState(Data characterData) : base(characterData) { }

    public override IEnumerator EnterState(BaseState prevState)
    {
        return base.EnterState(prevState);
    }

    public override IEnumerator ExitState(BaseState nextState)
    {
        return base.ExitState(nextState);
    }

    protected override void UpdateState() { }
    protected override void UpdatePaused() { }
    protected override void UpdateTransition() { }

    protected override void UpdateMovement() { }
    protected override void UpdateNavAgent() { }
    protected override void UpdateAnimator() { }
    protected override void UpdatePhysics() { }

    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }

    public override void OnCollisionEnter(Collision collision) { }
    public override void OnCollisionExit(Collision collision) { }
    public override void OnCollisionStay(Collision collision) { }
}
