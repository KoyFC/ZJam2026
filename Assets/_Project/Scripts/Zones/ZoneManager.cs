using System;
using UnityEngine;

public class ZoneManager : Singleton<ZoneManager>
{
    [field: SerializeField] public ZoneDataSO[] Zones { get; private set; }
    public ZoneDataSO CurrentZone { get; private set; } = null;
    public int HighestUnlockedZone { get; private set; } = 0; // By index in the array

    public static event Action<ZoneDataSO> OnZoneChanged;
    public static event Action<int> OnZoneUnlocked;

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

        CurrentZone = Zones[0];
        OnZoneChanged?.Invoke(CurrentZone);
        HighestUnlockedZone = 0; // TODO: Change this depending on the save data
        Array.Sort(Zones, (a, b) => a.Radius.CompareTo(b.Radius));
    }

    public bool UnlockNextZone()
    {
        if (HighestUnlockedZone >= Zones.Length)
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
}
