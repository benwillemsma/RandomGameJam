using System.Collections;
using UnityEngine;

public abstract class BaseState
{
    protected StateManager stateManager;

    /// <summary>
    /// True Once EnterState Coroutine Completes.
    /// </summary>
    protected bool hasStarted = false;
    protected bool hasStopped = false;

    protected float elapsedTime;
    protected bool inTransition = false;
    public bool InTransition
    {
        get {return inTransition; }
        set {inTransition = value; }
    }

    //Constructor
    public BaseState(StateManager stateM)
    {
        stateManager = stateM;
        elapsedTime = 0;
    }

    //Transition Functions
    /// <summary>
    /// Called when the State is Entered before first StateUpdate, This is the Current State.
    /// </summary>
    /// <param name="prevState"></param>
    /// <returns></returns>
    public virtual IEnumerator EnterState(BaseState prevState)
    {
        if (this != stateManager.State)
        {
            Debug.LogWarning(stateManager.gameObject + "has a RogueState: " + this + "\t\tCurrent State:" + stateManager.State);
            hasStopped = true;
        }
        else
        {
            hasStarted = true;
            yield return null;
        }
    }

    /// <summary>
    /// Called when the State is Exited after final StateUpdate, This is still the Current State.
    /// </summary>
    /// <param name="prevState"></param>
    /// <returns></returns>
    public virtual IEnumerator ExitState(BaseState nextState)
    {
        hasStopped = true;
        yield return null;
    }

    //State Updates
    public virtual void Update()
    {
        if (hasStarted && !hasStopped)
        {
            if (stateManager.IsPaused)
                UpdatePaused();

            else if (inTransition)
                UpdateTransition();

            else UpdateState();

            elapsedTime += Time.deltaTime;
        }
    }

    public virtual void FixedUpdate()
    {
        if (!stateManager.IsPaused)
            UpdatePhysics();
    }

    /// <summary>
    /// Physics Update, Called From StateManager FixedUpdate
    /// </summary>
    protected virtual void UpdatePhysics() { }

    /// <summary>
    /// Paused Update, Regular Update Called from StateManager Update
    /// </summary>
    protected virtual void UpdateState() { }

    /// <summary>
    /// Paused Update, Update Called when StateManager is Paused
    /// </summary>
    protected virtual void UpdatePaused() { }

    /// <summary>
    /// Paused Update, Update Called when StateManager is Transitioning
    /// </summary>
    protected virtual void UpdateTransition() { }

    //Trigger Functions
    public virtual void OnTriggerEnter(Collider collider) { }
    public virtual void OnTriggerStay(Collider collider) { }
    public virtual void OnTriggerExit(Collider collider) { }

    //Colission Functions
    public virtual void OnCollisionEnter(Collision collision) { }
    public virtual void OnCollisionStay(Collision collision) { }
    public virtual void OnCollisionExit(Collision collision) { }
}