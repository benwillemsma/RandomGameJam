using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField]
    protected bool checkOnPlay = true;
    public bool CheckOnPlay
    {
        get { return checkOnPlay; }
        set { checkOnPlay = value; }
    }

    [SerializeField]
    protected bool repeat = true;
    public bool Repeat
    {
        get { return repeat; }
        set { repeat = value; }
    }

    [SerializeField]
    protected bool inOrder = false;
    public bool InOrder
    {
        get { return inOrder; }
        set { inOrder = value; }
    }

    [SerializeField, HideInInspector]
    protected List<Condition> conditions = new List<Condition>();
    public List<Condition> Conditions
    {
        get { return conditions; }
        set { conditions = value; }
    }

    [SerializeField, HideInInspector]
    protected UnityEvent result;
    public UnityEvent Result
    {
        get { return result; }
        set { result = value; }
    }

    private bool conditionsMet;
    public bool ConditionsMet
    {
        get { return conditionsMet; }
        set { conditionsMet = value; }
    }

    protected bool check = false;
    public void Reset()
    {
        check = true;
    }

    protected virtual void OnValidate()
    {
        for (int i = 0; i < conditions.Count; i++)
        {
            if(conditions[i])
            conditions[i].Trigger = this;
        }
    }

    protected virtual void OnEnable()
    {
        if (checkOnPlay)
            check = true;
    }
    protected virtual void OnDisable()
    {
        StopAllCoroutines();
        check = false;
    }

    protected virtual void Update()
    {
        if (check) conditionsMet = CheckConditions();
    }

    public virtual bool CheckConditions()
    {
        if (inOrder)
        {
            for (int i = 0; i < conditions.Count; i++)
            {
                if (conditions[i].CheckCondition() == false)
                {
                    ResetConditions(i + 1); 
                    return false;
                }
            }
        }
        else
        {
            int condtionsMet = 0;
            for (int i = 0; i < conditions.Count; i++)
                condtionsMet += conditions[i].CheckCondition() ? 1 : 0;

            if (condtionsMet != conditions.Count)
                return false;
        }

        result.Invoke();
        Debug.Log(this + ":Triggered", this);

        if (!repeat)
        {
            check = false;
            StopAllCoroutines();
            ResetConditions(0);
        }
        return true;
    }

    private void ResetConditions(int startAtIndex)
    {
        for (int i = startAtIndex; i < conditions.Count; i++)
            conditions[i].ResetCondition();
    }
}