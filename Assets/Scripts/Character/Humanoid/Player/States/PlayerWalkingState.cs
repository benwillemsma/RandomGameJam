using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingState : PlayerState
{
    private bool grounded = true;
    private bool sprinting = false;

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
        CheckGround();
        movementDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        movementDirection = rb.transform.rotation * movementDirection;
        movementDirection = Vector3.ProjectOnPlane(movementDirection, Vector3.up);
        sprinting = Input.GetButton("Sprint");
    }
    protected override void UpdateMovement()
    {
        if (sprinting) movementDirection *= 1.5f;
        rb.transform.rotation = Quaternion.Lerp(rb.transform.rotation, properRotation, Time.deltaTime * 5);
        rb.transform.Rotate(rb.transform.up, Input.GetAxis("Mouse X") * Time.deltaTime * data.CameraSensitivity, Space.World);
    }
    protected override void UpdatePhysics()
    {
        if (grounded)
        {
            if (Input.GetButton("Jump") && rb.velocity.y <= 0)
                Jump();
            else
            {
                float fallspeed = rb.velocity.y;
                rb.velocity = movementDirection * data.runSpeed;
                rb.velocity += Vector3.up * fallspeed;
            }
        }
        else rb.AddForce(Vector3.down * 10);
    }

    // State Actions
    private void Jump()
    {
        grounded = false;
        rb.AddForce(Vector3.up * data.jumpForce, ForceMode.Impulse);
    }

    private void CheckGround()
    {
        Vector3 velocity = Vector3.ProjectOnPlane(rb.velocity.normalized, Vector3.up) * 0.2f;
        Vector3 start = rb.transform.position + rb.transform.up + velocity;
        Vector3 direction = -rb.transform.up;

        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, 1.1f, ~data.groundMask))
            grounded = true;
    }

    //Physics  Functions
    public override void OnTriggerEnter(Collider other)
    {
        if (!inTransition)
        {
            if (other.tag == "ClimbingNode")
            {
                ClimbingNode node = other.gameObject.GetComponent<ClimbingNode>();
                if (node) stateManager.ChangeState(new NodeClimbingState(data, node));
            }
            else if (other.tag == "ClimbingSurface")
            {
                stateManager.ChangeState(new SurfaceClimbingState(data, other));
            }
        }
    }
}
