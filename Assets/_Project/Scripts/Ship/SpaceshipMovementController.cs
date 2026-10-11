using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class ShipMovementController : MonoBehaviour
{
    [Header("References")]
    public PlayerInput m_playerInput;
    public Rigidbody m_rigidbody;

    [Header("Movement")]
    [SerializeField] private float maxForwardSpeed = 18f;
    [SerializeField] private float maxBackwardSpeed = 8f;
    [SerializeField] private float maxStrafeSpeed = 10f;

    [SerializeField] private float idleForwardSpeed = 3f;

    [SerializeField] private float acceleration = 18f;
    [SerializeField] private float deceleration = 8f;
    [SerializeField] private float strafeAcceleration = 22f;

    [Header("Rotation")]
    [SerializeField] private float mouseSensitivity = 0.12f;
    [SerializeField] private float rotationSmoothness = 15f;

    private Vector2 _movementInput;
    private Vector2 _lookInput;

    private bool _isFiring;

    private float _targetYaw;


    private void Awake()
    {
        m_playerInput = GetComponent<PlayerInput>();
        m_rigidbody = GetComponent<Rigidbody>();

        m_rigidbody.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;

        _targetYaw = transform.eulerAngles.y;
    }


    private void Update()
    {
        InputUpdate();
        HandleRotationInput();
    }


    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }


    private void InputUpdate()
    {
        if (m_playerInput == null)
            return;

        InputAction moveAction = m_playerInput.actions["Move"];
        InputAction lookAction = m_playerInput.actions["Look"];
        InputAction attackAction = m_playerInput.actions["Attack"];

        if (moveAction != null)
            _movementInput = Vector2.ClampMagnitude(
                moveAction.ReadValue<Vector2>(),
                1f
            );

        if (lookAction != null)
            _lookInput = lookAction.ReadValue<Vector2>();

        if (attackAction != null)
            _isFiring = attackAction.IsPressed();
    }


    private void HandleMovement()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(
            m_rigidbody.linearVelocity
        );

        float targetForwardSpeed;
        float targetStrafeSpeed = _movementInput.x * maxStrafeSpeed;

        if (_movementInput.y > 0f)
        {
            targetForwardSpeed = Mathf.Lerp(
                idleForwardSpeed,
                maxForwardSpeed,
                _movementInput.y
            );
        }
        else if (_movementInput.y < 0f)
        {
            targetForwardSpeed = Mathf.Lerp(
                idleForwardSpeed,
                -maxBackwardSpeed,
                -_movementInput.y
            );
        }
        else
        {
            targetForwardSpeed = idleForwardSpeed;
        }

        float forwardAcceleration =
            Mathf.Abs(targetForwardSpeed) > Mathf.Abs(localVelocity.z)
                ? acceleration
                : deceleration;

        float newForwardSpeed = Mathf.MoveTowards(
            localVelocity.z,
            targetForwardSpeed,
            forwardAcceleration * Time.fixedDeltaTime
        );

        float newStrafeSpeed = Mathf.MoveTowards(
            localVelocity.x,
            targetStrafeSpeed,
            strafeAcceleration * Time.fixedDeltaTime
        );

        Vector3 newLocalVelocity = new Vector3(
            newStrafeSpeed,
            localVelocity.y,
            newForwardSpeed
        );

        m_rigidbody.linearVelocity =
            transform.TransformDirection(newLocalVelocity);
    }


    private void HandleRotationInput()
    {
        _targetYaw += _lookInput.x * mouseSensitivity;
    }


    private void HandleRotation()
    {
        Quaternion targetRotation = Quaternion.Euler(
            0f,
            _targetYaw,
            0f
        );

        Quaternion newRotation = Quaternion.Slerp(
            m_rigidbody.rotation,
            targetRotation,
            rotationSmoothness * Time.fixedDeltaTime
        );

        m_rigidbody.MoveRotation(newRotation);
    }
}