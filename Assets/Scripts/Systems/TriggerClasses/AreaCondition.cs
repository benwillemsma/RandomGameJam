using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[System.Serializable]
public class AreaCondition : Condition
{
    public GameObject checkObject;
    public bool UseColider;
    public bool UsePlayer;

    public Bounds triggerArea = new Bounds(Vector3.zero, Vector3.one * 2);
    protected List<Rigidbody> bodies = new List<Rigidbody>();

    protected Vector3 center;

    public override bool CheckCondition()
    {
        center = transform.position +
            transform.right * triggerArea.center.x +
            transform.up * triggerArea.center.y +
            transform.forward * triggerArea.center.z;

        if (UsePlayer && GameManager.player)
            checkObject = GameManager.player.gameObject;

        if (checkObject)
        {
            for (int i = 0; i < bodies.Count; i++)
            {
                if (bodies[i].gameObject == checkObject)
                    return conditionIsMet = true;
            }
        }
        return conditionIsMet = false;
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!bodies.Contains(other.attachedRigidbody))
        {
            bodies.Add(other.attachedRigidbody);
            CheckCondition();
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (bodies.Contains(other.attachedRigidbody))
        {
            bodies.Remove(other.attachedRigidbody);
            CheckCondition();
        }
    }
}
