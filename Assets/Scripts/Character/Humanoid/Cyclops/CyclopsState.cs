using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CyclopsState : EnemyState<CyclopsData>
{
    #region CyclopsState Variables

    #endregion

    public CyclopsState(CyclopsData cyclops) : base(cyclops) { }
}
