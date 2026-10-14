using UnityEngine;

public class GameSaveManager : PersistentLazySingleton<GameSaveManager>
{
    [Header("Save Settings")]
    [Tooltip("Time to wait in seconds before writing data to disk.")]
    [SerializeField] private float m_SaveCooldown = 3f;
    private bool m_IsDirty = false;
    private float m_NextSaveTime = 0f;

    public void MarkAsDirty()
    {
        m_IsDirty = true;
        m_NextSaveTime = Time.unscaledTime + m_SaveCooldown;
    }

    public void ForceSaveNow()
    {
        if (m_IsDirty)
        {
            SaveSystem.SaveAll();
            m_IsDirty = false;
            Debug.Log("[GameSaveManager] Save completed.");
        }
    }

    #region Unity Methods
    private void Update()
    {
        if (m_IsDirty && Time.unscaledTime >= m_NextSaveTime)
        {
            ForceSaveNow();
        }
    }

    private void OnApplicationQuit()
    {
        ForceSaveNow();
    }

    // For mobile
    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused) ForceSaveNow();
    }

    // For PC
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) ForceSaveNow();
    }
    #endregion
}