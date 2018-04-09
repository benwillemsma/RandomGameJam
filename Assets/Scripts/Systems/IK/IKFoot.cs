using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFoot : MonoBehaviour
{
    public bool rightfoot;

    public CharacterData data;
    public IKController IK;

    public Transform foot;
    public Transform heel;
    public Transform toes;

    private Quaternion rotationOffset;
    private Vector3 positionOffset;

    private void Start()
    {
        transform.parent = null;
        positionOffset = transform.position - foot.position;

        if (rightfoot)
        {
            IK.RightFoot.rotationWeight = 1;
            IK.RightFoot.rotation = foot.rotation;
        }
        else
        {
            IK.LeftFoot.rotationWeight = 1;
            IK.LeftFoot.rotation = foot.rotation;
        }
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, data.transform.right), Vector3.up);
        transform.position = foot.position;
        AdjustFootIK();
    }

    private void AdjustFootIK()
    {
        Vector3 heelPoint = heel.position;
        RaycastHit heelHit;
        if (Physics.Raycast(heel.position + Vector3.up * 2, Vector3.down, out heelHit, 10))
            heelPoint = heelHit.point;

        Vector3 toePoint = toes.position;
        RaycastHit toeHit;
        if (Physics.Raycast(toes.position + Vector3.up * 2, Vector3.down, out toeHit, 10))
            toePoint = toeHit.point;

        Vector3 forward = Vector3.ProjectOnPlane(toePoint - heelPoint, data.transform.right);
        Debug.DrawRay(transform.position, toePoint - heelPoint, Color.magenta);
        Quaternion newRotation = Quaternion.LookRotation(forward, Vector3.up);

        if (rightfoot) IK.RightFoot.rotation = Quaternion.Lerp(IK.RightFoot.rotation, newRotation, Time.deltaTime * 5); 
        else IK.LeftFoot.rotation = Quaternion.Lerp(IK.RightFoot.rotation, newRotation, Time.deltaTime * 5); 

        Debug.DrawRay(transform.position, foot.forward, Color.red);
    }
}
