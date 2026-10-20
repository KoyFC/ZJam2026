using System;
using UnityEngine;

public class HealthController : MonoBehaviour, IDamageable
{
    public float CurrentHealth => _currentHealth;

    public float MaxHealth => _maxHealth;

    public bool IsAlive => _currentHealth > 0;

    // Health never drops below this, used to keep the player alive during the tutorial
    public float MinHealth { get; set; }

    public event Action<float> OnDamaged;
    public event Action OnDestroyed;
    public event Action<float> OnHealed;

    public static event Action<HealthController, float> OnAnyDamaged;
    public static event Action<HealthController> OnAnyDestroyed;

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

        float appliedDamage = Mathf.Min(damage, _currentHealth - MinHealth);
        _currentHealth = Mathf.Max(_currentHealth - damage, MinHealth);
        OnDamaged?.Invoke(damage);
        OnAnyDamaged?.Invoke(this, appliedDamage);
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            OnDestroyed?.Invoke();
            OnAnyDestroyed?.Invoke(this);
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
