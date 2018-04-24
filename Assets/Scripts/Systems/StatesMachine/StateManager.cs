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
        if (m_state != null)
        {
            SetTriggers(m_state);
            SetCollisions(m_state);

            StartCoroutine(m_state.EnterState(null));
            CurrentState = m_state.ToString();
        }
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
        if (!m_state.InTransition)
        {
            m_state.InTransition = true;
            newState.InTransition = true;
            StartCoroutine(HandleStateTransition(newState));
        }
    }
    protected IEnumerator HandleStateTransition(BaseState newState)
    {
        //Exit StateoverTime
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
        
        yield return StartCoroutine(m_state.EnterState(prevState));
        m_state.InTransition = false;

        prevState = null;
        CurrentState = m_state.ToString();
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
        if (state != null)
        {
            triggerEnter -= state.OnTriggerEnter;
            triggerStay -= state.OnTriggerStay;
            triggerExit -= state.OnTriggerExit;
        }
    }
    private void SetTriggers(BaseState state)
    {
        if (state != null)
        {
            triggerEnter += state.OnTriggerEnter;
            triggerStay += state.OnTriggerStay;
            triggerExit += state.OnTriggerExit;
        }
    }

    private void RemoveCollisions(BaseState state)
    {
        if (state != null)
        {
            collisionEnter -= state.OnCollisionEnter;
            collisionStay -= state.OnCollisionStay;
            collisionExit -= state.OnCollisionExit;
        }
    }
    private void SetCollisions(BaseState state)
    {
        if (state != null)
        {
            collisionEnter += state.OnCollisionEnter;
            collisionStay += state.OnCollisionStay;
            collisionExit += state.OnCollisionExit;
        }
    }

    // Unity Trigger Functions Call Current State Tirgger Functions
    private void OnTriggerEnter(Collider other)
    {
        if (m_state != null) triggerEnter.Invoke(other);
    }
    private void OnTriggerStay(Collider other)
    {
        if (m_state != null) triggerStay.Invoke(other);
    }
    private void OnTriggerExit(Collider other)
    {
        if (m_state != null) triggerExit.Invoke(other);
    }

    // Unity Collision Functions Call Current State Collision Functions
    private void OnCollisionEnter(Collision collision)
    {
        if (m_state != null) collisionExit.Invoke(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        if (m_state != null) collisionStay.Invoke(collision);
    }
    private void OnCollisionExit(Collision collision)
    {
        if (m_state != null) collisionExit.Invoke(collision);
    }
}