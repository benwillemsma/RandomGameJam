using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFoot : MonoBehaviour
{
    public bool rightfoot;

    public HumanoidData data;
    private IKController IK;
    private Animator anim;

    public Transform foot;
    public Transform heel;
    public Transform toes;

    public LayerMask groundMask;

    private Quaternion rotationOffset;
    private Vector3 positionOffset;

    private void Start()
    {
        IK = data.IK;
        anim = data.Anim;
        if (!IK || !anim)
        {
            enabled = false;
            return;
        }

        transform.parent = null;
        positionOffset = transform.position - foot.position;

        if (rightfoot)
        {
            IK.RightFoot.rotationWeight = 1;
            IK.RightFoot.rotation = foot.rotation;

            IK.RightFoot.positionWeight = 1;
            IK.RightFoot.position = foot.position;
        }
        else
        {
            IK.LeftFoot.rotationWeight = 1;
            IK.LeftFoot.rotation = foot.rotation;

            IK.LeftFoot.positionWeight = 1;
            IK.LeftFoot.position = foot.position;
        }
    }

    private void Update()
    {
        if (data)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, data.transform.right), Vector3.up);
            transform.position = foot.position + positionOffset;
            AdjustFootIK();
        }
    }

    private void AdjustFootIK()
    {
        Vector3 heelPoint = Vector3.zero;
        RaycastHit heelHit;
        if (Physics.Raycast(heel.position + Vector3.up * 5, Vector3.down, out heelHit, 10, groundMask))
            heelPoint = heelHit.point;

        Vector3 toePoint = Vector3.zero;
        RaycastHit toeHit;
        if (Physics.Raycast(toes.position + Vector3.up * 5, Vector3.down, out toeHit, 10, groundMask))
            toePoint = toeHit.point;

        if (heelPoint != Vector3.zero && toePoint != Vector3.zero)
        {
            // Rotation
            Vector3 forward = Vector3.ProjectOnPlane(toePoint - heelPoint, data.transform.right);
            Quaternion newRotation = Quaternion.LookRotation(forward, Vector3.up);

            if (rightfoot) data.IK.RightFoot.rotation = Quaternion.Lerp(IK.RightFoot.rotation, newRotation, Time.deltaTime * 5);
            else IK.LeftFoot.rotation = Quaternion.Lerp(IK.LeftFoot.rotation, newRotation, Time.deltaTime * 5);

            // Position
            Vector3 newPosition = heelPoint - heel.localPosition - positionOffset;
            if (rightfoot)
            {
                IK.RightFoot.positionWeight = anim.GetFloat("RightFoot");
                IK.RightFoot.position = Vector3.Lerp(IK.RightFoot.position, newPosition, Time.deltaTime * 10);
            }
            else
            {
                IK.LeftFoot.positionWeight = anim.GetFloat("LeftFoot");
                IK.LeftFoot.position = Vector3.Lerp(IK.LeftFoot.position, newPosition, Time.deltaTime * 10);
            }
        }
    }
}
