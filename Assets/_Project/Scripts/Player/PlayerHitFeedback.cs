using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(HealthController))]
public class PlayerHitFeedback : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float m_FlashDuration = 0.3f;
    [SerializeField, Min(0.01f)] private float m_BlinkInterval = 0.05f;
    [SerializeField] private Color m_HitOutlineColor = Color.red;

    private HealthController m_HealthController;
    private Outline m_Outline;
    private Color m_OriginalOutlineColor;
    private Renderer[] m_ShipRenderers;
    private bool[] m_OriginalRendererStates;
    private Sequence m_FlashSequence;
    private bool m_ShipVisible = true;

    private void Awake()
    {
        m_HealthController = GetComponent<HealthController>();
        m_Outline = GetComponentInChildren<Outline>(true);

        if (m_Outline == null)
        {
            Debug.LogError($"{name}: PlayerHitFeedback requires an Outline on the ship.", this);
            return;
        }

        m_OriginalOutlineColor = m_Outline.OutlineColor;
        m_ShipRenderers = m_Outline.GetComponentsInChildren<Renderer>(true);
        m_OriginalRendererStates = new bool[m_ShipRenderers.Length];
        for (int i = 0; i < m_ShipRenderers.Length; i++)
        {
            m_OriginalRendererStates[i] = m_ShipRenderers[i].enabled;
        }

        if (m_ShipRenderers.Length == 0)
            Debug.LogWarning($"{name}: No renderers were found on the ship.", this);
    }

    private void OnEnable()
    {
        if (m_HealthController == null)
            m_HealthController = GetComponent<HealthController>();

        m_HealthController.OnDamaged += HandleDamaged;
    }

    private void OnDisable()
    {
        if (m_HealthController != null)
            m_HealthController.OnDamaged -= HandleDamaged;

        ResetFeedback();
    }

    private void HandleDamaged(float damage)
    {
        if (m_ShipRenderers == null || m_ShipRenderers.Length == 0)
            return;

        m_FlashSequence?.Kill();
        RestoreShipRenderers();
        m_Outline.OutlineColor = m_HitOutlineColor;

        m_FlashSequence = DOTween.Sequence();
        float remainingDuration = Mathf.Max(0.01f, m_FlashDuration);
        float blinkInterval = Mathf.Max(0.01f, m_BlinkInterval);
        while (remainingDuration > 0f)
        {
            float interval = Mathf.Min(blinkInterval, remainingDuration);
            m_FlashSequence.AppendInterval(interval);
            m_FlashSequence.AppendCallback(ToggleShipVisibility);
            remainingDuration -= interval;
        }

        m_FlashSequence.AppendCallback(RestoreFeedback).SetLink(gameObject);
    }

    private void ToggleShipVisibility()
    {
        m_ShipVisible = !m_ShipVisible;

        for (int i = 0; i < m_ShipRenderers.Length; i++)
        {
            if (m_ShipRenderers[i] != null)
                m_ShipRenderers[i].enabled = m_OriginalRendererStates[i] && m_ShipVisible;
        }
    }

    private void RestoreShipRenderers()
    {
        if (m_ShipRenderers == null)
            return;

        m_ShipVisible = true;
        for (int i = 0; i < m_ShipRenderers.Length; i++)
        {
            if (m_ShipRenderers[i] != null)
                m_ShipRenderers[i].enabled = m_OriginalRendererStates[i];
        }
    }

    private void ResetFeedback()
    {
        m_FlashSequence?.Kill();
        RestoreFeedback();
    }

    private void RestoreFeedback()
    {
        RestoreShipRenderers();

        if (m_Outline != null)
            m_Outline.OutlineColor = m_OriginalOutlineColor;
    }
}
