using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeClimbingState : PlayerState
{
    private ClimbingNode[] currentNodes = new ClimbingNode[2];

    private Vector3 moveDirection;

    private float movementSpeed = 3f;
    private float moveX = 0;
    private float moveY = 0;

    private const int NONE = -1;
    private const int RIGHT = 0;
    private const int LEFT = 1;
    private const int UP = 2;
    private const int DOWN = 3;

    private int nodeIndex;
    private int movePolarity = RIGHT;

    private bool moving = false;
    private bool freehang = false;

    public NodeClimbingState(PlayerData player, ClimbingNode node) : base(player)
    {
        climbing = true;
        currentNodes[0] = node;
        currentNodes[1] = node;
    }

    //Transitions
    public override IEnumerator EnterState(BaseState prevState)
    {
        if (currentNodes[0].transform.root.tag == "Enemy/Cyclops")
            currentNodes[0].transform.root.GetComponent<CyclopsData>().beingClimbed = true;

        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        IK.HeadWeight = 0;

        elapsedTime = 0;
        while (elapsedTime < 0.5f)
        {
            if (!moving) IK.SetIKPositions(currentNodes[0].rightHand, currentNodes[1].leftHand, currentNodes[0].rightFoot, currentNodes[1].leftFoot);
            rb.transform.position = Vector3.Lerp(rb.transform.position, (currentNodes[1].PlayerPosition + currentNodes[0].PlayerPosition) / 2, elapsedTime * 2);

            Quaternion desiredRotation = currentNodes[0].transform.rotation;
            if (freehang) desiredRotation = Quaternion.Euler(0, desiredRotation.eulerAngles.y, desiredRotation.eulerAngles.z);
            rb.transform.rotation = Quaternion.Lerp(rb.transform.rotation, desiredRotation, elapsedTime * 2);

            IK.GlobalWeight = Mathf.Lerp(IK.RightHand.weight, 1, elapsedTime * 2);
            IK.RightFoot.weight = Mathf.Lerp(IK.RightFoot.weight, freehang ? 0 : 1, elapsedTime * 2);
            IK.LeftFoot.weight = IK.RightFoot.weight;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return base.EnterState(prevState);
    }
    public override IEnumerator ExitState(BaseState nextState)
    {
        if (currentNodes[0].transform.root.tag == "Enemy/Cyclops")
            currentNodes[0].transform.root.GetComponent<CyclopsData>().beingClimbed = false;

        rb.useGravity = true;

        elapsedTime = 0;
        while (elapsedTime < 0.5f)
        {
            rb.transform.rotation = Quaternion.Lerp(rb.transform.rotation, properRotation, elapsedTime * 2);
            IK.GlobalWeight = Mathf.Lerp(IK.RightHand.weight, 0, elapsedTime * 2);
            yield return null;
        }
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
            //data.UseStamina(8 * Time.deltaTime);
            //!climbing ||
            if ( data.Stamina <= 0
                || (!currentNodes[0] && !currentNodes[1]) 
                || (!currentNodes[0].Active && !currentNodes[1].Active)
                || (!currentNodes[0].gameObject.activeInHierarchy && !currentNodes[1].gameObject.activeInHierarchy))
                stateManager.ChangeState(new PlayerWalkingState(data));

            if (Input.GetButtonDown("Jump")) Jump();

            else if (moveDirection.magnitude > Mathf.Epsilon)
            {
                nodeIndex = Mathf.RoundToInt(Mathf.Atan2(moveDirection.x, moveDirection.y) / Mathf.PI * 4);
                if (nodeIndex < 0) nodeIndex += 8;

                movePolarity = FindNextMove();
                ClimbingNode NextNode = FindNextNode();
                if (NextNode && NextNode as ClimbingNode)
                {
                    moving = true;
                    data.StartCoroutine(Climb(NextNode as ClimbingNode));
                }
                else
                {
                    Collider col = SurfaceCheck();
                    if (col) stateManager.ChangeState(new SurfaceClimbingState(data, col.transform));
                }
            }
        }
    }

    protected override void UpdateIK()
    {
        if (!moving) IK.SetIKPositions
            (
            currentNodes[0].rightHand,
            currentNodes[1].leftHand,
            currentNodes[0].rightFoot,
            currentNodes[1].leftFoot
            );
    }

    protected override void UpdateMovement()
    {
        freehang = currentNodes[0].freehang || currentNodes[1].freehang;
        if (!moving && currentNodes[0] && currentNodes[1])
        {
            //Root Position - While Stationary
            rb.transform.position = (currentNodes[1].PlayerPosition + currentNodes[0].PlayerPosition) / 2;

            //Root Rotation - While Stationary
            Quaternion desiredRotation = Quaternion.Lerp(currentNodes[1].transform.rotation, currentNodes[0].transform.rotation, 0.5f);
            if (freehang) desiredRotation = Quaternion.Euler(0, desiredRotation.eulerAngles.y, desiredRotation.eulerAngles.z);
            rb.transform.rotation = desiredRotation;
        }
    }

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
        if (currentNodes[0] != currentNodes[1])
            return currentNodes[(movePolarity + 1) % 2];
        else if (currentNodes[0] == currentNodes[1])
        {
            nodeIndex = (nodeIndex + currentNodes[0].Rotation) % 8;
            //Find direct Neighbour from currentNode
            if (!currentNodes[0].neighbours[nodeIndex])
            {
                //No direct Neighbour found searching for indirect Neighbour
                if (currentNodes[0].neighbours[(nodeIndex + 1) % 8])
                    nodeIndex = (nodeIndex + 1) % 8;
                else if (currentNodes[0].neighbours[Mathf.Abs((nodeIndex - 1)) % 8])
                    nodeIndex = Mathf.Abs((nodeIndex - 1)) % 8;
                else return null;
            }
            return currentNodes[0].neighbours[nodeIndex];
        }
        return null;
    }

    private Collider SurfaceCheck()
    {
        Vector3 start = rb.transform.position + rb.transform.up;
        Vector3 direction = rb.transform.forward;
        RaycastHit[] hit = (Physics.RaycastAll(start, direction, 1f, data.climbingMask));
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider.tag=="ClimbingSurface")
                return hit[i].collider;
        }
        return null;
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
        rb.velocity = (data.transform.right * moveX / 2 + Vector3.up * moveY) * 5 + Vector3.up;

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
            rb.transform.rotation = desiredRotation;

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
}
