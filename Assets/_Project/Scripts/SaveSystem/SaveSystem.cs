using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    public static readonly string PLAYER_DATA_FILE = "PlayerData.json";
    public static readonly string PLAYER_STATS_FILE = "PlayerStats.json";
    public static readonly string SESSION_DATA_FILE = "SessionData.json";
    private static Dictionary<string, ISaveFile> s_FileRegistry = new Dictionary<string, ISaveFile>();

    public static T Get<T>(string fileName, string key, T defaultValue = default)
    {
        FileContainer<T> container = GetOrCreateContainer<T>(fileName);
        return container.Get(key, defaultValue);
    }

    public static void Set<T>(string fileName, string key, T value)
    {
        FileContainer<T> container = GetOrCreateContainer<T>(fileName);
        container.Set(key, value);
    }

    private static FileContainer<T> GetOrCreateContainer<T>(string fileName)
    {
        if (s_FileRegistry.ContainsKey(fileName))
        {
            return s_FileRegistry[fileName] as FileContainer<T>;
        }

        // If not found in memory, try loading from disk
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            FileContainer<T> loadedContainer = JsonUtility.FromJson<FileContainer<T>>(json);

            // IMPORTANT STEP HERE DO NOT SKIP
            loadedContainer.PopulateDictionaryFromList();

            s_FileRegistry[fileName] = loadedContainer;
            return loadedContainer;
        }

        // If it doesn't exist on disk either, create a new container in memory
        FileContainer<T> newContainer = new FileContainer<T>();
        s_FileRegistry[fileName] = newContainer;
        return newContainer;
    }

    public static void SaveAll()
    {
        foreach (var registry in s_FileRegistry)
        {
            string path = Path.Combine(Application.persistentDataPath, registry.Key);
            registry.Value.SaveToDisk(path);
            Debug.Log($"[SaveSystem] File saved: {registry.Key}");
        }
    }

    public static void MarkAsDirty()
    {
        GameSaveManager.Instance.MarkAsDirty();
    }
}