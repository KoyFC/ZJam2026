using System;
using UnityEngine;

public class BossSpawner : Singleton<BossSpawner>
{
    [SerializeField, Range(0f, 60f)] private float m_AheadAngle = 20f;
    [SerializeField] private float m_SpawnHeight = 15f;

    private HealthController m_BossHealth;
    private float m_DelayTimer;

    public bool IsCountdownActive { get; private set; }
    public float SpawnTimeLeft => Mathf.Max(0f, m_DelayTimer);
    public bool IsFightActive => m_BossHealth != null;
    public bool IsDefeated { get; private set; }

    public event Action OnBossSpawned;
    public event Action OnBossDefeated;

    private void Start()
    {
        if (WaveSpawner.InstanceExists)
            WaveSpawner.Instance.OnWavesCompleted += StartCountdown;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (WaveSpawner.InstanceExists)
            WaveSpawner.Instance.OnWavesCompleted -= StartCountdown;

        if (m_BossHealth != null)
            m_BossHealth.OnDestroyed -= HandleBossDefeated;
    }

    private void Update()
    {
        if (!IsCountdownActive)
            return;

        if (!IsPlayerInRun())
        {
            IsCountdownActive = false;
            return;
        }

        m_DelayTimer -= Time.deltaTime;
        if (m_DelayTimer <= 0f)
            SpawnBoss();
    }

    private void StartCountdown()
    {
        ZoneDataSO zone = ZoneManager.Instance.CurrentZone;
        if (zone == null || zone.BossPrefab == null || !IsPlayerInRun())
            return;

        RailFuel.Instance.SetPaused(true);
        m_DelayTimer = zone.BossDelay;
        IsCountdownActive = true;
    }

    private void SpawnBoss()
    {
        IsCountdownActive = false;
        ZoneDataSO zone = ZoneManager.Instance.CurrentZone;

        Vector3 position = GetSpawnPosition();
        EnemySpawnAnimation boss = Instantiate(zone.BossPrefab, position, Quaternion.identity);

        m_BossHealth = boss.GetComponent<HealthController>();
        m_BossHealth.OnDestroyed += HandleBossDefeated;

        boss.Spawn(position);
        OnBossSpawned?.Invoke();
    }

    private void HandleBossDefeated()
    {
        m_BossHealth.OnDestroyed -= HandleBossDefeated;
        m_BossHealth = null;

        if (!IsPlayerInRun())
            return;

        IsDefeated = true;
        UnlockedMechanics.Unlock(ZoneManager.Instance.CurrentZone.UnlockedMechanic);
        RailFuel.Instance.SetPaused(false);
        OnBossDefeated?.Invoke();
    }

    private bool IsPlayerInRun()
    {
        return RailFuel.InstanceExists && !RailFuel.Instance.IsStopped;
    }

    private Vector3 GetSpawnPosition()
    {
        ZoneManager zone = ZoneManager.Instance;
        Vector3 direction = PlayerRailFollower.Instance.GetDirectionAhead(m_AheadAngle);
        Vector3 position = zone.transform.position + direction * zone.GetMiddleRadius();
        position.y = m_SpawnHeight;
        return position;
    }
}
