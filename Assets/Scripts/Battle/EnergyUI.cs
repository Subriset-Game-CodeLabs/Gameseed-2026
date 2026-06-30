using System.Collections.Generic;
using UnityEngine;

public class EnergyUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> _energyIndicators = new List<GameObject>();
    [SerializeField] private int _currentEnergy;
    [SerializeField] private int _maxEnergy = 5;

    private void Start()
    {
        _currentEnergy = _maxEnergy;
        UpdateEnergyUI();
    }

    public void SetEnergy(int energy)
    {
        _currentEnergy = Mathf.Clamp(energy, 0, _maxEnergy);
        UpdateEnergyUI();
    }

    public void UseEnergy(int amount)
    {
        SetEnergy(_currentEnergy - amount);
    }

    public void RecoverEnergy(int amount)
    {
        SetEnergy(_currentEnergy + amount);
    }

    private void UpdateEnergyUI()
    {
        for (int i = 0; i < _energyIndicators.Count; i++)
        {
            _energyIndicators[i].SetActive(i < _currentEnergy);
        }
    }
}
