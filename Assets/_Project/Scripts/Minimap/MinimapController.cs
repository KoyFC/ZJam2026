using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MinimapController : Singleton<MinimapController>
{
    private Dictionary<MinimapEntity, Image> m_Elements = new Dictionary<MinimapEntity, Image>();
    [SerializeField] private Transform m_Parent;
    [SerializeField] private Image m_IconPrefab;

    public void RegisterElement(MinimapEntity element)
    {
        Image image = Instantiate(m_IconPrefab, m_Parent);
        image.sprite = element.Icon;
        m_Elements.Add(element, image);
    }

    public void UnregisterElement(MinimapEntity element)
    {
        m_Elements.Remove(element);
    }

    private void Update()
    {
        foreach (var element in m_Elements)
        {

        }
    }
}
