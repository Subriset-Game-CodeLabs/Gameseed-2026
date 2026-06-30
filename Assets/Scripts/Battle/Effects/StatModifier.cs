using UnityEngine;

public class StatModifier : EffectSO
{
    public float modifier;

    public override void Apply(Character caster, Character target)
    {
        target.ApplyModifier(this);
    }
}
