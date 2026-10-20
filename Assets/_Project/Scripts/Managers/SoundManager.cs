using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : PersistentLazySingleton<SoundManager>
{
    #region References
    [SerializeField] private AudioClip m_menuMusic;
    [SerializeField] private AudioClip m_soundEffect;

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

        DontDestroyOnLoad(gameObject);

        m_musicSource = gameObject.AddComponent<AudioSource>();
        m_musicSource.playOnAwake = false;
        m_musicSource.loop = true;

        m_soundEffectSource = gameObject.AddComponent<AudioSource>();
        m_soundEffectSource.playOnAwake = false;
    }

    private void OnEnable()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }
    #endregion

    #region Playback Methods
    public void PlayMusic()
    {
        if (m_menuMusic == null)
        {
            Debug.LogError("Menu music is not assigned in the SoundManager.", this);
            return;
        }

        if (m_musicSource.clip != m_menuMusic)
        {
            m_musicSource.clip = m_menuMusic;
            m_musicSource.Play();
        }
        else if (!m_musicSource.isPlaying)
        {
            m_musicSource.UnPause();
        }
    }

    public void PauseMusic()
    {
        m_musicSource.Pause();
    }

    public void PlaySoundEffect()
    {
        if (m_soundEffect == null)
        {
            Debug.LogError("The sound effect is not assigned in the SoundManager.", this);
            return;
        }

        m_soundEffectSource.PlayOneShot(m_soundEffect);
    }
    #endregion

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenuScene" || scene.name == "NearSideScene")
        {
            PlayMusic();
        }
        else if (scene.name == "FarSideScene")
        {
            PauseMusic();
        }
    }
}
