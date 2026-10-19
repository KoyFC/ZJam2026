using UnityEngine;

public class PlayerZoneLimiter : MonoBehaviour
{
    [SerializeField] private float m_PlayerRadius = 0.5f;
    [SerializeField] private float m_MinYPosition = 10f;
    [SerializeField] private float m_MaxYPosition = 50f;
    private Vector3 m_CenterPosition = Vector3.zero;
    public float MinYPosition => m_MinYPosition;
    public float MaxYPosition => m_MaxYPosition;
    private Rigidbody m_Rigidbody;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (!ZoneManager.InstanceExists)
            return;

        m_CenterPosition = ZoneManager.Instance.transform.position;
    }

    private void FixedUpdate()
    {
        if (m_Rigidbody == null || !ZoneManager.InstanceExists)
            return;

        if (!TryClampToZone(m_Rigidbody.position, out Vector3 clampedPosition, out Vector3 barrierNormal))
            return;

        m_Rigidbody.position = clampedPosition;

        Vector3 velocity = m_Rigidbody.linearVelocity;
        float velocityIntoBarrier = Vector3.Dot(velocity, -barrierNormal);
        if (velocityIntoBarrier > 0f)
            m_Rigidbody.linearVelocity = velocity + barrierNormal * velocityIntoBarrier;
    }

    private void LateUpdate()
    {
        if (m_Rigidbody != null || !ZoneManager.InstanceExists)
            return;

        if (!TryClampToZone(transform.position, out Vector3 clampedPosition, out _))
            return;

        transform.position = clampedPosition;
        Physics.SyncTransforms();
    }

    private bool TryClampToZone(Vector3 position, out Vector3 clampedPosition, out Vector3 barrierNormal)
    {
        clampedPosition = position;
        barrierNormal = Vector3.zero;

        float maxAllowedDistance = Mathf.Max(0f, ZoneManager.Instance.OuterRadius - m_PlayerRadius);
        float innerRadius = ZoneManager.Instance.InnerRadius;
        float minAllowedDistance = innerRadius > 0f ? Mathf.Min(innerRadius + m_PlayerRadius, maxAllowedDistance) : 0f;

        Vector3 horizontalOffset = position - m_CenterPosition;
        horizontalOffset.y = 0f;

        float distanceFromCenter = horizontalOffset.magnitude;
        float clampedY = Mathf.Clamp(position.y, m_MinYPosition, m_MaxYPosition);
        bool insideRing = distanceFromCenter >= minAllowedDistance && distanceFromCenter <= maxAllowedDistance;

        if (insideRing)
        {
            if (Mathf.Approximately(clampedY, position.y))
                return false;

            clampedPosition.y = clampedY;
            barrierNormal = position.y > m_MaxYPosition ? Vector3.down : Vector3.up;
            return true;
        }

        Vector3 directionFromCenter = distanceFromCenter > Mathf.Epsilon ? horizontalOffset / distanceFromCenter : Vector3.forward;
        float clampedDistanceFromCenter = Mathf.Clamp(distanceFromCenter, minAllowedDistance, maxAllowedDistance);

        clampedPosition = m_CenterPosition + directionFromCenter * clampedDistanceFromCenter;
        clampedPosition.y = clampedY;

        barrierNormal = distanceFromCenter > maxAllowedDistance ? -directionFromCenter : directionFromCenter;
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_CenterPosition, m_PlayerRadius);
    }
}
