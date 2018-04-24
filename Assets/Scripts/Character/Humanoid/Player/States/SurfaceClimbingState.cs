using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceClimbingState : PlayerState
{
    Transform parentRef;
    Vector3 offset;

    private RaycastHit last_SurfaceHit;
    private RaycastHit cur_SurfaceHit;

    private float movementSpeed = 0.3f;
    private float moveX = 0;
    private float moveY = 0;

    private bool freehang = false;

    public SurfaceClimbingState(PlayerData player,Transform surface) : base(player)
    {
        parentRef = surface;
        climbing = true;
    }

    //Transitions
    public override IEnumerator EnterState(BaseState prevState)
    {
        if (cur_SurfaceHit.transform.root.tag == "Enemy/Cyclops")
            cur_SurfaceHit.transform.root.GetComponent<CyclopsData>().beingClimbed = true;

        offset = parentRef.InverseTransformDirection(data.transform.position - parentRef.position);
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        anim.SetBool("Climbing", true);
        yield return base.EnterState(prevState);
    }
    public override IEnumerator ExitState(BaseState nextState)
    {
        if (cur_SurfaceHit.transform.root.tag == "Enemy/Cyclops")
            cur_SurfaceHit.transform.root.GetComponent<CyclopsData>().beingClimbed = false;

        anim.SetBool("Climbing", false);
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

    protected override void UpdateInput()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");

        movementDirection = new Vector3(moveX, moveY, 0).normalized * movementSpeed;
        movementDirection = rb.transform.rotation * movementDirection;
        movementDirection = Vector3.ProjectOnPlane(movementDirection, -cur_SurfaceHit.normal);

        //data.UseStamina(1 * Time.deltaTime);
        if (!climbing || data.Stamina <= 0 || Vector3.Dot(-data.transform.forward,Vector3.up) > 0.9f)
            stateManager.ChangeState(new PlayerWalkingState(data));
    }

    protected override void UpdateMovement()
    {
        if (cur_SurfaceHit.collider) freehang = Vector3.Dot(-cur_SurfaceHit.normal, Vector3.up) > 0.5f;
        Vector3 forward = Vector3.Lerp(-last_SurfaceHit.normal, -cur_SurfaceHit.normal, Time.deltaTime * 3);
        properRotation = Quaternion.LookRotation(forward, Vector3.up);
        rb.transform.rotation = Quaternion.Lerp(rb.transform.rotation, properRotation, Time.deltaTime * 5);
    }

    protected override void UpdatePhysics()
    {
        if (Input.GetButton("Jump"))
            Jump();
        else if (parentRef)
        {
            data.transform.position = parentRef.position + parentRef.rotation * offset;
            Vector3 velocity =  movementDirection * data.runSpeed;
            if (cur_SurfaceHit.collider) velocity += -cur_SurfaceHit.normal * 0.2f;
            offset += parentRef.InverseTransformDirection(velocity * Time.deltaTime);
        }
    }

    private bool SurfaceCheck()
    {
        Vector3 start = rb.transform.position + rb.transform.up - rb.transform.forward * 0.2f;
        Vector3 direction = rb.transform.forward;

        Debug.DrawRay(start, direction, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, 1.5f, data.climbingMask))
        {
            last_SurfaceHit = cur_SurfaceHit;
            cur_SurfaceHit = hit;
            if (cur_SurfaceHit.transform != parentRef)
            {
                parentRef = cur_SurfaceHit.transform;
                offset = parentRef.InverseTransformDirection(data.transform.position - parentRef.position);
            }
            return true;
        }
        return false;
    }

    //State Actions
    private void Jump()
    {
        anim.SetBool("Climbing", false);
        
        //Drop/Move allong wall
        rb.velocity = (data.transform.right * moveX / 2 + Vector3.up * moveY) * 5 + Vector3.up;

        rb.transform.localEulerAngles = new Vector3(0, rb.transform.localEulerAngles.y, rb.transform.localEulerAngles.z);
        stateManager.ChangeState(new PlayerWalkingState(data));
    }

    //Physics  Functions
    public override void OnTriggerEnter(Collider collider)
    {
        if (!inTransition)
        {
            if (collider.tag == "ClimbingNode")
            {
                ClimbingNode node = collider.gameObject.GetComponent<ClimbingNode>();
                if (node) stateManager.ChangeState(new NodeClimbingState(data, node));
            }
        }
    }
}
