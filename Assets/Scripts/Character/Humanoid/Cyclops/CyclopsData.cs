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

    #endregion

    private void Start()
    {
        m_stateM.State = new CyclopsWalkingState(this);
    }
}
