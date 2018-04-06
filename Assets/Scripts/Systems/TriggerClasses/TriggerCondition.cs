using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCondition : Condition
{
    public Trigger referenceTrigger;
    public bool isTrue;
    
    public override bool CheckCondition()
    {
        if (referenceTrigger)
            return conditionIsMet = referenceTrigger.ConditionsMet;
        else return true;
    }
}
