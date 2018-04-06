using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbingState : PlayerState<PlayerData>
{
    public PlayerClimbingState(PlayerData player) : base(player) { }

    //State Transitions
    public override IEnumerator EnterState(BaseState prevState)
    {
        yield return base.EnterState(prevState);
    }
    public override IEnumerator ExitState(BaseState nextState)
    {
        yield return base.ExitState(nextState);
    }

    //State Updates
    protected override void UpdateState()
    {
        base.UpdateState();
    }
    protected override void UpdatePaused() { }
    protected override void UpdateTransition() { }

    // Character Updates
    protected override void UpdateMovement() { }
    protected override void UpdateAnimator() { }
    protected override void UpdatePhysics() { }
    protected override void UpdateInput() { }

    //Trigger Functions
    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }

    //Colission Functions
    public override void OnCollisionEnter(Collision collision) { }
    public override void OnCollisionStay(Collision collision) { }
    public override void OnCollisionExit(Collision collision) { }
}
