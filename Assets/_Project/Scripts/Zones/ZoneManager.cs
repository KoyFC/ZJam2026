using System;
using UnityEngine;

public class ZoneManager : Singleton<ZoneManager>
{
    [field: SerializeField] public ZoneDataSO[] Zones { get; private set; }
    public int CurrentZoneIndex { get; private set; } = -1;

    [SerializeField] private float m_GizmoHeight = 20f;

    public float GetInnerRadius()
    {
        if (CurrentZoneIndex <= 0 || CurrentZoneIndex >= Zones.Length)
            return 0f;

        return Zones[CurrentZoneIndex - 1].Radius;
    }

    public float GetOuterRadius()
    {
        if (CurrentZoneIndex < 0 || CurrentZoneIndex >= Zones.Length)
            return 0f;

        return Zones[CurrentZoneIndex].Radius;
    }

    protected override void Awake()
    {
        base.Awake();
        if (Zones.Length == 0)
        {
            Debug.LogError("No zones set in ZoneManager.", this);
            CurrentZoneIndex = -1;
            return;
        }

        Array.Sort(Zones, (a, b) => a.Radius.CompareTo(b.Radius));

        foreach (var zone in Zones)
        {
            if (zone.Radius <= 0)
            {
                Debug.LogError($"Please set a radius above 0 for Zone {zone.ZoneName}.", zone);
            }
        }

        CurrentZoneIndex = Zones.Length - 1; // TODO: Change this depending on the save data
    }

    public bool UnlockNextZone()
    {
        if (CurrentZoneIndex <= 0)
            return false;

        CurrentZoneIndex--;
        return true;
    }

    private void OnValidate()
    {
        if (Zones.Length == 0)
            return;

        Array.Sort(Zones, (a, b) => a.Radius.CompareTo(b.Radius));
    }

    private void OnDrawGizmos()
    {
        if (Zones.Length == 0)
            return;

        int currentZoneIndex = Application.isPlaying ? CurrentZoneIndex : Zones.Length - 1;

        for (int i = 0; i < Zones.Length; i++)
        {
            if (Zones[i] == null)
                continue;

            if (i == currentZoneIndex || i == currentZoneIndex - 1)
                Gizmos.color = Color.cyan;
            else
                Gizmos.color = Color.red;

            DrawWireCylinder(transform.position, Zones[i].Radius, m_GizmoHeight);
        }
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
}
