using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbingState : PlayerState
{
    protected ClimbingNode[] currentNodes = new ClimbingNode[2];

    protected Vector3 moveDirection;
    protected Vector3 lookDirection;

    protected float movementSpeed = 3f;
    protected float moveX = 0;
    protected float moveY = 0;

    private const int NONE = -1;
    private const int RIGHT = 0;
    private const int LEFT = 1;
    private const int UP = 2;
    private const int DOWN = 3;

    private int nodeIndex;
    private int movePolarity = RIGHT;

    private bool moving = false;
    private bool freehang = false;

    public PlayerClimbingState(PlayerData player, ClimbingNode node) : base(player)
    {
        currentNodes[0] = node;
        currentNodes[1] = node;
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
        base.UpdateState();
    }
    protected override void UpdatePaused() { }
    protected override void UpdateTransition() { }

    protected override void UpdateInput()
    {
        if (!moving)
        {
            moveX = Input.GetAxisRaw("Horizontal");
            moveY = Input.GetAxisRaw("Vertical");

            moveDirection = new Vector3(moveX, moveY, 0);

            if ((!currentNodes[0] && !currentNodes[1])
                || (!currentNodes[0].Active && !currentNodes[1].Active)
                || (!currentNodes[0].gameObject.activeInHierarchy && !currentNodes[1].gameObject.activeInHierarchy))
                stateManager.ChangeState(new PlayerWalkingState(data));

            if (Input.GetButtonDown("Jump"))
                Jump();

            else if (Vector3.Dot(Vector3.Project(data.transform.forward, Vector3.up), lookDirection) >= 0)
            {
                if (moveDirection.magnitude > Mathf.Epsilon)
                {
                    nodeIndex = Mathf.RoundToInt(Mathf.Atan2(moveDirection.x, moveDirection.y) / Mathf.PI * 4);
                    if (nodeIndex < 0) nodeIndex += 8;

                    movePolarity = FindNextMove();
                    ClimbingNode NextNode = FindNextNode();
                    if (NextNode as ClimbingNode)
                    {
                        moving = true;
                        data.StartCoroutine(Climb(NextNode as ClimbingNode));
                    }
                }
            }
        }
    }
    protected override void UpdateMovement()
    {
        freehang = currentNodes[0].freehang || currentNodes[1].freehang;
        if (!moving && currentNodes[0] && currentNodes[1])
        {
            lookDirection = Camera.main.transform.forward;
            lookDirection = Vector3.ProjectOnPlane(lookDirection, rb.transform.up);

            //Root Position - While Stationary
            rb.transform.position = Vector3.Lerp(rb.transform.position, (currentNodes[1].PlayerPosition + currentNodes[0].PlayerPosition) / 2, Time.deltaTime * 5);

            //Root Rotation - While Stationary
            Quaternion desiredRotation = Quaternion.Lerp(currentNodes[1].transform.rotation, currentNodes[0].transform.rotation, 0.5f);
            if (freehang) desiredRotation = Quaternion.Euler(0, desiredRotation.eulerAngles.y, desiredRotation.eulerAngles.z);
            rb.transform.rotation = Quaternion.Lerp(rb.transform.rotation, desiredRotation, Time.deltaTime * 5);
        }
    }
    protected override void UpdateAnimator() { }
    protected override void UpdateIK()
    {
        if (!moving)
        {
            IK.SetIKPositions(currentNodes[0].rightHand, currentNodes[1].leftHand, currentNodes[0].rightFoot, currentNodes[1].leftFoot);

            IK.GlobalWeight = 1;
            IK.RightFoot.weight = Mathf.Lerp(IK.RightFoot.weight, freehang ? 0 : 1, Time.deltaTime * 2);
            IK.LeftFoot.weight = Mathf.Lerp(IK.RightFoot.weight, freehang ? 0 : 1, Time.deltaTime * 2);
            IK.HeadWeight = 0;
        }
    }

    protected override void UpdatePhysics() { }

    //State Behaviour
    private int FindNextMove()
    {
        int move = IndexPolarity(nodeIndex);
        if (move == NONE) // Moving Up or Down
        {
            if (currentNodes[1] != currentNodes[0])
                move = (movePolarity + 1) % 2;
            else move = movePolarity;
        }
        else if (currentNodes[1] != currentNodes[0])
            move = (movePolarity + 1) % 2;
        return move;
    }

    private ClimbingNode FindNextNode()
    {
        ClimbingNode baseNode = currentNodes[movePolarity];
        if (currentNodes[0] != currentNodes[1])
            baseNode = currentNodes[(movePolarity + 1) % 2];

        nodeIndex = (nodeIndex + baseNode.Rotation) % 8;
        if (currentNodes[0] == currentNodes[1])
        {
            //Find direct Neighbour from currentNode
            if (!baseNode.neighbours[nodeIndex])
            {
                //No direct Neighbour found searching for indirect Neighbour
                if (baseNode.neighbours[(nodeIndex + 1) % 8])
                    nodeIndex = (nodeIndex + 1) % 8;
                else if (baseNode.neighbours[Mathf.Abs((nodeIndex - 1)) % 8])
                    nodeIndex = Mathf.Abs((nodeIndex - 1)) % 8;
                else return baseNode;
            }
            return baseNode.neighbours[nodeIndex];
        }
        return baseNode;
    }
    private int IndexPolarity(int index)
    {
        int polarity;
        if (index < 4 && index > 0)
            polarity = RIGHT;
        else if (index > 4)
            polarity = LEFT;
        else polarity = NONE;
        return polarity;
    }

    //State Actions
    private void Jump()
    {
        anim.SetBool("climbing", false);
        anim.SetBool("isGrounded", false);

        //Wall Eject
        if (Vector3.Dot(currentNodes[0].transform.transform.forward, lookDirection) < 0)
        {
            rb.transform.LookAt(rb.transform.position + lookDirection);
            rb.velocity = Vector3.ProjectOnPlane(lookDirection.normalized, Vector3.up) * 2.5f + rb.transform.up * 2;
        }
        //Drop/Move allong wall
        else rb.velocity = (data.transform.right * moveX / 2 + Vector3.up * moveY) * 5 + Vector3.up;

        rb.transform.localEulerAngles = new Vector3(0, rb.transform.localEulerAngles.y, rb.transform.localEulerAngles.z);
        stateManager.ChangeState(new PlayerWalkingState(data));
    }

    private IEnumerator Climb(ClimbingNode nextNode)
    {
        UpdateAnimator();

        elapsedTime = 0;
        while (elapsedTime <= 1)
        {
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
                yield break;
            }
            //Move Right Side
            if (movePolarity == RIGHT)
            {
                IKController.LerpIKTarget(IK.RightHand, IKTarget.FromTransform(currentNodes[RIGHT].rightHand.transform), IKTarget.FromTransform(nextNode.rightHand.transform), elapsedTime);
                IKController.LerpIKTarget(IK.RightFoot, IKTarget.FromTransform(currentNodes[RIGHT].rightFoot.transform), IKTarget.FromTransform(nextNode.rightFoot.transform), elapsedTime);
            }
            else
            {
                IK.RightHand.Set(currentNodes[RIGHT].rightHand);
                IK.RightFoot.Set(currentNodes[RIGHT].rightFoot);
            }

            //Move Left Side
            if (movePolarity == LEFT)
            {
                IKController.LerpIKTarget(IK.LeftHand, IKTarget.FromTransform(currentNodes[LEFT].leftHand.transform), IKTarget.FromTransform(nextNode.leftHand.transform), elapsedTime);
                IKController.LerpIKTarget(IK.LeftFoot, IKTarget.FromTransform(currentNodes[LEFT].leftFoot.transform), IKTarget.FromTransform(nextNode.leftFoot.transform), elapsedTime);
            }
            else
            {
                IK.LeftHand.Set(currentNodes[LEFT].leftHand);
                IK.LeftFoot.Set(currentNodes[LEFT].leftFoot);
            }

            //Root Rotaion - While moving
            Quaternion desiredRotation = Quaternion.Lerp(
                Quaternion.Lerp(currentNodes[LEFT].transform.rotation, currentNodes[RIGHT].transform.rotation, 0.5f),
                Quaternion.Lerp(currentNodes[(movePolarity + 1) % 2].transform.rotation, nextNode.transform.rotation, 0.5f),
                elapsedTime
                );
            if (freehang) desiredRotation = Quaternion.Euler(0, desiredRotation.eulerAngles.y, desiredRotation.eulerAngles.z);
            rb.transform.rotation = Quaternion.Lerp(rb.transform.rotation, desiredRotation, Time.deltaTime * 5);

            //Root Position - While moving
            rb.transform.position = Vector3.Lerp(
                (currentNodes[LEFT].PlayerPosition + currentNodes[RIGHT].PlayerPosition) / 2,
                (currentNodes[(movePolarity + 1) % 2].PlayerPosition + nextNode.PlayerPosition) / 2,
                elapsedTime);

            elapsedTime += Time.deltaTime * movementSpeed;
            yield return null;
        }
        currentNodes[movePolarity] = nextNode;
        moving = false;
    }

    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }

    public override void OnCollisionEnter(Collision collision) { }
    public override void OnCollisionStay(Collision collision) { }
    public override void OnCollisionExit(Collision collision) { }
}
