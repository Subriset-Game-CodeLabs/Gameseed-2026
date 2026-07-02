using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Effects/StatModifier")]
public class StatModifier : EffectSO
{
    public float modifier;
    public bool selfTarget;

    public override void Apply(Character caster, Character target)
    {
        if (selfTarget)
        {
            caster.ApplyModifier(this);
        }else{
            target.ApplyModifier(this);
        }
    }
}
