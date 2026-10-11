using System;
using UnityEngine;

public class ZoneManager : Singleton<ZoneManager>
{
    [field: SerializeField] public ZoneDataSO[] Zones { get; private set; }
    public ZoneDataSO CurrentZone { get; private set; } = null;
    public int HighestUnlockedZone { get; private set; } = 0; // By index in the array

    public static event Action<ZoneDataSO> OnZoneChanged;
    public static event Action<int> OnZoneUnlocked;

    public float GetUnlockedRadius()
    {
        if (HighestUnlockedZone < 0 || HighestUnlockedZone >= Zones.Length)
            return 0f;

        return Zones[HighestUnlockedZone].Radius;
    }

    protected override void Awake()
    {
        base.Awake();
        if (Zones.Length == 0)
        {
            Debug.LogError("No zones assigned to ZoneManager.");
            HighestUnlockedZone = -1;
            CurrentZone = null;
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

        HighestUnlockedZone = 0; // TODO: Change this depending on the save data
        CurrentZone = Zones[0];
        OnZoneChanged?.Invoke(CurrentZone);
    }

    public bool UnlockNextZone()
    {
        if (HighestUnlockedZone >= Zones.Length - 1)
            return false;

        HighestUnlockedZone++;
        OnZoneUnlocked?.Invoke(HighestUnlockedZone);
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

        Gizmos.color = Color.cyan;
        int index = Mathf.Clamp(HighestUnlockedZone, 0, Zones.Length - 1);
        Gizmos.DrawWireSphere(transform.position, Zones[index].Radius);
    }
}
