using UnityEngine;
using UnityEngine.UI;

public class MinimapIconUI : MonoBehaviour
{
    [SerializeField] private Image m_ImageComponent;
    private Sprite m_Icon;

    private void Awake()
    {
        if (m_ImageComponent == null)
        {
            m_ImageComponent = GetComponentInChildren<Image>();
        }
    }

    public void Initialize(MinimapEntity entity)
    {
        m_Icon = entity.Icon;
        m_ImageComponent.sprite = m_Icon;
        m_ImageComponent.rectTransform.localScale = Vector3.one * entity.IconScale;
    }
}
