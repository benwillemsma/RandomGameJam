using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CyclopsData : EnemyData
{
    #region CyclopsIdleState Variables

    [Header("Vision Data")]
    public Transform eye;
    [Range(1,90)]
    public float visionAngle;
    public LayerMask VisionMask;

    [Header("Detection Data")]
    public float detectionLevel;

    [Range(0, PlayerData.MaxDetection)]
    public float walkingThreshold;
    [Range(1, PlayerData.MaxDetection)]
    public float searchThreshold;
    [Range(2, PlayerData.MaxDetection)]
    public float attackThreshold;

    [Header("Attack")]
    public EffectManager rightAxeAttack;
    public EffectManager leftAxeAttack;

    [Header("Particles")]
    public ParticleSystem Fire;
    public ParticleSystem EyeBeam;
    public ParticleSystem GroundPound;
    private bool onFire;

    #endregion

    private void Start()
    {
        m_stateM.State = new CyclopsWalkingState(this);
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.P))
            OnFire(!onFire);
    }

    public void OnFire(bool toggle)
    {
        onFire = toggle;
        if (onFire)
            Fire.Play(true);
        else Fire.Stop(true);
    }
}
