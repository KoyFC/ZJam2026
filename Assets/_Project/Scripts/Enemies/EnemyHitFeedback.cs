using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(HealthController))]
public class EnemyHitFeedback : MonoBehaviour
{
    public static event Action OnEnemyHit;

    [Header("Outline")]
    [SerializeField, Min(0f)] private float m_OutlineWidth = 4f;
    [SerializeField, Min(0f)] private float m_OutlineDuration = 0.2f;
    [SerializeField] private Ease m_OutlineEase = Ease.InQuad;

    private HealthController m_HealthController;
    private Outline m_Outline;

    private void Awake()
    {
        m_HealthController = GetComponent<HealthController>();
        m_Outline = GetComponentInChildren<Outline>();
    }

    private void OnEnable()
    {
        m_HealthController.OnDamaged += HandleDamaged;
        HideOutline();
    }

    private void OnDisable()
    {
        m_HealthController.OnDamaged -= HandleDamaged;
        HideOutline();
    }

    private void HandleDamaged(float damage)
    {
        OnEnemyHit?.Invoke();
        FlashOutline();
    }

    private void FlashOutline()
    {
        if (m_Outline == null)
            return;

        m_Outline.DOKill();
        m_Outline.OutlineWidth = m_OutlineWidth;
        m_Outline.DOWidth(0f, m_OutlineDuration).SetEase(m_OutlineEase);
    }

    private void HideOutline()
    {
        if (m_Outline == null)
            return;

        m_Outline.DOKill();
        m_Outline.OutlineWidth = 0f;
    }
}
