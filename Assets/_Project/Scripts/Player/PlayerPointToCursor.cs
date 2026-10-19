using UnityEngine;

public class PlayerPointToCursor : MonoBehaviour
{
    private Camera m_Camera;
    [SerializeField] private Transform m_Transform;
    [SerializeField] private float m_TurnSpeed = 10f;
    [SerializeField] private Vector2 m_AimStrength = new Vector2(0.6f, 0.4f);
    [SerializeField, Range(0f, 90f)] private float m_MaxAimAngle = 90f;

    private void Awake()
    {
        if (m_Transform == null)
            m_Transform = transform;
    }

    private void Update()
    {
        if (!PlayerCrosshair.InstanceExists || !HasCamera())
            return;

        Vector3 direction = SoftenDirection(PlayerCrosshair.Instance.AimPoint - m_Transform.position);
        if (direction.sqrMagnitude < Mathf.Epsilon)
            return;

        Vector3 forward = m_Camera.transform.forward;
        if (Vector3.Angle(forward, direction) > m_MaxAimAngle)
            direction = Vector3.RotateTowards(forward, direction, m_MaxAimAngle * Mathf.Deg2Rad, 0f);

        Quaternion targetRotation = Quaternion.LookRotation(direction, m_Camera.transform.up);
        float lerp = 1f - Mathf.Exp(-m_TurnSpeed * Time.deltaTime);
        m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, targetRotation, lerp);
    }

    private bool HasCamera()
    {
        if (m_Camera == null)
            m_Camera = Camera.main;

        return m_Camera != null;
    }

    private Vector3 SoftenDirection(Vector3 direction)
    {
        Transform cameraTransform = m_Camera.transform;
        Vector3 local = cameraTransform.InverseTransformDirection(direction);
        local.x *= m_AimStrength.x;
        local.y *= m_AimStrength.y;
        return cameraTransform.TransformDirection(local);
    }
}
