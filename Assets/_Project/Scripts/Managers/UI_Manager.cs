using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    #region References
    [Header("Canvases")]
    [SerializeField] private GameObject m_statsCanvas;

    private GameObject m_currentCanvas;
    #endregion

    #region Unity Methods
    // I don't think this needs to be persistent

    // protected override void Awake()
    // {
    //     base.Awake();

    //     m_currentCanvas = null;
    //     DontDestroyOnLoad(gameObject);
    // }

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

}
