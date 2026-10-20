using System;
using UnityEngine;

public class ZoneManager : Singleton<ZoneManager>
{
    [field: SerializeField] public ZoneDataSO[] Zones { get; private set; }
    public int CurrentZoneIndex { get; private set; } = -1;
    public ZoneDataSO CurrentZone => CurrentZoneIndex >= 0 ? Zones[CurrentZoneIndex] : null;
    // Level 1 is the outermost zone
    public int CurrentLevel => Zones.Length - CurrentZoneIndex;
    public int LevelCount => Zones.Length - 1;
    public bool IsFinalLevel => CurrentLevel == LevelCount;

    public event Action OnZoneChanged;

    [field: SerializeField, Min(0f)] public float InnerRadius { get; private set; } = 350f;
    [field: SerializeField, Min(0f)] public float OuterRadius { get; private set; } = 500f;
    [SerializeField] private float m_GizmoHeight = 20f;

    protected override void Awake()
    {
        base.Awake();
        if (Zones.Length == 0)
        {
            Debug.LogError("No zones set in ZoneManager.", this);
            CurrentZoneIndex = -1;
            return;
        }

        SortZones();
        SelectLevel(LevelProgress.Selected);
    }

    private void OnEnable()
    {
        LevelProgress.OnSelected += SelectLevel;
    }

    private void OnDisable()
    {
        LevelProgress.OnSelected -= SelectLevel;
    }

    private void SelectLevel(int level)
    {
        SetCurrentZone(Mathf.Clamp(Zones.Length - level, 0, Zones.Length - 1));
    }

    public bool SetCurrentZone(int zoneIndex)
    {
        if (zoneIndex < 0 || zoneIndex >= Zones.Length)
            return false;

        CurrentZoneIndex = zoneIndex;
        OnZoneChanged?.Invoke();
        return true;
    }

    private void SortZones()
    {
        Array.Sort(Zones, (a, b) => a.MinimapRadius.CompareTo(b.MinimapRadius));
    }

    private void OnValidate()
    {
        if (InnerRadius >= OuterRadius)
            Debug.LogWarning("Inner Radius should be lower than Outer Radius.", this);

        if (Zones == null || Zones.Length == 0 || Array.IndexOf(Zones, null) >= 0)
            return;

        SortZones();

        for (int i = 1; i < Zones.Length; i++)
        {
            if (Zones[i] != null && Zones[i - 1] != null && Zones[i].MinimapRadius <= Zones[i - 1].MinimapRadius)
            {
                Debug.LogWarning($"Zone {Zones[i].ZoneName} should have a higher Minimap Radius than {Zones[i - 1].ZoneName}.", Zones[i]);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if (InnerRadius > 0f)
            DrawWireCylinder(transform.position, InnerRadius, m_GizmoHeight);
        DrawWireCylinder(transform.position, OuterRadius, m_GizmoHeight);
    }

    private void DrawWireCylinder(Vector3 center, float radius, float height)
    {
        const int segments = 64;
        Vector3 up = Vector3.up * (height * 0.5f);
        Vector3 previous = center + Vector3.right * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 next = center + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;

            Gizmos.DrawLine(previous - up, next - up);
            Gizmos.DrawLine(previous + up, next + up);

            if (i % (segments / 8) == 0)
                Gizmos.DrawLine(next - up, next + up);

            previous = next;
        }
    }

    public float GetMiddleRadius()
    {
        return (InnerRadius + OuterRadius) * 0.5f;
    }

    public Vector3 GetOffsetFromCenter(Vector3 position)
    {
        Vector3 offset = position - transform.position;
        offset.y = 0f;
        return offset;
    }

    public Vector3 GetTangent(Vector3 position, bool clockwise)
    {
        Vector3 tangent = Vector3.Cross(Vector3.up, GetOffsetFromCenter(position).normalized);
        return clockwise ? tangent : -tangent;
    }
}
