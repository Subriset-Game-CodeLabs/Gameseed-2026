using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Effects/IncreaseEnergy")]
public class IncreaseEnergy : EffectSO
{
    public int value;
    public override void Apply(Character caster, Character target)
    {
        caster.SkillComponent.AddEnergy(value);
    }
}
