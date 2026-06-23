using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField]
    private int _maxHP;
    private int _currentHP;
    private bool isAlive = true;

    void Start()
    {
        _currentHP = _maxHP;
    }

    public void TakeDamage(int damage)
    {
        _currentHP -= damage;
        Debug.Log(transform.name + " Take " + damage + " Damage");
        if (_currentHP <= 0)
        {
            isAlive = false;
            Debug.Log(transform.name + " Is Dead");
        }
    }

    public bool IsAlive() => isAlive;

}
