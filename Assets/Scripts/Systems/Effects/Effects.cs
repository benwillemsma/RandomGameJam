using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    public float initial_value;
    public float final_value;
    public float overTime_value;

    public abstract void OnEffectStart(CharacterData character);
    public abstract void OnEffectStay(CharacterData character);
    public abstract void OnEffectEnd(CharacterData character);
}
