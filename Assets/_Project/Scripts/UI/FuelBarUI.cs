using UnityEngine;
using UnityEngine.UI;

public class FuelBarUI : MonoBehaviour
{
    [SerializeField] private Slider m_Slider;

    private void Awake()
    {
        m_Slider.minValue = 0f;
        m_Slider.maxValue = 1f;
        m_Slider.interactable = false;
    }

    private void Update()
    {
        if (!RailFuel.InstanceExists)
            return;

        m_Slider.value = 1f - RailFuel.Instance.Normalized;
    }
}
