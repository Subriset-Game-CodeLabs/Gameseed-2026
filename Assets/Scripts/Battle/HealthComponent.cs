using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    private int _maxHP;
    private int _currentHP;
    private bool isAlive = true;

    public int CurrentHP => _currentHP;
    public int MaxHP => _maxHP;

    public event Action OnHealthChanged;

    public void Initialize(int maxHP)
    {
        _maxHP = maxHP;
        _currentHP = _maxHP;
        isAlive = true;
        OnHealthChanged?.Invoke();
    }

    public void TakeDamage(int damage)
    {
        _currentHP -= damage;
        OnHealthChanged?.Invoke();
        Debug.Log(transform.name + " Take " + damage + " Damage");
        if (_currentHP <= 0)
        {
            isAlive = false;
            Debug.Log(transform.name + " Is Dead");
        }
    }

    public bool IsAlive() => isAlive;

}
