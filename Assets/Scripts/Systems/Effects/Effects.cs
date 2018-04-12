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

public class DamageEffect : Effect
{
    public override void OnEffectStart(CharacterData character)
    {
        character.TakeDamage(initial_value);
    }
    public override void OnEffectStay(CharacterData character)
    {
        character.TakeDamage(overTime_value);
    }
    public override void OnEffectEnd(CharacterData character)
    {
        character.TakeDamage(overTime_value);
    }
}

public class KnockBackEffect : Effect
{
    public override void OnEffectStart(CharacterData character)
    {

    }
    public override void OnEffectStay(CharacterData character)
    {

    }
    public override void OnEffectEnd(CharacterData character)
    {

    }
}
