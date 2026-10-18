using System;
using UnityEngine;

[RequireComponent(typeof(HealthController))]
public class EnemyHitFeedback : MonoBehaviour
{
    [SerializeField] private HitEffect m_HitEffectPrefab;
    [SerializeField] private float m_FrontOffset = 2f;

    public static event Action OnEnemyHit;

    private HealthController m_HealthController;
    private HitEffect m_HitEffect;
    private Camera m_Camera;

    private void Awake()
    {
        m_HealthController = GetComponent<HealthController>();
        m_HealthController.OnDamaged += HandleDamaged;

        if (m_HitEffectPrefab != null)
            m_HitEffect = Instantiate(m_HitEffectPrefab);
    }

    private void OnDestroy()
    {
        m_HealthController.OnDamaged -= HandleDamaged;

        if (m_HitEffect != null)
            Destroy(m_HitEffect.gameObject);
    }

    private void HandleDamaged(float damage)
    {
        OnEnemyHit?.Invoke();

        if (m_HitEffect != null)
            m_HitEffect.Play(GetFrontPoint());
    }

    private Vector3 GetFrontPoint()
    {
        if (m_Camera == null)
            m_Camera = Camera.main;

        if (m_Camera == null)
            return transform.position;

        Vector3 toCamera = (m_Camera.transform.position - transform.position).normalized;
        return transform.position + toCamera * m_FrontOffset;
    }
}
