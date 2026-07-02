using System;
using System.Linq;
using UnityEngine;

public class SkillComponent : MonoBehaviour
{
    private int _maxEnergy;
    private int _currentEnergy;
    private SkillInstance[] _usableSkills;
    public SkillInstance[] CharacterSkill => _usableSkills;

    public int CurrentEnergy => _currentEnergy;
    public int MaxEnergy => _maxEnergy;

    public event Action<SkillInstance> OnSkillUsedSuccess;
    public event Action<string> OnSkillUsedFailed;
    public event Action<int> OnEnergyIncreased;

    public void AddEnergy(int value)
    {
        _currentEnergy = Mathf.Clamp(_currentEnergy + value, 0, _maxEnergy);
        OnEnergyIncreased?.Invoke(value);
    }

    public void UseSkill(SkillInstance useSkill, Character caster, Character target)
    {
        Debug.Log("Skill PRessed");
        if (useSkill.Data.manaCost > _currentEnergy)
        {
            OnSkillUsedFailed?.Invoke("Not Enough Mana");
            return;
        }

        if (useSkill == null || !useSkill.IsReady())
        {
            OnSkillUsedFailed?.Invoke("Skill on cooldown");
            return;
        }
        _currentEnergy -= useSkill.Data.manaCost;

        foreach (var effect in useSkill.Data.effects)
            effect.Apply(caster, target);

        useSkill.remainingCooldown = useSkill.Data.cooldown;
        OnSkillUsedSuccess?.Invoke(useSkill);
    }

    public void Initialize(int maxEnergy, SkillItem[] selectedSkills)
    {
        _maxEnergy = maxEnergy;
        _currentEnergy = _maxEnergy;
        _usableSkills = selectedSkills
            .Select(skill => new SkillInstance { Data = skill, remainingCooldown = 1 })
            .ToArray();
    }

    public void TickCooldowns()
    {
        foreach (var skill in _usableSkills)
            skill.Tick();
    }

}