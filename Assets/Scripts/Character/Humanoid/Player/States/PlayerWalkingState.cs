using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingState : PlayerState
{
    private bool sprinting;

    public PlayerWalkingState(PlayerData player) : base(player) { }

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
    protected override void UpdateInput()
    {
        movementDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        movementDirection = rb.transform.rotation * movementDirection;
        sprinting = Input.GetButton("Sprint");
        if (Input.GetButton("Jump")) Jump();
    }
    protected override void UpdateMovement()
    {
        if (sprinting) movementDirection *= 1.5f;
        rb.transform.Rotate(rb.transform.up, Input.GetAxis("Mouse X") * Time.deltaTime * data.CameraSensitivity, Space.World);
    }
    protected override void UpdateAnimator() { }
    protected override void UpdatePhysics()
    {
        float fallspeed = rb.velocity.y;
        rb.velocity = movementDirection * data.runSpeed;
        rb.velocity += Vector3.up * fallspeed;
    }
    protected override void UpdateIK() { }

    // State Actions
    private void Jump()
    {
        rb.AddForce(Vector3.up * data.jumpForce, ForceMode.Impulse);
    }

    //Trigger Functions
    public override void OnTriggerEnter(Collider collider)
    {
        ClimbingNode node;
        if (!inTransition)
        {
            node = collider.gameObject.GetComponent<ClimbingNode>();
            if (node) stateManager.ChangeState(new PlayerClimbingState(data, node));
        }
    }
    public override void OnTriggerStay(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }

    //Colission Functions
    public override void OnCollisionEnter(Collision collision) { }
    public override void OnCollisionStay(Collision collision) { }
    public override void OnCollisionExit(Collision collision) { }

}
