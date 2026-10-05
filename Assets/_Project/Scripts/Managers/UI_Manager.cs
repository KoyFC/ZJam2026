using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    #region References
    [Header("Canvases")]
    [SerializeField] private GameObject m_upgradeCanvas;
    [SerializeField] private GameObject m_statsCanvas;

    private GameObject m_currentCanvas;
    #endregion

    #region Unity Methods
    public void Awake()
    {
        if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        Instance = this;

        m_currentCanvas = null;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        m_upgradeCanvas.SetActive(false);
        m_statsCanvas.SetActive(false);
    }
    #endregion

    #region Canvas Methods
    public void ToggleUpgradeCanvas()
    {
        if(m_currentCanvas != m_upgradeCanvas)
        {
            CloseCurrentCanvas();
        }
        m_upgradeCanvas.SetActive(!m_upgradeCanvas.activeSelf);
        m_currentCanvas = m_upgradeCanvas.activeSelf ? m_upgradeCanvas : null;
    }

    public void ToggleStatsCanvas()
    {
        if(m_currentCanvas != m_statsCanvas)
        {
            CloseCurrentCanvas();
        }
        m_statsCanvas.SetActive(!m_statsCanvas.activeSelf);
        m_currentCanvas = m_statsCanvas.activeSelf ? m_statsCanvas : null;
    }

    public void CloseCurrentCanvas()
    {
        if(m_currentCanvas != null)
        {
            m_currentCanvas.SetActive(false);
            m_currentCanvas = null;
        }
    }
    #endregion

}
