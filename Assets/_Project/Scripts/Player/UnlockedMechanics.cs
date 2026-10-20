using System;

public enum PlayerMechanic
{
    None,
    Missiles,
    Shield
}

public static class UnlockedMechanics
{
    private const string KEY_PREFIX = "Mechanic_";

    public static event Action<PlayerMechanic> OnUnlocked;

    public static bool IsUnlocked(PlayerMechanic mechanic)
    {
        return mechanic == PlayerMechanic.None || SaveSystem.Get(SaveSystem.PLAYER_DATA_FILE, KEY_PREFIX + mechanic, 0) == 1;
    }

    public static void Unlock(PlayerMechanic mechanic)
    {
        if (IsUnlocked(mechanic))
            return;

        SaveSystem.Set(SaveSystem.PLAYER_DATA_FILE, KEY_PREFIX + mechanic, 1);
        SaveSystem.MarkAsDirty();
        OnUnlocked?.Invoke(mechanic);
    }
}
