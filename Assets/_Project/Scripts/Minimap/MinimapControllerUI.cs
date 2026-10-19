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

    protected override void Awake()
    {
        base.Awake();
        if (m_BoundsCollider != null)
        {
            Bounds bounds = m_BoundsCollider.bounds;
            m_WorldSize = new Vector2(bounds.size.x, bounds.size.z);
            m_WorldCenter = new Vector2(bounds.center.x, bounds.center.z);
        }
        else if (ZoneManager.InstanceExists)
        {
            Vector3 center = ZoneManager.Instance.transform.position;
            m_WorldSize = Vector2.one * ZoneManager.Instance.OuterRadius * 2f;
            m_WorldCenter = new Vector2(center.x, center.z);
        }
    }

    private void Update()
    {
        foreach (var element in m_Elements)
        {
            var entity = element.Key;
            var icon = element.Value;

            icon.transform.localPosition = WorldToMinimapPosition(entity.transform.position);
            if (entity.RotateWithEntity)
                icon.transform.localRotation = WorldToMinimapRotation(entity.transform);
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
        if (ZoneManager.InstanceExists)
            worldPosition = RemapZonePosition(worldPosition);

        var parentRect = m_Parent.rectTransform.rect;

        // Calculate normalized position of the entity in the world
        var normalizedX = (worldPosition.x - m_WorldCenter.x) / m_WorldSize.x + 0.5f;
        var normalizedY = (worldPosition.z - m_WorldCenter.y) / m_WorldSize.y + 0.5f;

        return new Vector2(parentRect.xMin + normalizedX * parentRect.width, parentRect.yMin + normalizedY * parentRect.height);
    }

    public Quaternion WorldToMinimapRotation(Transform worldTransform)
    {
        return Quaternion.Euler(0f, 0f, -worldTransform.eulerAngles.y);
    }

    public Vector2 GetWorldToMinimapScale()
    {
        var parentRect = m_Parent.rectTransform.rect;
        return new Vector2(parentRect.width / m_WorldSize.x, parentRect.height / m_WorldSize.y);
    }

    public float GetZoneInnerMinimapRadius(int zoneIndex)
    {
        return zoneIndex > 0 ? ZoneManager.Instance.Zones[zoneIndex - 1].MinimapRadius : 0f;
    }

    public float MinimapToWorldRadius(float minimapRadius)
    {
        return minimapRadius * ZoneManager.Instance.OuterRadius;
    }

    private float RemapZoneRadius(float worldRadius)
    {
        if (!ZoneManager.InstanceExists)
            return worldRadius;

        var zoneManager = ZoneManager.Instance;
        int zoneIndex = zoneManager.CurrentZoneIndex;
        if (zoneIndex < 0)
            return worldRadius;

        float ringProgress = Mathf.InverseLerp(zoneManager.InnerRadius, zoneManager.OuterRadius, worldRadius);
        float minimapRadius = Mathf.Lerp(GetZoneInnerMinimapRadius(zoneIndex), zoneManager.Zones[zoneIndex].MinimapRadius, ringProgress);
        return MinimapToWorldRadius(minimapRadius);
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
