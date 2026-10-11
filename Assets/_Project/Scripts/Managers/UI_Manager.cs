using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    #region References
    [Header("Canvases")]
    [SerializeField] private GameObject m_upgradeCanvas;
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
        m_upgradeCanvas.SetActive(false);
        m_statsCanvas.SetActive(false);
    }
    #endregion

    #region Canvas Methods
    public void ToggleUpgradeCanvas()
    {
        if (m_currentCanvas != m_upgradeCanvas)
        {
            CloseCurrentCanvas();
        }
        m_upgradeCanvas.SetActive(!m_upgradeCanvas.activeSelf);
        m_currentCanvas = m_upgradeCanvas.activeSelf ? m_upgradeCanvas : null;

        if (!m_upgradeCanvas.activeSelf)
        {
            ScrollRect scrollRect = m_upgradeCanvas.GetComponentInChildren<ScrollRect>();
            if (scrollRect != null)
            {
                scrollRect.normalizedPosition = new Vector2(0.5f, 0.5f);
            }
        }
    }

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
