using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IKController))]
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
        IK = GetComponent<IKController>();
    }
    #endregion
}