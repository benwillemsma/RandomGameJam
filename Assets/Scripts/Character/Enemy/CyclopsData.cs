using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CyclopsData : HumanoidData
{
    #region CyclopsIdleState Variables

    [Header("Attack1")]
    public float Attack1_Damage;
    public float Attack1_Duration;

    [Header("Attack2")]
    public float Attack2_Damage;
    public float Attack2_Duration;

    public Transform[] patrolPoints;

    #endregion

    private void Start()
    {
        m_stateM.State = new CyclopsIdleState(this);
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }
}
