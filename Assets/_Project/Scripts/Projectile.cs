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
    private Transform m_HomingTarget;
    private float m_HomingTurnSpeed;
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
        m_DefaultDamage = m_Damage;
    }

    private void OnEnable()
    {
        m_Age = 0f;
        m_Damage = m_DefaultDamage;
        m_HomingTarget = null;

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

    public void HomeIn(Transform target, float turnSpeed)
    {
        m_HomingTarget = target;
        m_HomingTurnSpeed = turnSpeed;
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
        if (m_HomingTarget != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(m_HomingTarget.position - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, m_HomingTurnSpeed * Time.deltaTime);
        }

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
        BossShield shield = target.GetComponentInParent<BossShield>();
        if (shield != null && shield.TryReflect(this))
        {
            Release();
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
