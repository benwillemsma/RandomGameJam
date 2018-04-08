using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerData : HumanoidData
{
    #region Variables
    public float lightPerSecond;
    public float lightLevel;

    [Header("Movement")]
    [Range(0, 10)]
    public float runSpeed = 8;
    [Range(4, 20)]
    public float jumpForce = 6;
    [Range(0, 10)]
    public float airControl = 5;
    public bool toggleCrouch;
    public LayerMask groundMask;

    [Header("Camera")]
    [Range(0, 50)]
    public float CameraSensitivity = 30;
    [Range(0, 2)]
    public float CameraOffset = 0.75f;
    public bool InvertedCamera;

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
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "LightArea")
            lightLevel = Mathf.Clamp(lightLevel + (Time.deltaTime * lightPerSecond), 0, 100);
    }
    #endregion
}