using UnityEngine;

public class PlayerZoneLimiter : MonoBehaviour
{
    [SerializeField] private float m_PlayerRadius = 0.5f;
    private Vector3 m_CenterPosition = Vector3.zero;

    private void Start()
    {
        if (!ZoneManager.InstanceExists)
            return;

        m_CenterPosition = ZoneManager.Instance.transform.position;
    }

    private void LateUpdate()
    {
        if (!ZoneManager.InstanceExists)
            return;

        float maxDistance = Mathf.Max(0f, ZoneManager.Instance.GetOuterRadius() - m_PlayerRadius);
        float innerRadius = ZoneManager.Instance.GetInnerRadius();
        float minDistance = innerRadius > 0f ? Mathf.Min(innerRadius + m_PlayerRadius, maxDistance) : 0f;

        Vector3 offset = transform.position - m_CenterPosition;
        offset.y = 0f;

        float distance = offset.magnitude;
        if (distance >= minDistance && distance <= maxDistance)
            return;

        Vector3 direction = distance > Mathf.Epsilon ? offset / distance : Vector3.forward;
        float clampedDistance = Mathf.Clamp(distance, minDistance, maxDistance);

        Vector3 clamped = m_CenterPosition + direction * clampedDistance;
        clamped.y = transform.position.y;

        transform.position = clamped;
        Physics.SyncTransforms();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_CenterPosition, m_PlayerRadius);
    }
}
