using System.Collections.Generic;
using UnityEngine;

public class EnergyUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> _energyIndicators = new List<GameObject>();

    private SkillComponent _skillComponent;

    public void Initialize(SkillComponent skillComponent)
    {
        _skillComponent = skillComponent;
        _skillComponent.OnEnergyIncreased += OnEnergyChanged;
        _skillComponent.OnSkillUsedSuccess += OnSkillUsed;
        UpdateEnergyUI();
    }

    private void OnEnergyChanged(int value)
    {
        UpdateEnergyUI();
    }

    private void OnSkillUsed(SkillInstance skill)
    {
        UpdateEnergyUI();
    }

    private void OnDestroy()
    {
        if (_skillComponent != null)
        {
            _skillComponent.OnEnergyIncreased -= OnEnergyChanged;
            _skillComponent.OnSkillUsedSuccess -= OnSkillUsed;
        }
    }

    private void UpdateEnergyUI()
    {
        for (int i = 0; i < _energyIndicators.Count; i++)
        {
            _energyIndicators[i].SetActive(i < _skillComponent.CurrentEnergy);
        }
    }
}
