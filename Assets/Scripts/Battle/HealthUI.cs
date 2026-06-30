using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> healthIndicators = new List<GameObject>();
    [SerializeField] private int currentHP;
    [SerializeField] private int maxHP = 5;

    private void Start()
    {
        // Initialize with all indicators active
        currentHP = maxHP;
        UpdateHealthUI();
    }

    public void SetHP(int hp)
    {
        currentHP = Mathf.Clamp(hp, 0, maxHP);
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        SetHP(currentHP - damage);
    }

    public void Heal(int amount)
    {
        SetHP(currentHP + amount);
    }

    private void UpdateHealthUI()
    {
        for (int i = 0; i < healthIndicators.Count; i++)
        {
            // Deactivate indicators beyond current HP
            healthIndicators[i].SetActive(i < currentHP);
        }
    }

}
