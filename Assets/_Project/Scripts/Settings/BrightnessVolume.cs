using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class BrightnessVolume : MonoBehaviour
{
    private const float PRIORITY = 1000f;
    private const float MAX_FOG_REDUCTION = 0.5f;

    private VolumeProfile m_Profile;
    private LiftGammaGain m_LiftGammaGain;
    private float m_SceneFogDensity;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Create()
    {
        GameObject volumeObject = new GameObject(nameof(BrightnessVolume));
        DontDestroyOnLoad(volumeObject);
        volumeObject.AddComponent<BrightnessVolume>();
    }

    private void Awake()
    {
        m_Profile = ScriptableObject.CreateInstance<VolumeProfile>();
        m_LiftGammaGain = m_Profile.Add<LiftGammaGain>();
        m_LiftGammaGain.gamma.overrideState = true;

        Volume volume = gameObject.AddComponent<Volume>();
        volume.isGlobal = true;
        volume.priority = PRIORITY;
        volume.sharedProfile = m_Profile;

        ApplyGamma();
    }

    private void OnEnable()
    {
        GameSettings.OnChanged += Apply;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        GameSettings.OnChanged -= Apply;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        Destroy(m_Profile);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        m_SceneFogDensity = RenderSettings.fogDensity;
        ApplyFog();
    }

    private void Apply()
    {
        ApplyGamma();
        ApplyFog();
    }

    private void ApplyGamma()
    {
        m_LiftGammaGain.gamma.value = new Vector4(1f, 1f, 1f, GameSettings.Brightness);
    }

    private void ApplyFog()
    {
        float boost = Mathf.Max(0f, GameSettings.Brightness) / GameSettings.MAX_BRIGHTNESS;
        RenderSettings.fogDensity = m_SceneFogDensity * (1f - MAX_FOG_REDUCTION * boost);
    }
}
