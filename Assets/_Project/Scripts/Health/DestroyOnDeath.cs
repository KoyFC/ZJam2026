using UnityEngine;

public enum DeathAction
{
    Destroy,
    Disable
}

[RequireComponent(typeof(HealthController))]
public class DestroyOnDeath : MonoBehaviour
{
    [SerializeField] private Explosion m_DeathExplosion;
    [SerializeField] private DeathAction m_DeathAction = DeathAction.Destroy;

    private HealthController m_HealthController;

    private void Awake()
    {
        m_HealthController = GetComponent<HealthController>();
        m_HealthController.OnDestroyed += HandleDeath;
    }

    private void OnDestroy()
    {
        m_HealthController.OnDestroyed -= HandleDeath;
    }

    private void HandleDeath()
    {
        if (m_DeathExplosion != null)
        {
            Pool<Explosion> pool = PoolManager.Instance.GetPool(m_DeathExplosion);
            pool.Get(transform.position, Quaternion.identity).Launch(pool, false);
        }

        if (m_DeathAction == DeathAction.Destroy)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }
}
