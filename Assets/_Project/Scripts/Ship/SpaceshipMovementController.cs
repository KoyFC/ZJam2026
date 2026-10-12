using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class ShipMovementController : MonoBehaviour
{
    [Header("References")]
    public PlayerInput m_playerInput;
    public Rigidbody m_rigidBody;

    [Header("Movement Settings")]
    public float m_maxForwardSpeed = 10f;
    public float m_maxBackwardSpeed = 5f;
    public float m_maxStrafeSpeed = 7f;

    public float m_acceleration = 5f;
    public float m_deceleration = 5f;

    [Header("Rotation Settings")]
    public float m_maxRotationSpeed = 180f;
    public float m_rotationAcceleration = 720f;
    public float m_rotationDeceleration = 1080f;

    private Vector2 m_movementInput;
    private Vector2 m_rotationInput;

    private void Awake()
    {
        m_playerInput = GetComponent<PlayerInput>();
        m_rigidBody = GetComponent<Rigidbody>();
        m_rigidBody.constraints |= RigidbodyConstraints.FreezePositionY |
                                  RigidbodyConstraints.FreezeRotationX |
                                  RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        m_movementInput = m_playerInput.actions["Move"].ReadValue<Vector2>();
        m_rotationInput = m_playerInput.actions["Look"].ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        Vector2 input = Vector2.ClampMagnitude(m_movementInput, 1f);
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
        float forwardSpeed = input.y >= 0f ? m_maxForwardSpeed : m_maxBackwardSpeed;
        Vector3 targetVelocity = (right * input.x * m_maxStrafeSpeed) +
                                 (forward * input.y * forwardSpeed);
        Vector3 planarVelocity = Vector3.ProjectOnPlane(m_rigidBody.linearVelocity, Vector3.up);
        float acceleration = input.sqrMagnitude > 0.001f ? m_acceleration : m_deceleration;

        m_rigidBody.linearVelocity = Vector3.MoveTowards(
            planarVelocity,
            targetVelocity,
            acceleration * Time.fixedDeltaTime);
    }

    private void HandleRotation()
    {
        float rotationInput = Mathf.Clamp(m_rotationInput.x, -1f, 1f);
        float targetAngularSpeed = rotationInput * m_maxRotationSpeed * Mathf.Deg2Rad;
        float angularAcceleration = Mathf.Abs(rotationInput) > 0.01f
            ? m_rotationAcceleration
            : m_rotationDeceleration;
        float yawSpeed = Mathf.MoveTowards(
            m_rigidBody.angularVelocity.y,
            targetAngularSpeed,
            angularAcceleration * Mathf.Deg2Rad * Time.fixedDeltaTime);

        m_rigidBody.angularVelocity = new Vector3(0f, yawSpeed, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * m_movementInput.y * 5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * m_movementInput.x * 5f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (m_rigidBody != null ? m_rigidBody.linearVelocity.normalized : Vector3.zero));
    }

}