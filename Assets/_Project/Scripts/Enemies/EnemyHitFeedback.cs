using System;
using UnityEngine;

[RequireComponent(typeof(HealthController))]
public class EnemyHitFeedback : MonoBehaviour
{
    [SerializeField] private HitEffect m_HitEffect;
    [SerializeField] private float m_FrontOffset = 2f;

    public static event Action OnEnemyHit;

    private HealthController m_HealthController;
    private Pool<HitEffect> m_HitPool;
    private Camera m_Camera;

    private void Awake()
    {
        m_HealthController = GetComponent<HealthController>();
        m_HealthController.OnDamaged += HandleDamaged;

        if (m_HitEffect != null)
            m_HitPool = PoolManager.Instance.GetPool(m_HitEffect);
    }

    private void OnDestroy()
    {
        m_HealthController.OnDamaged -= HandleDamaged;
    }

    private void HandleDamaged(float damage)
    {
        OnEnemyHit?.Invoke();

        if (m_HitPool != null)
            m_HitPool.Get(GetFrontPoint(), Quaternion.identity).Play(m_HitPool);
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
