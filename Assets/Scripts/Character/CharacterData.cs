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
    protected float m_deathDelay = 3f;

    protected bool m_isDead = false;
    
    protected float m_damageImmune;

    public float Health
    {
        get { return m_currentHealth; }
    }

    public virtual void TakeDamage(float damage)
    {
        if (m_damageImmune <= 0)
        {
            m_damageImmune = m_damageImmuneDuration;
            m_currentHealth -= damage;

            if (Health <= 0)
                RespawnCharacter();
        }
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
        if (m_damageImmune > 0)
            m_damageImmune -= Time.deltaTime;
    }

    protected virtual IEnumerator Die()
    {
        m_isDead = true;
        yield return new WaitForSeconds(m_deathDelay);
        Destroy(transform.gameObject);
        GameManager.Instance.Continue();
    }

    public void RespawnCharacter()
    {
        transform.position = SpawnPoint;
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
