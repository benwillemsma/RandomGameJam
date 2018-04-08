using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceClimbingState : PlayerState
{
    private List<Collider> Surfaces = new List<Collider>();

    private RaycastHit last_SurfaceHit;
    private RaycastHit cur_SurfaceHit;

    private float movementSpeed = 0.3f;
    private float moveX = 0;
    private float moveY = 0;

    public SurfaceClimbingState(PlayerData player,Collider surface) : base(player)
    {
        Surfaces.Add(surface);
    }

    //Transitions
    public override IEnumerator EnterState(BaseState prevState)
    {
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        yield return base.EnterState(prevState);
    }
    public override IEnumerator ExitState(BaseState nextState)
    {
        rb.useGravity = true;
        yield return base.ExitState(nextState);
    }

    //State Updates
    protected override void UpdateState()
    {
        if (!SurfaceCheck())
        {
            stateManager.ChangeState(new PlayerWalkingState(data));
            return;
        }
        base.UpdateState();
    }
    protected override void UpdatePaused() { }
    protected override void UpdateTransition() { }

    protected override void UpdateInput()
    {
        SurfaceCheck(); moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");

        movementDirection = new Vector3(moveX, moveY, 0).normalized * movementSpeed;
        movementDirection = rb.transform.rotation * movementDirection;
        movementDirection = Vector3.ProjectOnPlane(movementDirection, -cur_SurfaceHit.normal);
    }
    protected override void UpdateMovement()
    {
        Vector3 forward = Vector3.Lerp(-last_SurfaceHit.normal, -cur_SurfaceHit.normal, Time.deltaTime * 3);
        properRotation = Quaternion.LookRotation(forward, Vector3.up);
        rb.transform.rotation = Quaternion.Lerp(rb.transform.rotation, properRotation, Time.deltaTime * 5);
    }
    protected override void UpdatePhysics()
    {
        if (Input.GetButton("Jump")) Jump();
        else
        {
            rb.velocity = movementDirection * data.runSpeed;
            if (cur_SurfaceHit.collider) rb.velocity += -cur_SurfaceHit.normal * 0.2f;
        }
    }

    private bool SurfaceCheck()
    {
        Vector3 start = rb.transform.position + rb.transform.up;
        Vector3 direction = rb.transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, 1.1f, data.climbingMask))
        {
            last_SurfaceHit = cur_SurfaceHit;
            cur_SurfaceHit = hit;
            return true;
        }
        return false;
    }

    //State Actions
    private void Jump()
    {
        anim.SetBool("climbing", false);
        anim.SetBool("isGrounded", false);
        
        //Drop/Move allong wall
        rb.velocity = (data.transform.right * moveX / 2 + Vector3.up * moveY) * 5 + Vector3.up;

        rb.transform.localEulerAngles = new Vector3(0, rb.transform.localEulerAngles.y, rb.transform.localEulerAngles.z);
        stateManager.ChangeState(new PlayerWalkingState(data));
    }

    //Physics  Functions
    public override void OnTriggerEnter(Collider collider)
    {
        if (!Surfaces.Contains(collider))
            Surfaces.Add(collider);
        
        if (!inTransition)
        {
            if (collider.tag == "ClimbingNode")
            {
                ClimbingNode node = collider.gameObject.GetComponent<ClimbingNode>();
                if (node) stateManager.ChangeState(new NodeClimbingState(data, node));
            }
        }
    }

    public override void OnTriggerExit(Collider collider)
    {
        if (Surfaces.Contains(collider))
        {
            Surfaces.Remove(collider);
            if (Surfaces.Count == 0)
                stateManager.ChangeState(new PlayerWalkingState(data));
        }
    }
}
