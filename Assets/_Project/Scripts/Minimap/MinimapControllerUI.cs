using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MinimapControllerUI : Singleton<MinimapControllerUI>
{
    [SerializeField] private Collider m_BoundsCollider;
    [SerializeField] private Vector2 m_WorldSize = new Vector2(100, 100);
    [SerializeField] private Vector2 m_WorldCenter = Vector2.zero;

    private Dictionary<MinimapEntity, MinimapIconUI> m_Elements = new();
    [SerializeField] private Image m_Parent;
    [SerializeField] private MinimapIconUI m_IconPrefab;

    private void Start()
    {
        if (m_BoundsCollider != null)
        {
            Bounds bounds = m_BoundsCollider.bounds;
            m_WorldSize = new Vector2(bounds.size.x, bounds.size.z);
            m_WorldCenter = new Vector2(bounds.center.x, bounds.center.z);
        }
    }

    public void RegisterElement(MinimapEntity element)
    {
        if (!m_Elements.ContainsKey(element))
        {
            var icon = Instantiate(m_IconPrefab, m_Parent.transform);
            icon.Initialize(element);
            m_Elements.Add(element, icon);
        }
    }

    public void UnregisterElement(MinimapEntity element)
    {
        if (m_Elements.ContainsKey(element))
        {
            Destroy(m_Elements[element].gameObject);
            m_Elements.Remove(element);
        }
    }

    private void Update()
    {
        foreach (var element in m_Elements)
        {
            var entity = element.Key;
            var icon = element.Value;

            var parentRect = m_Parent.rectTransform.rect;

            // Calculate normalized position of the entity in the world
            var normalizedX = (entity.transform.position.x - m_WorldCenter.x) / m_WorldSize.x + 0.5f;
            var normalizedY = (entity.transform.position.z - m_WorldCenter.y) / m_WorldSize.y + 0.5f;

            // Calculate the position of the icon in the minimap
            var iconX = parentRect.xMin + normalizedX * parentRect.width;
            var iconY = parentRect.yMin + normalizedY * parentRect.height;

            icon.transform.localPosition = new Vector3(iconX, iconY, 0);
        }
    }
}
