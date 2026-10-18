using System;
using UnityEngine;

[RequireComponent(typeof(HealthController))]
public class EnemyHitFeedback : MonoBehaviour
{
    public static event Action OnEnemyHit;

    private HealthController m_HealthController;

    private void Awake()
    {
        m_HealthController = GetComponent<HealthController>();
    }

    private void OnEnable()
    {
        m_HealthController.OnDamaged += HandleDamaged;
    }

    private void OnDisable()
    {
        m_HealthController.OnDamaged -= HandleDamaged;
    }

    private void HandleDamaged(float damage)
    {
        OnEnemyHit?.Invoke();
    }
}
