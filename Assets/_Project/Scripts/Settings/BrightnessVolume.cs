using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BrightnessVolume : MonoBehaviour
{
    private const float PRIORITY = 1000f;

    private VolumeProfile m_Profile;
    private LiftGammaGain m_LiftGammaGain;

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

        Apply();
    }

    private void OnEnable()
    {
        GameSettings.OnChanged += Apply;
    }

    private void OnDisable()
    {
        GameSettings.OnChanged -= Apply;
    }

    private void OnDestroy()
    {
        Destroy(m_Profile);
    }

    private void Apply()
    {
        m_LiftGammaGain.gamma.value = new Vector4(1f, 1f, 1f, GameSettings.Brightness);
    }
}
