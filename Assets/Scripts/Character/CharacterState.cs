using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterState<Data> : BaseState where Data : CharacterData
{
    protected Data data;
    protected Rigidbody rb;
    protected Animator anim;

    public CharacterState(Data characterData) : base(characterData.StateM)
    {
        data = characterData;
        rb = characterData.RB;
        anim = characterData.Anim;
    }

    //State Updates
    protected override void UpdateState()
    {
        UpdateMovement();
        if (anim) UpdateAnimator();
    }

    /// /// <summary>
    /// Character MovementUpdate, first update to be Called in StateUpdate
    /// </summary>
    protected virtual void UpdateMovement() { }

    /// <summary>
    /// Character AnimatorUpdate, Called after MovementUpdate if Animator was found.
    /// </summary>
    protected virtual void UpdateAnimator() { }
}
