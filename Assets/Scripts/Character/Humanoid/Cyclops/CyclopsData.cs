using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CyclopsData : EnemyData
{
    #region CyclopsIdleState Variables

    [Header("CyclopsData")]
    public float Attack1_Damage;
    public float Attack1_Duration;
    
    public float Attack2_Damage;
    public float Attack2_Duration;

    #endregion

    private void Start()
    {
        m_stateM.State = new CyclopsIdleState(this);
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }
}
