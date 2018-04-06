using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConditionType
{
    Timed,                          // - Trigger is simply placed on a timer with an option to loop.
    Area,                           // – Triggered when "CheckObject" is within the trigger's area.
    Destroyed,                      // - Triggered when a specific "CheckObject" is destroyed.
    Amount,                         // - Triggered when defined amount of “objects” are in scene(can be zero).  
    Button,                         // - Triggered with button press.
    Trigger,                        // - Triggered when referenced Trigger returns true.
    BulletHit,                      // - Triggered when shot by buttlet type.
}

[System.Serializable]
[RequireComponent(typeof(Trigger))]
[ExecuteInEditMode]
public class Condition : MonoBehaviour
{
    protected Trigger trigger;
    public Trigger Trigger
    {
        get { return trigger; }
        set { trigger = value; }
    }

    [Space(20)]
    protected bool conditionIsMet = false;
    public bool ConditionMet
    {
        get { return conditionIsMet; }
        set { conditionIsMet = value; }
    }

    protected void OnEnable()
    {
        if (!trigger) trigger = GetComponent<Trigger>();
        InitCondition();
    }

    private void Update()
    {
        //hideFlags = trigger == null ? 
        hideFlags = HideFlags.None; //: HidFlags.HideInInspector;
    }

    protected void OnDisable()
    {
        StopAllCoroutines();
    }

    public virtual void InitCondition() { }
    public virtual void ResetCondition() { conditionIsMet = false; }
    public virtual bool CheckCondition() { return false; }
}