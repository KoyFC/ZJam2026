
using System;

/// <summary>
/// Interface for objects that can take damage and be healed.
/// </summary>
public interface IDamageable
{
    // Properties
    float CurrentHealth { get; }
    float MaxHealth { get; }
    bool IsAlive { get; }

    // Events
    event Action<float> OnDamaged;
    event Action OnDestroyed;
    event Action<float> OnHealed;
    
    // Methods
    void ReceiveDamage(float damage);
    void Heal(float amount);
}
