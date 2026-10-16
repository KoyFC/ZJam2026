using UnityEngine;
using System.Collections.Generic;

public class Pool<T> where T : Component
{
    private readonly T m_Prefab;
    private readonly Transform m_Parent;
    private readonly Queue<T> m_PoolQueue = new Queue<T>();

    public Pool(T prefab, Transform parent, int initialSize)
    {
        m_Prefab = prefab;
        m_Parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            T instance = Object.Instantiate(m_Prefab, m_Parent);
            instance.gameObject.SetActive(false);
            m_PoolQueue.Enqueue(instance);
        }
    }

    public T Get(Vector3 position, Quaternion rotation)
    {
        if (m_PoolQueue.Count == 0)
            return Object.Instantiate(m_Prefab, position, rotation, m_Parent);

        T instance = m_PoolQueue.Dequeue();
        instance.transform.SetPositionAndRotation(position, rotation);
        instance.gameObject.SetActive(true);
        return instance;
    }

    public void Release(T instance)
    {
        instance.gameObject.SetActive(false);
        m_PoolQueue.Enqueue(instance);
    }
}