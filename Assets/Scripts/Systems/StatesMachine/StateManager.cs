using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    private BaseState m_state;
    public BaseState State
    {
        get { return m_state; }
        set { m_state = value; }
    }
    private bool m_isPaused;
    public bool IsPaused
    {
        get {return m_isPaused; }
        set { m_isPaused = value; }
    }

    [Space(10),SerializeField,Header("Debuging Info")]
    private string CurrentState;

    protected void Start()
    {
        //Initialize trigger Delegates
        SetTriggers(m_state);
        SetCollisions(m_state);

        m_state.EnterState(null);
        CurrentState = m_state.ToString();
    }

    protected void Update()
    {
        if (m_state != null) m_state.Update();
    }

    protected void FixedUpdate()
    {
        if (m_state != null) m_state.FixedUpdate();
    }

    //State Functions
    public void ChangeState(BaseState newState)
    {
        StartCoroutine(HandleStateTransition(newState));
        CurrentState = m_state.ToString();
    }
    protected IEnumerator HandleStateTransition(BaseState newState)
    {
        //Exit StateoverTime
        m_state.InTransition = true;
        yield return StartCoroutine(m_state.ExitState(newState));
        m_state.InTransition = false;

        RemoveTriggers(m_state);
        RemoveCollisions(m_state);

        //Set New State
        BaseState prevState = m_state;
        m_state = newState;

        //Enter State
        SetTriggers(newState);
        SetCollisions(newState);

        m_state.InTransition = true;
        yield return StartCoroutine(m_state.EnterState(prevState));
        m_state.InTransition = false;

        prevState = null;
    }

    // Trigger Delegates
    delegate void TriggerDelegate(Collider collider);
    private TriggerDelegate triggerEnter;
    private TriggerDelegate triggerStay;
    private TriggerDelegate triggerExit;
    delegate void CollisionDelegate(Collision collision);
    private CollisionDelegate collisionEnter;
    private CollisionDelegate collisionStay;
    private CollisionDelegate collisionExit;
    
    private void RemoveTriggers(BaseState state)
    {
        triggerEnter -= state.OnTriggerEnter;
        triggerStay -= state.OnTriggerStay;
        triggerExit -= state.OnTriggerExit;
    }
    private void SetTriggers(BaseState state)
    {
        triggerEnter += state.OnTriggerEnter;
        triggerStay += state.OnTriggerStay;
        triggerExit += state.OnTriggerExit;
    }

    private void RemoveCollisions(BaseState state)
    {
        collisionEnter -= state.OnCollisionEnter;
        collisionStay -= state.OnCollisionStay;
        collisionExit -= state.OnCollisionExit;
    }
    private void SetCollisions(BaseState state)
    {
        collisionEnter += state.OnCollisionEnter;
        collisionStay += state.OnCollisionStay;
        collisionExit += state.OnCollisionExit;
    }

    // Unity Trigger Functions Call Current State Tirgger Functions
    private void OnTriggerEnter(Collider other)
    {
        triggerEnter.Invoke(other);
    }
    private void OnTriggerStay(Collider other)
    {
        triggerStay.Invoke(other);
    }
    private void OnTriggerExit(Collider other)
    {
        triggerExit.Invoke(other);
    }

    // Unity Collision Functions Call Current State Collision Functions
    private void OnCollisionEnter(Collision collision)
    {
        collisionExit.Invoke(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        collisionStay.Invoke(collision);
    }
    private void OnCollisionExit(Collision collision)
    {
        collisionExit.Invoke(collision);
    }
}