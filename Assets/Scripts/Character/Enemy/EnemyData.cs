using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyData : HumanoidData
{
    #region EnemyData Variables

    private NavMeshAgent m_agent;
    public NavMeshAgent Agent
    {
        get { return m_agent; }
    }

    [Header("AI")]
    public bool useAI = true;
    public int patrolIndex = -1;
    public Transform[] patrolPoints;

    #endregion

    protected override void Awake()
    {
        base.Awake();
        m_agent = GetComponentInChildren<NavMeshAgent>();
        if (!m_agent) { Debug.Log("No NavMeshAgent found For:" + this, this); enabled = false; }
    }
}
