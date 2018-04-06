using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FindType
{
    Tag,
    Layer,
    ByType
}

public enum CompareType
{
    LessThan,
    GreaterThan,
    EqualTo,
    LessThanOrEqual,
    GreaterThanOrEqual,
}

public class AmountCondition : AreaCondition
{
    public FindType typeOfFind;
    public CompareType typeOfCompare;

    public string checkTag;
    public LayerMask layer;
    public Object typeTemplate;
    
    public int amount;
    public int numOfObjects = 0;

    public override bool CheckCondition()
    {
        center = transform.position +
               transform.right * triggerArea.center.x +
               transform.up * triggerArea.center.y +
               transform.forward * triggerArea.center.z;

        numOfObjects = bodies.Count;

        // - Compare Tests - 
        if (typeOfCompare == CompareType.LessThan && numOfObjects < amount)
            ConditionMet = true;

        else if (typeOfCompare == CompareType.GreaterThan && numOfObjects > amount)
            ConditionMet = true;

        else if (typeOfCompare == CompareType.EqualTo && numOfObjects == amount)
            ConditionMet = true;

        else if (typeOfCompare == CompareType.LessThanOrEqual && numOfObjects <= amount)
            ConditionMet = true;

        else if (typeOfCompare == CompareType.GreaterThanOrEqual && numOfObjects >= amount)
            ConditionMet = true;

        else conditionIsMet = false;

        return ConditionMet;
    }

    private bool CheckBase(Collider other)
    {
        if (typeOfFind == FindType.Tag && other.attachedRigidbody.tag == checkTag)
            return true;

        else if (typeOfFind == FindType.Layer && other.attachedRigidbody.gameObject.layer == layer)
            return true;

        else if (typeOfFind == FindType.ByType && typeTemplate != null && other.GetComponentsInChildren(typeTemplate.GetType()) != null)
            return true;

        else return false;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (CheckBase(other)) base.OnTriggerExit(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (CheckBase(other)) base.OnTriggerExit(other);
    }
}
