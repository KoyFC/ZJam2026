using UnityEngine;

[RequireComponent(typeof(HealthController))]
public class ResourceDropper : MonoBehaviour
{
    [SerializeField] private ResourceDrop m_Drop;
    [SerializeField, Range(0f, 1f)] private float m_DropChance = 1f;
    [SerializeField, Range(0f, 100f)] private float m_HealthDropPercentage = 50f;
    [SerializeField, Min(0f)] private float m_HealthAmount = 25f;
    [SerializeField, Min(0f)] private float m_FuelAmount = 45f;

    [Header("Rise")]
    [SerializeField] private bool m_RiseOnDrop;
    [SerializeField] private float m_MinRiseHeight = 15f;
    [SerializeField] private float m_MaxRiseHeight = 20f;

    private HealthController m_HealthController;

    private void Awake()
    {
        m_HealthController = GetComponent<HealthController>();
        m_HealthController.OnDestroyed += Drop;
    }

    private void OnDestroy()
    {
        m_HealthController.OnDestroyed -= Drop;
    }

    private void Drop()
    {
        if (m_Drop == null || Random.value >= m_DropChance)
            return;

        Vector3 position = transform.position;
        float height = m_RiseOnDrop ? Random.Range(m_MinRiseHeight, m_MaxRiseHeight) : position.y;
        ResourceDropType type = IsHealthDrop() ? ResourceDropType.Health : ResourceDropType.Fuel;
        float amount = type == ResourceDropType.Health ? m_HealthAmount : m_FuelAmount;

        Pool<ResourceDrop> pool = PoolManager.Instance.GetPool(m_Drop);
        pool.Get(position, Quaternion.identity).Launch(pool, type, amount, ClampToPlayerHeight(height));
    }

    private bool IsHealthDrop()
    {
        if (m_HealthDropPercentage <= 0f)
            return false;
        if (m_HealthDropPercentage >= 100f)
            return true;

        return Random.value * 100f < m_HealthDropPercentage;
    }

    private static float ClampToPlayerHeight(float height)
    {
        if (!PlayerRailFollower.InstanceExists)
            return height;

        PlayerZoneLimiter zoneLimiter = PlayerRailFollower.Instance.GetComponent<PlayerZoneLimiter>();
        return Mathf.Clamp(height, zoneLimiter.MinYPosition, zoneLimiter.MaxYPosition);
    }

    private void OnValidate()
    {
        if (m_MinRiseHeight > m_MaxRiseHeight)
            Debug.LogWarning($"{name}: Min Rise Height is greater than Max Rise Height.", this);
    }
}
