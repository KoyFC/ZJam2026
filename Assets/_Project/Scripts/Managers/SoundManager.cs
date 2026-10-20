using DG.Tweening;
using UnityEngine;

public class SoundManager : PersistentLazySingleton<SoundManager>
{
    private const float MUSIC_VOLUME = 1f;
    private const float FADE_DURATION = 0.5f;

    #region References
    private AudioSource m_musicSource;
    private AudioSource m_soundEffectSource;
    #endregion

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
    }
    #endregion

    #region Playback Methods
    // A null clip fades the current music out and leaves silence
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

        m_musicSource.DOFade(0f, FADE_DURATION).SetUpdate(true).OnComplete(() => SwapMusic(clip));
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

        m_musicSource.volume = 0f;
        m_musicSource.Play();
        m_musicSource.DOFade(MUSIC_VOLUME, FADE_DURATION).SetUpdate(true);
    }
}
