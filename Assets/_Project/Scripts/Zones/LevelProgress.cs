using System;

public static class LevelProgress
{
    private const string SELECTED_LEVEL_KEY = "SelectedLevel";
    private const string UNLOCKED_LEVEL_KEY = "UnlockedLevel";

    public static event Action<int> OnSelected;

    public static int Selected => SaveSystem.Get(SaveSystem.PLAYER_DATA_FILE, SELECTED_LEVEL_KEY, 1);
    public static int Unlocked => SaveSystem.Get(SaveSystem.PLAYER_DATA_FILE, UNLOCKED_LEVEL_KEY, 1);

    public static bool IsUnlocked(int level)
    {
        return level <= Unlocked;
    }

    public static void Select(int level)
    {
        if (!IsUnlocked(level))
            return;

        SaveSystem.Set(SaveSystem.PLAYER_DATA_FILE, SELECTED_LEVEL_KEY, level);
        SaveSystem.MarkAsDirty();
        OnSelected?.Invoke(level);
    }

    public static void Unlock(int level)
    {
        if (IsUnlocked(level))
            return;

        SaveSystem.Set(SaveSystem.PLAYER_DATA_FILE, UNLOCKED_LEVEL_KEY, level);
        SaveSystem.MarkAsDirty();
    }
}
