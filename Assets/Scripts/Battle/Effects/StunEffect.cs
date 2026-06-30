using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Effects/Stun")]
public class StunEffect : EffectSO
{
    public override void Apply(Character caster, Character target)
    {
        target.AddStatus(StatusEffect.Stunned);
    }
}
