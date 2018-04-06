using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerData : CharacterData
{
    #region Variables

    #endregion

    #region Trigger Input
    private InputTrigger rightTriggerState = new InputTrigger("RightTrigger", TriggerType.Trigger);
    private InputTrigger leftTriggerState = new InputTrigger("LeftTrigger", TriggerType.Trigger);

    public InputTrigger RightTrigger
    {
        get { return rightTriggerState; }
    }
    public InputTrigger LeftTrigger
    {
        get { return leftTriggerState; }
    }
    #endregion

    #region Main
    protected override void Awake()
    {
        base.Awake();
        if (!GameManager.player)
        {
            GameManager.player = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            StopAllCoroutines();
            DestroyImmediate(gameObject);
        }
    }

    private void Start ()
    {
        m_stateM.State = new PlayerWalkingState(this);
        StartCoroutine(m_stateM.State.EnterState(null));
    }

   

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }
    #endregion
}