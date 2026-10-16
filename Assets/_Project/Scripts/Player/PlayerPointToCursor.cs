using UnityEngine;

public class PlayerPointToCursor : MonoBehaviour
{
    private Camera m_Camera;
    [SerializeField] private Transform m_Transform;
    [SerializeField] private float m_TurnSpeed = 10f;
    [SerializeField] private Vector2 m_AimStrength = new Vector2(0.6f, 0.4f);

    private void Awake()
    {
        if (m_Transform == null)
            m_Transform = transform;

        m_Camera = Camera.main;
    }

    private void Update()
    {
        if (!PlayerCrosshair.InstanceExists)
            return;

        Vector3 direction = SoftenDirection(PlayerCrosshair.Instance.AimPoint - m_Transform.position);
        if (direction.sqrMagnitude < Mathf.Epsilon)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction, m_Camera.transform.up);
        float lerp = 1f - Mathf.Exp(-m_TurnSpeed * Time.deltaTime);
        m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, targetRotation, lerp);
    }

    private Vector3 SoftenDirection(Vector3 direction)
    {
        Transform cameraTransform = m_Camera.transform;
        Vector3 local = cameraTransform.InverseTransformDirection(direction);
        local.x *= m_AimStrength.x;
        local.y *= m_AimStrength.y;
        return cameraTransform.TransformDirection(local);
    }

    private void OnDrawGizmos()
    {
        if (!PlayerCrosshair.InstanceExists)
            return;

        Vector3 aimPoint = PlayerCrosshair.Instance.AimPoint;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(m_Transform.position, aimPoint);
        Gizmos.DrawWireSphere(aimPoint, 0.5f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(m_Transform.position, m_Transform.forward * 5f);
    }
}
