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
        if (!IsAlive) return;

        OnHealed?.Invoke(amount);
        _currentHealth += amount;
        if (_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
    }

    public void ReceiveDamage(float damage)
    {
        if (!IsAlive) return;

        _currentHealth -= damage;
        OnDamaged?.Invoke(damage);
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            OnDestroyed?.Invoke();
        }
    }

    [Header("Health Settings")]
    [SerializeField]
    private float _currentHealth;

    [SerializeField]
    private float _maxHealth = 100f;

    private void OnEnable()
    {
        _currentHealth = _maxHealth;
    }
}
