using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject m_Content;
    [SerializeField] private OptionsMenu m_OptionsPrefab;
    [SerializeField] private string m_ExitScene = "NearSideScene";

    private OptionsMenu m_Options;
    private bool m_IsPaused;
    private bool m_RunEnded;
    private bool m_WasCursorVisible;
    private CursorLockMode m_PreviousLockMode;
    private bool m_WasInputActive;
    private bool m_OptionsWereOpen;

    private bool OptionsOpen => m_Options != null && m_Options.gameObject.activeSelf;

    private void Awake()
    {
        m_Content.SetActive(false);
        PlayerRunEnd.OnRunEnded += OnRunEnded;
    }

    private void OnDestroy()
    {
        PlayerRunEnd.OnRunEnded -= OnRunEnded;

        if (m_IsPaused)
            Time.timeScale = 1f;
    }

    private void Update()
    {
        if (SceneLoader.IsBusy || m_OptionsWereOpen)
            return;

        if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame)
            return;

        if (m_IsPaused)
            Resume();
        else
            Pause();
    }

    private void LateUpdate()
    {
        m_OptionsWereOpen = OptionsOpen;
    }

    public void Pause()
    {
        if (m_IsPaused || m_RunEnded)
            return;

        m_IsPaused = true;
        Time.timeScale = 0f;

        m_WasCursorVisible = Cursor.visible;
        m_PreviousLockMode = Cursor.lockState;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (PlayerInputController.InstanceExists)
        {
            m_WasInputActive = PlayerInputController.Instance.IsInputActive;
            PlayerInputController.Instance.SetInputActive(false);
        }

        m_Content.SetActive(true);
    }

    public void Resume()
    {
        if (!m_IsPaused || SceneLoader.IsBusy)
            return;

        m_IsPaused = false;
        Time.timeScale = 1f;

        Cursor.visible = m_WasCursorVisible;
        Cursor.lockState = m_PreviousLockMode;

        if (PlayerInputController.InstanceExists)
            PlayerInputController.Instance.SetInputActive(m_WasInputActive);

        m_Content.SetActive(false);
    }

    public void OpenOptions()
    {
        if (m_Options == null)
            m_Options = Instantiate(m_OptionsPrefab);

        m_Options.Open();
    }

    public void Exit()
    {
        SceneLoader.Load(m_ExitScene);
    }

    private void OnRunEnded()
    {
        m_RunEnded = true;
        Resume();
    }
}
