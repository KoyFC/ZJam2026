using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPointToCursor : MonoBehaviour
{
    [SerializeField] private Transform m_Transform;
    [SerializeField] private Camera m_Camera;
    [SerializeField] private float m_AimDistance = 200f;
    [SerializeField] private float m_TurnSpeed = 10f;

    private Vector3 m_AimPoint;
    private bool m_HasAimPoint;

    private void Awake()
    {
        if (m_Transform == null)
            m_Transform = transform;

        if (m_Camera == null)
            m_Camera = Camera.main;
    }

    private void Update()
    {
        m_HasAimPoint = Mouse.current != null && TryGetAimPoint(out m_AimPoint);
        if (!m_HasAimPoint)
            return;

        Vector3 direction = m_AimPoint - m_Transform.position;
        if (direction.sqrMagnitude < Mathf.Epsilon)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction, m_Camera.transform.up);
        float lerp = 1f - Mathf.Exp(-m_TurnSpeed * Time.deltaTime);
        m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, targetRotation, lerp);
    }

    private bool TryGetAimPoint(out Vector3 aimPoint)
    {
        Transform cameraTransform = m_Camera.transform;
        Vector3 forward = cameraTransform.forward;
        Plane aimPlane = new Plane(-forward, cameraTransform.position + forward * m_AimDistance);
        Ray ray = m_Camera.ScreenPointToRay(Mouse.current.position.ReadValue());

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
