using UnityEngine;

public abstract class EffectSO : ScriptableObject {
    public abstract void Apply(Character caster, Character target);
}

public enum SkillTarget
{
    Enemy,
    Self,
    Both
}

public enum StatusEffect
{
    Stunned
}