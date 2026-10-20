using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FuelBarUI : MonoBehaviour
{
    [SerializeField] private Slider m_Slider;
    [SerializeField] private TMP_Text m_PercentText;

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

        float fuelLeft = 1f - RailFuel.Instance.Normalized;
        m_Slider.value = fuelLeft;
        m_PercentText.text = $"{fuelLeft * 100f:0}%";
    }
}
