using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Slider m_MasterSlider;
    [SerializeField] private Slider m_MusicSlider;
    [SerializeField] private Slider m_SfxSlider;
    [SerializeField] private Slider m_BrightnessSlider;

    private void Awake()
    {
        m_BrightnessSlider.minValue = GameSettings.MIN_BRIGHTNESS;
        m_BrightnessSlider.maxValue = GameSettings.MAX_BRIGHTNESS;

        Bind(m_MasterSlider, GameSettings.MasterVolume, value => GameSettings.MasterVolume = value);
        Bind(m_MusicSlider, GameSettings.MusicVolume, value => GameSettings.MusicVolume = value);
        Bind(m_SfxSlider, GameSettings.SfxVolume, value => GameSettings.SfxVolume = value);
        Bind(m_BrightnessSlider, GameSettings.Brightness, value => GameSettings.Brightness = value);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            Close();
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void ResetBrightness()
    {
        m_BrightnessSlider.value = 0f;
    }

    private static void Bind(Slider slider, float value, Action<float> apply)
    {
        slider.SetValueWithoutNotify(value);
        slider.onValueChanged.AddListener(apply.Invoke);
    }
}
