using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CyclopsBaseState<Data> : CharacterState<Data> where Data: CyclopsData
{
    #region EnemyState Variables

    #endregion

    public CyclopsBaseState(Data characterData) : base(characterData) { }
}
