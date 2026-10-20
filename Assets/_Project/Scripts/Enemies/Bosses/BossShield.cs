using System.Collections;
using UnityEngine;

public class BossShield : MonoBehaviour
{
    [SerializeField] private GameObject m_Shield;
    [SerializeField, Min(0f)] private float m_ShieldCooldown = 5f;
    [SerializeField, Min(0f)] private float m_ShieldDuration = 3f;
    [SerializeField] private LayerMask m_ReflectedHitMask = 64;

    private bool m_IsShieldActive;

    private void Start()
    {
        if (m_Shield == null)
        {
            Debug.LogError("BossShield requires a shield GameObject.", this);
            enabled = false;
            return;
        }

        SetShieldActive(false);
        StartCoroutine(ShieldCycle());
    }

    private void OnDisable()
    {
        if (m_Shield != null)
        {
            SetShieldActive(false);
        }
    }

    public bool TryDeflect(Projectile projectile)
    {
        if (!m_IsShieldActive || projectile == null || !projectile.IsPlayerProjectile || !PlayerRailFollower.InstanceExists)
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
            SetShieldActive(true);
            yield return new WaitForSeconds(m_ShieldDuration);
            SetShieldActive(false);
        }
    }

    private void SetShieldActive(bool isActive)
    {
        m_IsShieldActive = isActive;
        m_Shield.SetActive(isActive);
    }
}
