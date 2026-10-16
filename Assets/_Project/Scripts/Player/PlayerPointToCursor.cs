using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPointToCursor : MonoBehaviour
{
    private Camera m_Camera;
    [SerializeField] private Transform m_Transform;
    [SerializeField] private float m_AimDistance = 200f;
    [SerializeField] private float m_TurnSpeed = 10f;
    [SerializeField] private Vector2 m_AimStrength = new Vector2(0.6f, 0.4f);

    private Vector3 m_AimPoint;
    private bool m_HasAimPoint;

    private void Awake()
    {
        if (m_Transform == null)
            m_Transform = transform;

        m_Camera = Camera.main;
    }

    private void Update()
    {
        m_HasAimPoint = TryGetAimPoint(GetAimRay(), out m_AimPoint);
        if (!m_HasAimPoint)
            return;

        Vector3 direction = SoftenDirection(m_AimPoint - m_Transform.position);
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

    private Ray GetAimRay()
    {
        if (PlayerCrosshair.InstanceExists)
            return PlayerCrosshair.Instance.FireRay;

        if (Mouse.current == null)
            return new Ray(m_Camera.transform.position, m_Camera.transform.forward);

        return m_Camera.ScreenPointToRay(Mouse.current.position.ReadValue());
    }

    private bool TryGetAimPoint(Ray ray, out Vector3 aimPoint)
    {
        Transform cameraTransform = m_Camera.transform;
        Vector3 forward = cameraTransform.forward;
        Plane aimPlane = new Plane(-forward, cameraTransform.position + forward * m_AimDistance);

        bool hit = aimPlane.Raycast(ray, out float distance);
        aimPoint = ray.GetPoint(distance);
        return hit;
    }

    private void OnDrawGizmos()
    {
        if (!m_HasAimPoint)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(m_Transform.position, m_AimPoint);
        Gizmos.DrawWireSphere(m_AimPoint, 0.5f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(m_Transform.position, m_Transform.forward * 5f);
    }
}
