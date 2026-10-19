using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    private class ActiveEnemy
    {
        public Vector2Int Cell;
        public Pool<EnemySpawnAnimation> Pool;
        public HealthController Health;
        public Action OnDeath;
    }

    [SerializeField] private FlyingEnemyGrid m_Grid;

    public int CompletedWaves { get; private set; }

    public event Action OnWavesCompleted;

    private readonly Dictionary<EnemySpawnAnimation, ActiveEnemy> m_ActiveEnemies = new();
    private ZoneDataSO m_CurrentZone;
    private int m_NextWaveIndex;
    private float m_DelayTimer;
    private float m_WaveTimer;
    private bool m_IsDespawning;
    private bool m_IsStopped;

    private void Start()
    {
        if (!ZoneManager.InstanceExists || ZoneManager.Instance.CurrentZone == null)
        {
            m_IsStopped = true;
            return;
        }

        m_CurrentZone = ZoneManager.Instance.CurrentZone;
        m_DelayTimer = m_CurrentZone.DelayBetweenWaves;

        if (RailFuel.InstanceExists)
            RailFuel.Instance.OnFuelEmpty += Stop;

        NotifyWavesEndByTime();
    }

    private void OnDestroy()
    {
        if (RailFuel.InstanceExists)
            RailFuel.Instance.OnFuelEmpty -= Stop;
    }

    private void Update()
    {
        if (m_IsStopped)
            return;

        if (m_ActiveEnemies.Count == 0)
        {
            m_DelayTimer -= Time.deltaTime;
            if (m_DelayTimer <= 0f)
                StartNextWave();

            return;
        }

        m_WaveTimer -= Time.deltaTime;
        if (m_WaveTimer <= 0f && !m_IsDespawning)
            DespawnAll();
    }

    public void Stop()
    {
        m_IsStopped = true;
    }

    private void StartNextWave()
    {
        if (m_NextWaveIndex >= m_CurrentZone.Waves.Length)
        {
            Stop();
            return;
        }

        WaveData wave = m_CurrentZone.Waves[m_NextWaveIndex];
        m_NextWaveIndex++;
        m_WaveTimer = wave.TimeLimit;
        m_IsDespawning = false;

        foreach (WaveEntry entry in wave.Enemies)
        {
            for (int i = 0; i < entry.Count; i++)
            {
                if (!m_Grid.TryGetRandomFreeCell(out Vector2Int cell))
                {
                    Debug.LogWarning($"Wave {m_NextWaveIndex} of {m_CurrentZone.ZoneName} has more enemies than grid cells.", this);
                    return;
                }

                SpawnEnemy(entry.Prefab, cell);
            }
        }
    }

    private void SpawnEnemy(EnemySpawnAnimation prefab, Vector2Int cell)
    {
        Pool<EnemySpawnAnimation> pool = PoolManager.Instance.GetPool(prefab);
        Vector3 cellPosition = m_Grid.GetCellWorldPosition(cell);
        EnemySpawnAnimation enemy = pool.Get(cellPosition, Quaternion.identity);

        var activeEnemy = new ActiveEnemy
        {
            Cell = cell,
            Pool = pool,
            Health = enemy.GetComponent<HealthController>(),
            OnDeath = () => RemoveEnemy(enemy)
        };

        // Subscribed after DestroyOnDeath, so the death explosion still plays before the enemy goes back to the pool
        activeEnemy.Health.OnDestroyed += activeEnemy.OnDeath;
        enemy.OnDespawn += RemoveEnemy;
        m_ActiveEnemies.Add(enemy, activeEnemy);

        enemy.Spawn(cellPosition);
    }

    private void DespawnAll()
    {
        m_IsDespawning = true;
        foreach (EnemySpawnAnimation enemy in m_ActiveEnemies.Keys)
            enemy.Despawn();
    }

    private void RemoveEnemy(EnemySpawnAnimation enemy)
    {
        if (!m_ActiveEnemies.Remove(enemy, out ActiveEnemy activeEnemy))
            return;

        activeEnemy.Health.OnDestroyed -= activeEnemy.OnDeath;
        enemy.OnDespawn -= RemoveEnemy;
        m_Grid.ReleaseCell(activeEnemy.Cell);
        activeEnemy.Pool.Release(enemy);

        if (m_ActiveEnemies.Count > 0)
            return;

        CompletedWaves++;
        if (m_NextWaveIndex < m_CurrentZone.Waves.Length)
        {
            m_DelayTimer = m_CurrentZone.DelayBetweenWaves;
            return;
        }

        Stop();
        OnWavesCompleted?.Invoke();
    }

    private void NotifyWavesEndByTime()
    {
        // Debug
        // A player who waits out every wave should run out of fuel first
        if (!PlayerRailFollower.InstanceExists)
            return;

        float lapDuration = 2f * Mathf.PI * ZoneManager.Instance.GetMiddleRadius() / PlayerRailFollower.Instance.ForwardSpeed;
        if (m_CurrentZone.TotalWaveTime <= lapDuration)
            Debug.LogWarning($"Waves of {m_CurrentZone.ZoneName} last {m_CurrentZone.TotalWaveTime:0}s, less than a lap ({lapDuration:0}s). Waiting them out beats the zone.", this);
    }
}
