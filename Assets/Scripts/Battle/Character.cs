using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Data")]
    private CharacterData _characterData;

    [Header("Components")]
    private AimController _aimController;
    private Stick _stick;
    private HealthComponent _healthComponent;
    private Shockwave _characterHand;
    private SkillComponent _skillComponent;
    private string _ownerTag;

    public AimController AimController => _aimController;
    public Stick Stick => _stick;
    public HealthComponent HealthComponent => _healthComponent;
    public SkillComponent SkillComponent => _skillComponent;
    public Shockwave CharacterHand => _characterHand;

    private Vector3 _originalCharacterPosition;
    private Quaternion _originalCharacterRotation;

    private List<StatusEffect> _activeStatus = new List<StatusEffect>();
    public List<StatusEffect> StatusEffect => _activeStatus;

    public bool IsSkipped;
    public float CharacterPower;

    void Awake()
    {
        _aimController = GetComponent<AimController>();
        _stick = GetComponent<Stick>();
        _healthComponent = GetComponent<HealthComponent>();
        _skillComponent = GetComponent<SkillComponent>();
    }

    void Start()
    {
        CharacterPower = _characterData.CharacterSmashPower;
    }

    public void SetupComponents(CharacterData characterData, Character opponentCharacter, Shockwave characterHand)
    {
        _ownerTag = gameObject.tag;
        _characterData = characterData;
        _characterHand = characterHand;

        if (_ownerTag == "Player")
        {
            _aimController.SetUseMouseInput(true);
        }
        else
        {
            _aimController.SetUseMouseInput(false);
        }
        _aimController.SetTargetIndicator(_characterHand.transform);
        _aimController.SetTargetCharacter(opponentCharacter.transform);
        _characterHand.SetTargetIndicator(transform);

        _skillComponent.Initialize(_characterData.BattleInventory.selectedSkills);

        _originalCharacterPosition = transform.position;
        _originalCharacterRotation = transform.rotation;
    }

    public void ResetPosition()
    {
        transform.SetPositionAndRotation(_originalCharacterPosition, _originalCharacterRotation);
    }

    public void UseSkill(SkillInstance skill, Character caster, Character target)
    {
        _skillComponent.UseSkill(skill, caster, target);
    }

    public void UseItem(BaseItem item, Character caster, Character target)
    {
        UsableItem usableItem = (UsableItem) item;
        if (InventoryManager.Instance.UseItem(usableItem))
        {
            foreach (var effect in usableItem.effects)
            {
                effect.Apply(caster, target);
            }
        }

    }

    public void AddStatus(StatusEffect statusEffect)
    {
        _activeStatus.Add(statusEffect);

        if (statusEffect == global::StatusEffect.Stunned)
        {
            IsSkipped = true;
        }
    }

    public void ClearStatus()
    {
        _activeStatus.Clear();
        IsSkipped = false;
        CharacterPower = _characterData.CharacterSmashPower;
    }

    public void ApplyModifier(StatModifier mod)
    {
        CharacterPower *= mod.modifier;
    }

    

}
