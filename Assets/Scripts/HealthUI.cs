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

    /// <summary>
    /// Set the current HP and update the UI accordingly
    /// </summary>
    public void SetHP(int hp)
    {
        currentHP = Mathf.Clamp(hp, 0, maxHP);
        UpdateHealthUI();
    }

    /// <summary>
    /// Take damage and reduce HP
    /// </summary>
    public void TakeDamage(int damage)
    {
        SetHP(currentHP - damage);
    }

    /// <summary>
    /// Heal and increase HP
    /// </summary>
    public void Heal(int amount)
    {
        SetHP(currentHP + amount);
    }

    /// <summary>
    /// Update the health UI by deactivating indicators when HP is lost
    /// </summary>
    private void UpdateHealthUI()
    {
        for (int i = 0; i < healthIndicators.Count; i++)
        {
            // Deactivate indicators beyond current HP
            healthIndicators[i].SetActive(i < currentHP);
        }
    }

    /// <summary>
    /// Get the current HP
    /// </summary>
    public int GetCurrentHP()
    {
        return currentHP;
    }

    /// <summary>
    /// Get the max HP
    /// </summary>
    public int GetMaxHP()
    {
        return maxHP;
    }

    /// <summary>
    /// Check if the player is dead
    /// </summary>
    public bool IsDead()
    {
        return currentHP <= 0;
    }
}
