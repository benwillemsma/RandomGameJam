using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        character.TakeDamage(final_value);
    }
}