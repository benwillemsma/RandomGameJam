using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(StateManager))]
[RequireComponent(typeof(AudioSource))]
public abstract class CharacterData : MonoBehaviour
{
    #region Components
    protected StateManager m_stateM;
    public StateManager StateM
    {
        get { return m_stateM; }
    }
    protected Rigidbody m_rb;
    public Rigidbody RB
    {
        get { return m_rb; }
    }
    protected Animator m_anim;
    public Animator Anim
    {
        get { return m_anim; }
    }
    #endregion

    #region Health Info
    [Space(10)]
    public Transform spawn;
    protected Vector3 m_startLocation;
    public Vector3 SpawnPoint
    {
        get
        {
            if (spawn) return spawn.position;
            else return m_startLocation;
        }
    }

    [SerializeField]
    protected float m_maxHealth = 100;
    [SerializeField]
    protected float m_currentHealth;
    [SerializeField]
    protected float m_damageImmuneDuration = 0.5f;

    [SerializeField]
    protected float m_healthRegen = 5;
    [SerializeField]
    protected float m_healthRegenDelay = 1;
    protected float m_healthRegenTimer = 0;

    [SerializeField]
    protected float m_deathDelay = 5f;

    protected bool m_isDead = false;
    public bool IsDead { get { return m_isDead; } }
    
    protected float m_damageImmune;

    public float Health
    {
        get { return m_currentHealth; }
    }

    public virtual void TakeDamage(float damage)
    {
        if (m_damageImmune <= 0)
        {
            m_healthRegenTimer = 0;
            m_damageImmune = m_damageImmuneDuration;
            m_currentHealth -= damage;

            if (m_currentHealth <= 0)
                StartCoroutine(Die());
        }
    }

    public virtual void Heal(float health)
    {
        m_currentHealth += health;
        m_currentHealth = Mathf.Min(m_currentHealth, m_maxHealth);
    }

    protected virtual void SetUndamageable(float duration = Mathf.Infinity)
    {
        m_damageImmuneDuration = duration;
    }

    protected virtual void SetDamageable()
    {
        m_damageImmuneDuration = -1;
    }

    protected virtual void UpdateHealthStatus()
    {
        m_healthRegenTimer += Time.deltaTime;
        if (m_healthRegenTimer > m_healthRegenDelay)
            Heal(m_healthRegen * Time.deltaTime);
        if (m_damageImmune > 0)
            m_damageImmune -= Time.deltaTime;
    }

    protected virtual IEnumerator Die()
    {
        m_isDead = true;
        if(m_rb) m_rb.constraints = RigidbodyConstraints.None;

        yield return new WaitForSeconds(m_deathDelay);

        if (spawn) Respawn();
        else Destroy(transform.gameObject);
    }

    public virtual void Respawn()
    {
        m_isDead = false;
        if (m_rb) m_rb.constraints = RigidbodyConstraints.FreezeRotation;
        if (spawn)
        {
            transform.position = spawn.position;
            transform.rotation = spawn.rotation;
        }
        else transform.position = SpawnPoint;

        RB.velocity = Vector3.zero;
        m_currentHealth = m_maxHealth;
    }
    #endregion

    #region Audio
    protected AudioSource m_audioSource;

    public void PlaySound(string soundName)
    {
        GameManager.AudioManager.playAudio(this, soundName);
    }

    public AudioSource SFXSource
    {
        get { return m_audioSource; }
        set { m_audioSource = value; }
    }
    #endregion

    #region Main
    protected virtual void Awake()
    {
        m_stateM = GetComponent<StateManager>();
        m_audioSource = GetComponent<AudioSource>();
        m_rb = GetComponentInChildren<Rigidbody>();
        if (!m_rb) { Debug.Log("No Rigidbody found For:" + this, this); enabled = false; }
        m_anim = GetComponentInChildren<Animator>();
        if (!m_anim) { Debug.Log("No Animator found For:" + this, this); enabled = false; }

m_currentHealth = m_maxHealth;
        m_startLocation = transform.position;
    }

    protected virtual void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    protected virtual void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    protected virtual void Update()
    {
        UpdateHealthStatus();
    }

    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode) { }
    #endregion
}
