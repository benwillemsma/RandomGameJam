using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidData : CharacterData
{
    #region Variables
    protected IKController m_IK;
    public IKController IK
    {
        get { return m_IK; }
        set { m_IK = value; }
    }
    #endregion

    #region Main
    protected override void Awake()
    {
        base.Awake();
        IK = GetComponentInChildren<IKController>();
        if (!IK) { Debug.Log("No IKController found For:" + this, this); enabled = false; }
    }
    #endregion
}