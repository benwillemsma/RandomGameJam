using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : HumanoidState<PlayerData>
{
    #region PlayerState Variables

    public float speed;

    protected Vector3 movementDirection;

    #endregion

    public PlayerState(PlayerData playerData) : base(playerData) { }

    protected override void UpdateState()
    {
        UpdateInput();
        base.UpdateState();
    }

    /// <summary>
    /// Character InputUpdate, Called Before StateUpdate;
    /// </summary>
    protected abstract void UpdateInput();
}
