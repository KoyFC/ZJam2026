using UnityEngine;

public class NearSideMenuController : MonoBehaviour
{
    #region References
    [Header("Canvases")]
    [SerializeField] private GameObject m_statsCanvas;

    private GameObject m_currentCanvas;
    #endregion

    #region Unity Methods

    private void Start()
    {
        m_statsCanvas.SetActive(false);
    }
    #endregion

    #region Canvas Methods
    public void ToggleStatsCanvas()
    {
        if (m_currentCanvas != m_statsCanvas)
        {
            CloseCurrentCanvas();
        }
        m_statsCanvas.SetActive(!m_statsCanvas.activeSelf);
        m_currentCanvas = m_statsCanvas.activeSelf ? m_statsCanvas : null;
    }

    public void CloseCurrentCanvas()
    {
        if (m_currentCanvas != null)
        {
            m_currentCanvas.SetActive(false);
            m_currentCanvas = null;
        }
    }
    #endregion

    public void ExitToMainMenu()
    {
        CloseCurrentCanvas();

        SceneLoader.Load("MainMenuScene");
    }

    public void StartCombat()
    {
        // Close any open canvas before starting combat
        CloseCurrentCanvas();

        // Load the combat scene
        SceneLoader.Load("FarSideScene");
    }

}
