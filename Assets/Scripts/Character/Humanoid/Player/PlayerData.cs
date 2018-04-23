using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerData : HumanoidData
{
    #region Variables

    [Header("Movement")]
    [Range(0, 10)]
    public float runSpeed = 8;
    [Range(4, 20)]
    public float jumpForce = 6;
    [Range(0, 10)]
    public float airControl = 5;
    public bool toggleCrouch;

    [Header("Stealth")]
    public float soundLevel;
    public float lightLevel;
    public const float MaxDetection = 15;
    public float TotalDetection(Vector3 atPosition)
    {
        if (this)
        {
            float distance = (transform.position - atPosition).magnitude;
            return soundLevel / distance + lightLevel;
        }
        else return 0;
    }

    #region Stamina

    [Header("Stamina")]
    [Range(0, 100), SerializeField]
    protected float m_maxStamina = 100;
    [Range(0, 100), SerializeField]
    protected float m_currentStamina = 100;
    [Range(0, 100), SerializeField]
    protected float m_staminaRegen = 5;
    [Range(0, 100), SerializeField]
    protected float m_staminaRegenDelay = 1;
    protected float m_staminaRegenTimer = 0;

    public float Stamina
    {
        get { return m_currentStamina; }
    }

    public virtual void UseStamina(float amount)
    {
        m_currentStamina -= amount;
        m_staminaRegenTimer = 0;
    }

    public virtual void RegenerateStamina(float amount)
    {
        m_currentStamina += amount;
        m_currentStamina = Mathf.Min(m_currentStamina, m_maxStamina);
    }

    protected virtual void UpdateStaminaStatus()
    {
        m_staminaRegenTimer += Time.deltaTime;
        if (m_staminaRegenTimer > m_staminaRegenDelay)
            RegenerateStamina(m_staminaRegen * Time.deltaTime);
    }

    #endregion

    [Header("LayerMasks")]
    public LayerMask groundMask;
    public LayerMask climbingMask;

    [Header("BeamAttack")]
    [Range(0, 100), SerializeField]
    protected float lightPerSecond = 5;
    [Range(0, 100), SerializeField]
    protected float m_charge = 0;
    public float LightCharge
    {
        get { return m_charge; }
    }

    [Header("Camera")]
    [Range(0, 50)]
    public float CameraSensitivity = 30;
    [Range(0, 2)]
    public float CameraOffset = 0.75f;
    public bool InvertedCamera;

    [Header("HUD")]
    [SerializeField]
    protected Slider healthBar;
    [SerializeField]
    protected Slider staminaBar;
    [SerializeField]
    protected Slider chargeBar;

    private Image healthImage;

    public override void Respawn()
    {
        base.Respawn();
        m_currentStamina = m_maxStamina;
    }

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
            GameManager.player = this;
        else
        {
            StopAllCoroutines();
            DestroyImmediate(gameObject);
        }
        if (healthBar) healthImage = healthBar.fillRect.GetComponent<Image>();
    }

    protected override void Update()
    {
        base.Update();
        UpdateStaminaStatus();
        UpdateHud();
    }

    public void UpdateHud()
    {
        if (healthBar)
        {
            healthBar.value = m_currentHealth;
            healthImage.color = Color.Lerp(Color.red, Color.green, m_currentHealth / 100);
        }
        if (staminaBar) staminaBar.value = m_currentStamina;
        if (chargeBar) chargeBar.value = m_charge;
    }

    private void FixedUpdate()
    {
        if (LightChecker.instance) lightLevel = LightChecker.instance.GetLIghtValue(transform.position + transform.up);
    }

    private void Start ()
    {
        m_stateM.State = new PlayerWalkingState(this);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "LightArea")
            m_charge = Mathf.Clamp(m_charge + (Time.deltaTime * lightPerSecond), 0, 100);
    }
    #endregion
}