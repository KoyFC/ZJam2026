using System;
using UnityEngine;

public class ZoneManager : Singleton<ZoneManager>
{
    [field: SerializeField] public ZoneDataSO[] Zones { get; private set; }
    public int CurrentZoneIndex { get; private set; } = -1;

    public static event Action<ZoneDataSO> OnZoneChanged;
    public static event Action<int> OnZoneUnlocked;

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
            Debug.LogError("No zones assigned to ZoneManager.");
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
        OnZoneChanged?.Invoke(Zones[CurrentZoneIndex]);
    }

    public bool UnlockNextZone()
    {
        if (CurrentZoneIndex <= 0)
            return false;

        CurrentZoneIndex--;
        OnZoneUnlocked?.Invoke(CurrentZoneIndex);
        // OnZoneChanged?.Invoke(Zones[CurrentZoneIndex]);
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

        int index = Application.isPlaying ? CurrentZoneIndex : Zones.Length - 1;
        if (index < 0 || index >= Zones.Length)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, Zones[index].Radius);

        if (index > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, Zones[index - 1].Radius);
        }
    }
}
