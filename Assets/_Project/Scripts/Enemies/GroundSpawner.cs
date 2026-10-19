using System.Collections.Generic;
using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    [SerializeField, Range(0f, 180f)] private float m_StartAngle = 30f;
    [SerializeField, Min(0f)] private float m_BarrierMargin = 10f;

    [Header("Placement")]
    [SerializeField] private LayerMask m_GroundMask;
    [SerializeField, Min(0f)] private float m_RaycastHeight = 200f;
    [SerializeField, Min(0f)] private float m_MinSeparation = 15f;
    [SerializeField, Min(1)] private int m_MaxAttempts = 5;

    private readonly List<EnemySpawnAnimation> m_Enemies = new();
    private ZoneDataSO m_CurrentZone;

    private void Start()
    {
        if (!ZoneManager.InstanceExists || ZoneManager.Instance.CurrentZone == null || !PlayerRailFollower.InstanceExists)
            return;

        m_CurrentZone = ZoneManager.Instance.CurrentZone;
        SpawnEnemies();

        if (WaveSpawner.InstanceExists)
            WaveSpawner.Instance.OnWaveStarted += RaiseEnemies;
        else
            RaiseEnemies();
    }

    private void OnDestroy()
    {
        if (WaveSpawner.InstanceExists)
            WaveSpawner.Instance.OnWaveStarted -= RaiseEnemies;
    }

    private void SpawnEnemies()
    {
        int count = m_CurrentZone.GroundEnemyCount;
        float slotAngle = (360f - m_StartAngle) / Mathf.Max(1, count);

        for (int i = 0; i < count; i++)
        {
            EnemySpawnAnimation prefab = GetRandomPrefab();
            if (prefab == null)
                return;

            for (int attempt = 0; attempt < m_MaxAttempts; attempt++)
            {
                float angle = m_StartAngle + (i + Random.value) * slotAngle;
                if (TryGetSpawnPoint(angle, out RaycastHit hit))
                {
                    EnemySpawnAnimation enemy = Instantiate(prefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal), transform);
                    enemy.gameObject.SetActive(false);
                    m_Enemies.Add(enemy);
                    break;
                }
            }
        }
    }

    private void RaiseEnemies()
    {
        if (WaveSpawner.InstanceExists)
            WaveSpawner.Instance.OnWaveStarted -= RaiseEnemies;

        foreach (EnemySpawnAnimation enemy in m_Enemies)
        {
            enemy.gameObject.SetActive(true);
            enemy.Spawn(enemy.transform.position);
        }
    }

    private EnemySpawnAnimation GetRandomPrefab()
    {
        GroundEnemyEntry[] entries = m_CurrentZone.GroundEnemies;
        if (entries == null || entries.Length == 0)
            return null;

        float totalWeight = 0f;
        foreach (GroundEnemyEntry entry in entries)
            totalWeight += entry.Weight;

        float roll = Random.value * totalWeight;
        foreach (GroundEnemyEntry entry in entries)
        {
            roll -= entry.Weight;
            if (roll <= 0f && entry.Weight > 0f)
                return entry.Prefab;
        }

        return null;
    }

    private bool TryGetSpawnPoint(float angle, out RaycastHit hit)
    {
        ZoneManager zoneManager = ZoneManager.Instance;
        float radius = Random.Range(zoneManager.InnerRadius + m_BarrierMargin, zoneManager.OuterRadius - m_BarrierMargin);
        Vector3 direction = PlayerRailFollower.Instance.GetDirectionAhead(angle);

        Vector3 origin = zoneManager.transform.position + direction * radius + Vector3.up * m_RaycastHeight;
        if (!Physics.Raycast(origin, Vector3.down, out hit, Mathf.Infinity, m_GroundMask, QueryTriggerInteraction.Ignore))
            return false;

        return !IsNearEnemy(hit.point);
    }

    private bool IsNearEnemy(Vector3 position)
    {
        foreach (EnemySpawnAnimation enemy in m_Enemies)
        {
            if (Vector3.Distance(enemy.transform.position, position) < m_MinSeparation)
                return true;
        }

        return false;
    }
}
