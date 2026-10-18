using System;
using UnityEngine;

public class PlayerFiringController : MonoBehaviour
{
    [Header("Primary Fire")]
    [SerializeField] private ParticleSystem m_Bullets;
    [SerializeField] private float m_FireRate = 10f;
    [SerializeField] private float m_ParticleSpeed = 50f;

    [Header("Missile")]
    [SerializeField] private Transform m_MissileSpawn;
    [SerializeField] private Projectile m_MissilePrefab;
    [SerializeField] private int m_MissilePoolSize = 10;
    [SerializeField] private Explosion m_ExplosionPrefab;

    public static event Action OnFired;

    private Pool<Projectile> m_MissilePool;
    private Pool<Explosion> m_ExplosionPool;
    private float m_NextShotTime;

    private void Awake()
    {
        if (m_MissileSpawn == null)
            m_MissileSpawn = transform;

        if (m_MissilePrefab != null)
        {
            Transform container = new GameObject("Missile Pool").transform;
            m_MissilePool = new Pool<Projectile>(m_MissilePrefab, container, m_MissilePoolSize);
        }

        if (m_ExplosionPrefab != null)
            m_ExplosionPool = PoolManager.Instance.GetPool(m_ExplosionPrefab, m_MissilePoolSize);

        if (m_Bullets != null)
        {
            var main = m_Bullets.main;
            main.startSpeed = m_ParticleSpeed;
        }
    }

    private void OnEnable()
    {
        PlayerInputController.OnSecondaryFire += FireMissile;
    }

    private void OnDisable()
    {
        PlayerInputController.OnSecondaryFire -= FireMissile;
    }

    private void Update()
    {
        if (!PlayerInputController.InstanceExists || !PlayerInputController.Instance.IsPrimaryFireHeld)
            return;

        if (Time.time >= m_NextShotTime)
            FireBullet();
    }

    private void FireBullet()
    {
        if (m_Bullets == null)
            return;

        m_NextShotTime = Time.time + 1f / m_FireRate;

        Transform muzzle = m_Bullets.transform;
        muzzle.rotation = GetAimRotation(muzzle);
        m_Bullets.Emit(1);

        OnFired?.Invoke();
    }

    private void FireMissile()
    {
        if (m_MissilePool == null)
            return;

        Projectile missile = m_MissilePool.Get(m_MissileSpawn.position, GetAimRotation(m_MissileSpawn));
        missile.Launch(m_MissilePool, m_ExplosionPool);

        OnFired?.Invoke();
    }

    private Quaternion GetAimRotation(Transform origin)
    {
        return PlayerCrosshair.InstanceExists
            ? PlayerCrosshair.Instance.GetAimRotation(origin.position)
            : origin.rotation;
    }
}
