using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

   private GameObject m_currentCanvas;

   private void Awake()
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

    #region Canvas Methods
    private void ToggleCanvas(GameObject canvas)
    {
        if(m_currentCanvas != null)
        {
            m_currentCanvas.SetActive(false);
        }

        m_currentCanvas = canvas;
        m_currentCanvas.SetActive(true);
    }
    #endregion

    #region Public Methods
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
