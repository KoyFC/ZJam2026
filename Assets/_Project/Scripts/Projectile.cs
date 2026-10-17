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
    private float m_Age;

    private void Awake()
    {
        m_ParticleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void OnEnable()
    {
        m_Age = 0f;

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

    public void Launch(Pool<Projectile> pool)
    {
        m_Pool = pool;
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
        IDamageable damageable = target.GetComponentInParent<IDamageable>();
        if (damageable != null)
            damageable.ReceiveDamage(m_Damage);

        Release();
    }
}
