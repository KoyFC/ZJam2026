using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController m_CharacterController;
    [SerializeField] private PlayerInputController m_InputController;

    [SerializeField] private float m_MoveSpeed = 5f;

    private void Awake()
    {
        if (m_CharacterController == null)
            m_CharacterController = GetComponent<CharacterController>();

        if (m_InputController == null)
            m_InputController = GetComponent<PlayerInputController>();
    }

    private void Update()
    {
        Vector2 moveInput = m_InputController.MoveInput;
        Vector3 moveDirection = new Vector3(moveInput.x, moveInput.y, 0f);

        // Move relative to transform forward and right
        Vector3 worldMoveDirection = transform.TransformDirection(moveDirection);
        m_CharacterController.Move(worldMoveDirection * m_MoveSpeed * Time.deltaTime);
    }
}
