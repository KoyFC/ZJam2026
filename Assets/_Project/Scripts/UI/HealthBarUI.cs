using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider m_Slider;
    [SerializeField] private HealthController m_Health;

    private void Awake()
    {
        m_Slider.minValue = 0f;
        m_Slider.maxValue = 1f;
        m_Slider.interactable = false;
    }

    private void Update()
    {
        if (m_Health == null)
            return;

        m_Slider.value = m_Health.CurrentHealth / m_Health.MaxHealth;
    }
}
