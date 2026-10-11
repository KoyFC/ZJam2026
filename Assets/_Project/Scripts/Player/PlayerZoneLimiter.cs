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

        float minDistance = ZoneManager.Instance.GetUnlockedRadius() + m_PlayerRadius;
        float minDistanceSquared = minDistance * minDistance;

        Vector3 offset = transform.position - m_CenterPosition;
        offset.y = 0f;

        if (offset.sqrMagnitude >= minDistanceSquared)
            return;

        Vector3 direction = offset.sqrMagnitude > Mathf.Epsilon ? offset.normalized : Vector3.forward;

        Vector3 clamped = m_CenterPosition + direction * minDistance;
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
