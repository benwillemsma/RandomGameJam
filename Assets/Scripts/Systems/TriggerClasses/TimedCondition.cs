using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TimedCondition : Condition
{
    public float timerAmount = 1;
    public bool loop;
    
    private bool isCounting;

    IEnumerator StartTimer()
    {
        isCounting = true;
        yield return new WaitForSeconds(timerAmount);
        ConditionMet = true;
        if (loop)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            isCounting = false;
        }
    }

    public override bool CheckCondition()
    {
        if (!isCounting)
        {
            StartCoroutine(StartTimer());
            ConditionMet = false;
        }
        return conditionIsMet;
    }

    public override void ResetCondition()
    {
        base.ResetCondition();
        StopAllCoroutines();
        isCounting = false;
    }
}