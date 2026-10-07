using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MinimapControllerUI : Singleton<MinimapControllerUI>
{
    private Dictionary<MinimapEntity, MinimapIconUI> m_Elements = new();
    [SerializeField] private Transform m_Parent;
    [SerializeField] private MinimapIconUI m_IconPrefab;

    public void RegisterElement(MinimapEntity element)
    {
        if (!m_Elements.ContainsKey(element))
        {
            var icon = Instantiate(m_IconPrefab, m_Parent);
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

            Vector3 worldPosition = entity.transform.position;
            Vector3 minimapPosition = new Vector3(worldPosition.x, worldPosition.z, 0);
            icon.transform.localPosition = minimapPosition;
        }
    }
}
