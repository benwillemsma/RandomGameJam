using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : HumanoidState<PlayerData>
{
    #region PlayerState Variables

    protected bool canInput = true;
    protected Vector3 movementDirection;
    protected Quaternion properRotation;

    #endregion

    public PlayerState(PlayerData playerData) : base(playerData) { }

    protected override void UpdateState()
    {
        properRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(rb.transform.forward, Vector3.up), Vector3.up);
        base.UpdateState();
        if (canInput) UpdateInput();

        Vector3 velocityLevel = rb.velocity;
        velocityLevel.y = 0;
        data.detectionLevel = velocityLevel.magnitude;
    }

    /// <summary>
    /// Player InputUpdate, Called Before StateUpdate;
    /// </summary>
    protected abstract void UpdateInput();
}
