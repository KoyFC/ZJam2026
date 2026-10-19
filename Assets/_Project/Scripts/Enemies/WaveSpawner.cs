using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : Singleton<WaveSpawner>
{
    private class ActiveEnemy
    {
        public Vector2Int Cell;
        public Pool<EnemySpawnAnimation> Pool;
        public HealthController Health;
        public Action OnDeath;
    }

    [SerializeField] private FlyingEnemyGrid m_Grid;
    private readonly Dictionary<EnemySpawnAnimation, ActiveEnemy> m_ActiveEnemies = new();
    private ZoneDataSO m_CurrentZone;
    private int m_NextWaveIndex;
    private float m_DelayTimer;
    private bool m_IsStopped;

    public int CompletedWaves { get; private set; }
    public int CurrentWave => m_NextWaveIndex;
    public int TotalWaves => m_CurrentZone != null ? m_CurrentZone.Waves.Length : 0;
    public int EnemiesLeft => m_ActiveEnemies.Count;
    public bool IsWaveActive => m_ActiveEnemies.Count > 0;
    public bool IsStopped => m_IsStopped;
    public float NextWaveTimeLeft => Mathf.Max(0f, m_DelayTimer);

    public event Action OnWaveStarted;
    public event Action OnEnemyKilled;
    public event Action OnWavesCompleted;

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
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (RailFuel.InstanceExists)
            RailFuel.Instance.OnFuelEmpty -= Stop;
    }

    private void Update()
    {
        if (m_IsStopped)
            return;

        if (m_ActiveEnemies.Count > 0)
            return;

        m_DelayTimer -= Time.deltaTime;
        if (m_DelayTimer <= 0f)
            StartNextWave();
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
        OnWaveStarted?.Invoke();

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
        m_ActiveEnemies.Add(enemy, activeEnemy);

        enemy.Spawn(cellPosition);
    }

    private void RemoveEnemy(EnemySpawnAnimation enemy)
    {
        if (!m_ActiveEnemies.Remove(enemy, out ActiveEnemy activeEnemy))
            return;

        activeEnemy.Health.OnDestroyed -= activeEnemy.OnDeath;
        m_Grid.ReleaseCell(activeEnemy.Cell);
        activeEnemy.Pool.Release(enemy);
        OnEnemyKilled?.Invoke();

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
}
