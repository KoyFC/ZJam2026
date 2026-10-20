using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleDamage : MonoBehaviour
{
    [SerializeField] private float m_Damage = 2f;

    private readonly List<ParticleCollisionEvent> m_CollisionEvents = new List<ParticleCollisionEvent>();
    private ParticleSystem m_ParticleSystem;

    private void Awake()
    {
        m_ParticleSystem = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other)
    {
        int hits = m_ParticleSystem.GetCollisionEvents(other, m_CollisionEvents);

        BossShield shield = other.GetComponentInParent<BossShield>();
        if (shield != null && shield.Blocks(other))
        {
            shield.Reflect(m_CollisionEvents, hits);
            return;
        }

        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable == null)
            return;

        damageable.ReceiveDamage(m_Damage * hits);
    }
}
