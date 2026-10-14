using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MinimapControllerUI : Singleton<MinimapControllerUI>
{
    [SerializeField] private Collider m_BoundsCollider;
    [SerializeField] private Vector2 m_WorldSize = new Vector2(100, 100);
    [SerializeField] private Vector2 m_WorldCenter = Vector2.zero;
    [Tooltip("Places each zone on the minimap using its Minimap Radius instead of its real radius.")]
    [SerializeField] private bool m_UseZoneMinimapRadius = true;

    private Dictionary<MinimapEntity, MinimapIconUI> m_Elements = new();
    [SerializeField] private Image m_Parent;
    [SerializeField] private MinimapIconUI m_IconPrefab;

    protected override void Awake()
    {
        base.Awake();
        if (m_BoundsCollider != null)
        {
            Bounds bounds = m_BoundsCollider.bounds;
            m_WorldSize = new Vector2(bounds.size.x, bounds.size.z);
            m_WorldCenter = new Vector2(bounds.center.x, bounds.center.z);
        }
    }

    private void Update()
    {
        foreach (var element in m_Elements)
        {
            var entity = element.Key;
            var icon = element.Value;

            icon.transform.localPosition = WorldToMinimapPosition(entity.transform.position);
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

    public Vector2 WorldToMinimapPosition(Vector3 worldPosition)
    {
        if (m_UseZoneMinimapRadius && ZoneManager.InstanceExists)
            worldPosition = RemapZonePosition(worldPosition);

        var parentRect = m_Parent.rectTransform.rect;

        // Calculate normalized position of the entity in the world
        var normalizedX = (worldPosition.x - m_WorldCenter.x) / m_WorldSize.x + 0.5f;
        var normalizedY = (worldPosition.z - m_WorldCenter.y) / m_WorldSize.y + 0.5f;

        return new Vector2(parentRect.xMin + normalizedX * parentRect.width, parentRect.yMin + normalizedY * parentRect.height);
    }

    public Vector2 GetWorldToMinimapScale()
    {
        var parentRect = m_Parent.rectTransform.rect;
        return new Vector2(parentRect.width / m_WorldSize.x, parentRect.height / m_WorldSize.y);
    }

    public float RemapZoneRadius(float worldRadius)
    {
        if (!m_UseZoneMinimapRadius || !ZoneManager.InstanceExists)
            return worldRadius;

        var zones = ZoneManager.Instance.Zones;
        if (zones.Length == 0)
            return worldRadius;

        var outermostZone = zones[zones.Length - 1];
        float outermostRadius = outermostZone.Radius;

        for (int i = 0; i < zones.Length; i++)
        {
            if (worldRadius > zones[i].Radius)
                continue;

            float previousRadius = i == 0 ? 0f : zones[i - 1].Radius;
            float previousMinimapRadius = i == 0 ? 0f : zones[i - 1].MinimapRadius;
            float zoneProgress = Mathf.InverseLerp(previousRadius, zones[i].Radius, worldRadius);
            return Mathf.Lerp(previousMinimapRadius, zones[i].MinimapRadius, zoneProgress) * outermostRadius;
        }

        float distanceOutsideZones = worldRadius - outermostRadius;
        return outermostZone.MinimapRadius * outermostRadius + distanceOutsideZones;
    }

    private Vector3 RemapZonePosition(Vector3 worldPosition)
    {
        var zoneCenter = ZoneManager.Instance.transform.position;
        var offset = worldPosition - zoneCenter;
        offset.y = 0f;

        float distance = offset.magnitude;
        if (distance <= Mathf.Epsilon)
            return worldPosition;

        var direction = offset / distance;
        return zoneCenter + direction * RemapZoneRadius(distance);
    }
}
