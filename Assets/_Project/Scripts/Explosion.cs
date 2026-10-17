using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float m_Damage = 25f;
    [SerializeField] private float m_Radius = 5f;
    [SerializeField] private LayerMask m_HitMask;
    [SerializeField] private float m_Lifetime = 2f;

    private static readonly Collider[] s_Hits = new Collider[32];
    private readonly HashSet<IDamageable> m_Damaged = new HashSet<IDamageable>();

    private ParticleSystem[] m_ParticleSystems;
    private Pool<Explosion> m_Pool;
    private float m_Age;

    private void Awake()
    {
        m_ParticleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    public void Launch(Pool<Explosion> pool)
    {
        m_Pool = pool;
        m_Age = 0f;

        foreach (ParticleSystem particles in m_ParticleSystems)
        {
            particles.Clear();
            particles.Play();
        }

        DealDamage();
    }

    private void DealDamage()
    {
        m_Damaged.Clear();

        int count = Physics.OverlapSphereNonAlloc(transform.position, m_Radius, s_Hits, m_HitMask);
        for (int i = 0; i < count; i++)
        {
            IDamageable damageable = s_Hits[i].GetComponentInParent<IDamageable>();
            if (damageable != null && m_Damaged.Add(damageable))
                damageable.ReceiveDamage(m_Damage);
        }
    }

    private void Update()
    {
        m_Age += Time.deltaTime;
        if (m_Age >= m_Lifetime)
            Release();
    }

    private void Release()
    {
        if (m_Pool != null)
            m_Pool.Release(this);
        else
            Destroy(gameObject);
    }
}
