using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyShooter))]
public class BossShield : MonoBehaviour
{
    [SerializeField] private GameObject m_Shield;
    [SerializeField, Min(0f)] private float m_ShieldCooldown = 5f;
    [SerializeField, Min(0f)] private float m_ShieldDuration = 3f;
    [SerializeField] private LayerMask m_ReflectedHitMask = 64;

    private EnemyShooter m_Shooter;

    public bool IsActive { get; private set; }

    private void Awake()
    {
        m_Shooter = GetComponent<EnemyShooter>();

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

    public bool TryDeflect(Projectile projectile)
    {
        if (!IsActive || projectile == null || !projectile.IsPlayerProjectile || !PlayerRailFollower.InstanceExists)
        {
            return false;
        }

        return projectile.Deflect(PlayerRailFollower.Instance.transform, m_ReflectedHitMask);
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
