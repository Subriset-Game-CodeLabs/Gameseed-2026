using System;
using System.Linq;
using UnityEngine;

public class SkillComponent : MonoBehaviour
{
    [SerializeField]
    private int _maxEnergy;
    private int _currentEnergy;
    private SkillInstance[] _usableSkills;
    public SkillInstance[] CharacterSkill => _usableSkills;

    public event Action<SkillInstance> OnSkillUsedSuccess;
    public event Action<string> OnSkillUsedFailed;
    public event Action<int> OnEnergyIncreased;

    void Start()
    {
        _currentEnergy = _maxEnergy;
    }

    public void AddEnergy(int value)
    {
        _currentEnergy = Mathf.Clamp(value, 0, _maxEnergy);
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

    public void Initialize(SkillItem[] selectedSkills)
    {
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