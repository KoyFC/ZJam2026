using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyShooter))]
public class BossShield : MonoBehaviour
{
    [SerializeField] private GameObject m_Shield;
    [SerializeField, Min(0f)] private float m_ShieldCooldown = 5f;
    [SerializeField, Min(0f)] private float m_ShieldDuration = 3f;

    [Header("Reflections")]
    [Tooltip("How fast reflected missiles and bullets turn towards the player, in degrees per second")]
    [SerializeField, Min(0f)] private float m_HomingTurnSpeed = 45f;
    [SerializeField] private Projectile m_ReflectedMissile;
    [SerializeField, Min(1)] private int m_MissilePoolSize = 8;
    [SerializeField] private ParticleSystem m_ReflectedBullets;
    [SerializeField, Min(0f)] private float m_ReflectedBulletSpeed = 100f;

    private EnemyShooter m_Shooter;
    private Pool<Projectile> m_MissilePool;
    private ParticleSystem.Particle[] m_Particles = new ParticleSystem.Particle[0];

    public bool IsActive { get; private set; }

    private void Awake()
    {
        m_Shooter = GetComponent<EnemyShooter>();

        if (m_ReflectedMissile != null)
            m_MissilePool = PoolManager.Instance.GetPool(m_ReflectedMissile, m_MissilePoolSize);

        if (m_Shield == null)
        {
            Debug.LogError("BossShield requires a shield GameObject.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        SetShieldActive(false);
        StartCoroutine(ShieldCycle());
    }

    private void OnDisable()
    {
        if (m_Shield != null)
            SetShieldActive(false);
    }

    public bool Blocks(GameObject other)
    {
        return IsActive && other == m_Shield;
    }

    public void Reflect(List<ParticleCollisionEvent> collisions, int count)
    {
        if (m_ReflectedBullets == null || !PlayerRailFollower.InstanceExists)
            return;

        Vector3 target = PlayerRailFollower.Instance.transform.position;
        for (int i = 0; i < count; i++)
        {
            Vector3 position = collisions[i].intersection;
            ParticleSystem.EmitParams bullet = new ParticleSystem.EmitParams
            {
                position = position,
                velocity = (target - position).normalized * m_ReflectedBulletSpeed,
                applyShapeToPosition = false
            };
            m_ReflectedBullets.Emit(bullet, 1);
        }
    }

    // Swallows the player's missile and sends an enemy one back at them
    public bool TryReflect(Projectile projectile)
    {
        if (!IsActive || projectile == null || !projectile.IsPlayerProjectile)
            return false;

        if (m_MissilePool == null || !PlayerRailFollower.InstanceExists)
            return true;

        Transform player = PlayerRailFollower.Instance.transform;
        Vector3 position = projectile.transform.position;
        Projectile missile = m_MissilePool.Get(position, Quaternion.LookRotation(player.position - position));
        missile.Launch(m_MissilePool);
        missile.HomeIn(player, m_HomingTurnSpeed);
        return true;
    }

    // Steers reflected bullets towards the player, limited by the turn speed so they can be dodged
    private void LateUpdate()
    {
        if (m_ReflectedBullets == null || !PlayerRailFollower.InstanceExists)
            return;

        int count = m_ReflectedBullets.particleCount;
        if (count == 0)
            return;

        if (m_Particles.Length < count)
            m_Particles = new ParticleSystem.Particle[count];

        m_ReflectedBullets.GetParticles(m_Particles, count);
        Vector3 target = PlayerRailFollower.Instance.transform.position;
        float maxTurn = m_HomingTurnSpeed * Mathf.Deg2Rad * Time.deltaTime;
        for (int i = 0; i < count; i++)
        {
            Vector3 toTarget = target - m_Particles[i].position;
            m_Particles[i].velocity = Vector3.RotateTowards(m_Particles[i].velocity, toTarget, maxTurn, 0f);
        }

        m_ReflectedBullets.SetParticles(m_Particles, count);
    }

    private IEnumerator ShieldCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_ShieldCooldown);
            yield return new WaitUntil(CanActivate);
            SetShieldActive(true);
            yield return new WaitForSeconds(m_ShieldDuration);
            SetShieldActive(false);
        }
    }

    // Never raise the shield mid-attack or while still rising from the ground
    private bool CanActivate()
    {
        return m_Shooter.enabled && !m_Shooter.IsBusy;
    }

    private void SetShieldActive(bool isActive)
    {
        IsActive = isActive;
        m_Shield.SetActive(isActive);
        m_Shooter.IsSuspended = isActive;
    }
}
