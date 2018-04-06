using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState<Data> : CharacterState<Data> where Data : PlayerData
{
    public PlayerState(Data playerData) : base(playerData) { }

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
