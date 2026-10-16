using System;
using UnityEngine;

public class HealthController : MonoBehaviour, IDamageable
{
    public float CurrentHealth => _currentHealth;

    public float MaxHealth => _maxHealth;

    public bool IsAlive => _currentHealth > 0;

    public event Action<float> OnDamaged;
    public event Action OnDestroyed;
    public event Action<float> OnHealed;

    public void Heal(float amount)
    {
        OnHealed?.Invoke(amount);
        _currentHealth += amount;
        if (_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
    }

    public void ReceiveDamage(float damage)
    {
        _currentHealth -= damage;
        OnDamaged?.Invoke(damage);
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            OnDestroyed?.Invoke();
        }
    }


    private float _currentHealth;
    private float _maxHealth = 100f;

    void Awake()
    {
        _currentHealth = _maxHealth;
    }
}
