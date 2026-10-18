using System.Collections.Generic;
using UnityEngine;

public class PoolManager : LazySingleton<PoolManager>
{
    private readonly Dictionary<Component, object> m_Pools = new Dictionary<Component, object>();

    public Pool<T> GetPool<T>(T prefab, int initialSize = 0) where T : Component
    {
        if (m_Pools.TryGetValue(prefab, out object pool))
            return (Pool<T>)pool;

        Transform container = new GameObject($"{prefab.name} Pool").transform;
        container.SetParent(transform);

        Pool<T> created = new Pool<T>(prefab, container, initialSize);
        m_Pools[prefab] = created;
        return created;
    }
}
