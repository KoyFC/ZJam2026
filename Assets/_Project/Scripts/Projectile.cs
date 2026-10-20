using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Speed => m_Speed;

    [SerializeField] private float m_Speed = 10f;
    [SerializeField] private float m_Lifetime = 5f;

    [Header("Damage")]
    [SerializeField] private float m_Damage = 10f;
    [SerializeField] private float m_HitRadius = 0.5f;
    [SerializeField] private LayerMask m_HitMask;

    private ParticleSystem m_ParticleSystem;
    private Pool<Projectile> m_Pool;
    private Pool<Explosion> m_ExplosionPool;
    private LayerMask m_DefaultHitMask;
    private float m_DefaultDamage;
    private float m_Age;

    public bool IsPlayerProjectile
    {
        get
        {
            int enemyLayer = LayerMask.NameToLayer("Enemy");
            return enemyLayer >= 0 && (m_HitMask.value & (1 << enemyLayer)) != 0;
        }
    }

    private void Awake()
    {
        m_ParticleSystem = GetComponentInChildren<ParticleSystem>();
        m_DefaultHitMask = m_HitMask;
        m_DefaultDamage = m_Damage;
    }

    private void OnEnable()
    {
        m_Age = 0f;
        m_HitMask = m_DefaultHitMask;
        m_Damage = m_DefaultDamage;

        if (m_ParticleSystem != null)
        {
            m_ParticleSystem.Clear();
            m_ParticleSystem.Play();
        }
    }

    private void OnDisable()
    {
        if (m_ParticleSystem != null)
            m_ParticleSystem.Stop();
    }

    public void Launch(Pool<Projectile> pool, Pool<Explosion> explosionPool = null)
    {
        m_Pool = pool;
        m_ExplosionPool = explosionPool;
    }

    public bool Deflect(Transform target, LayerMask hitMask)
    {
        Vector3 direction = target.position - transform.position;
        if (direction.sqrMagnitude <= Mathf.Epsilon)
        {
            return false;
        }

        transform.rotation = Quaternion.LookRotation(direction.normalized);
        m_HitMask = hitMask;
        m_Age = 0f;
        return true;
    }

    public void Release()
    {
        if (m_Pool != null)
            m_Pool.Release(this);
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        float distance = m_Speed * Time.deltaTime;

        if (Physics.SphereCast(transform.position, m_HitRadius, transform.forward, out RaycastHit hit, distance, m_HitMask))
        {
            transform.position += transform.forward * hit.distance;
            Hit(hit.collider);
            return;
        }

        transform.position += transform.forward * distance;

        m_Age += Time.deltaTime;
        if (m_Age >= m_Lifetime)
            Release();
    }

    private void Hit(Collider target)
    {
        ShieldBoss shield = target.GetComponentInParent<ShieldBoss>();
        if (shield != null && shield.TryDeflect(this))
        {
            return;
        }

        IDamageable damageable = target.GetComponentInParent<IDamageable>();
        if (damageable != null)
            damageable.ReceiveDamage(m_Damage);

        if (m_ExplosionPool != null)
            m_ExplosionPool.Get(transform.position, Quaternion.identity).Launch(m_ExplosionPool);

        Release();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_HitRadius);
    }
}
