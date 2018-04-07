using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : HumanoidState<PlayerData>
{
    #region PlayerState Variables
    
    protected bool canInput = true;
    protected Vector3 movementDirection;

    #endregion

    public PlayerState(PlayerData playerData) : base(playerData) { }

    protected override void UpdateState()
    {
        base.UpdateState();
        if (canInput) UpdateInput();
    }

    /// <summary>
    /// Player InputUpdate, Called Before StateUpdate;
    /// </summary>
    protected abstract void UpdateInput();
}
