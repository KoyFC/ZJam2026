using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : PersistentLazySingleton<SoundManager>
{
    private const float MUSIC_VOLUME = 1f;
    private const float FADE_DURATION = 0.5f;

    [SerializeField] private AudioClip clickSFX;


#region References
private AudioSource m_musicSource;
    private AudioSource m_soundEffectSource;
    #endregion

    private float m_musicFade;

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();

        if (Instance != this)
        {
            return;
        }

        m_musicSource = gameObject.AddComponent<AudioSource>();
        m_musicSource.playOnAwake = false;
        m_musicSource.loop = true;

        m_soundEffectSource = gameObject.AddComponent<AudioSource>();
        m_soundEffectSource.playOnAwake = false;

        ApplyVolumes();
        GameSettings.OnChanged += ApplyVolumes;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameSettings.OnChanged -= ApplyVolumes;
    }
    #endregion

    #region Playback Methods
    public void PlayMusic(AudioClip clip)
    {
        if (m_musicSource.clip == clip)
        {
            return;
        }

        m_musicSource.DOKill();

        if (!m_musicSource.isPlaying)
        {
            SwapMusic(clip);
            return;
        }

        FadeMusic(0f).OnComplete(() => SwapMusic(clip));
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        m_soundEffectSource.PlayOneShot(clip);
    }
    #endregion

    private void SwapMusic(AudioClip clip)
    {
        m_musicSource.clip = clip;

        if (clip == null)
        {
            m_musicSource.Stop();
            return;
        }

        m_musicFade = 0f;
        ApplyVolumes();
        m_musicSource.Play();
        FadeMusic(1f);
    }

    private Tween FadeMusic(float target)
    {
        return DOTween.To(() => m_musicFade, SetMusicFade, target, FADE_DURATION).SetUpdate(true).SetTarget(m_musicSource);
    }

    private void SetMusicFade(float fade)
    {
        m_musicFade = fade;
        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        m_musicSource.volume = MUSIC_VOLUME * GameSettings.MusicVolume * m_musicFade;
        m_soundEffectSource.volume = GameSettings.SfxVolume;
    }

    public void PlayClickSound()
    {
        if (clickSFX != null)
            SoundManager.Instance.PlaySoundEffect(clickSFX);
    }
    


}
