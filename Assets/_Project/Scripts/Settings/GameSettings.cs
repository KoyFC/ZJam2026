using System;
using UnityEngine;

public static class GameSettings
{
    public const float MIN_BRIGHTNESS = -0.3f;
    public const float MAX_BRIGHTNESS = 0.6f;

    private const string SETTINGS_FILE = "SettingsData.json";

    public static event Action OnChanged;

    public static float MasterVolume
    {
        get => Get(nameof(MasterVolume), 1f);
        set => Set(nameof(MasterVolume), Mathf.Clamp01(value));
    }

    public static float MusicVolume
    {
        get => Get(nameof(MusicVolume), 1f);
        set => Set(nameof(MusicVolume), Mathf.Clamp01(value));
    }

    public static float SfxVolume
    {
        get => Get(nameof(SfxVolume), 1f);
        set => Set(nameof(SfxVolume), Mathf.Clamp01(value));
    }

    public static float Brightness
    {
        get => Get(nameof(Brightness), 0f);
        set => Set(nameof(Brightness), Mathf.Clamp(value, MIN_BRIGHTNESS, MAX_BRIGHTNESS));
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ApplyMasterVolume()
    {
        AudioListener.volume = MasterVolume;
    }

    private static float Get(string key, float defaultValue)
    {
        return SaveSystem.Get(SETTINGS_FILE, key, defaultValue);
    }

    private static void Set(string key, float value)
    {
        SaveSystem.Set(SETTINGS_FILE, key, value);
        SaveSystem.MarkAsDirty();
        ApplyMasterVolume();
        OnChanged?.Invoke();
    }
}
