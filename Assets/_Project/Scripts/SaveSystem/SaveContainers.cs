using System.Collections.Generic;
using System.IO;
using UnityEngine;

public interface ISaveFile
{
    void SaveToDisk(string path);
}

[System.Serializable]
public class SaveItem<T>
{
    public string key;
    public T value;

    public SaveItem(string key, T value)
    {
        this.key = key;
        this.value = value;
    }
}

[System.Serializable]
public class FileContainer<T> : ISaveFile
{
    // Even if it won't be seen in the inspector, mark as SerializeField so it gets saved to file. Could also be public.
    [SerializeField] private List<SaveItem<T>> m_ItemList = new List<SaveItem<T>>();
    private Dictionary<string, T> m_ItemDictionary = new Dictionary<string, T>();

    public T Get(string key, T defaultValue = default)
    {
        return m_ItemDictionary.TryGetValue(key, out T value) ? value : defaultValue;
    }

    public void Set(string key, T value)
    {
        m_ItemDictionary[key] = value;
    }

    public void PopulateDictionaryFromList()
    {
        m_ItemDictionary.Clear();
        foreach (var item in m_ItemList)
        {
            m_ItemDictionary[item.key] = item.value;
        }
    }

    public void SaveToDisk(string path)
    {
        m_ItemList.Clear();
        foreach (var kvp in m_ItemDictionary)
        {
            m_ItemList.Add(new SaveItem<T>(kvp.Key, kvp.Value));
        }
        string json = JsonUtility.ToJson(this, prettyPrint: true);
        File.WriteAllText(path, json);
    }
}