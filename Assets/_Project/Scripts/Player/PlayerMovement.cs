using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController m_CharacterController;
    [SerializeField] private PlayerInputController m_InputController;

    [SerializeField] private Vector2 m_MoveSpeed = new Vector2(10f, 5f);

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
        Vector3 localVelocity = new Vector3(moveInput.x * m_MoveSpeed.x, moveInput.y * m_MoveSpeed.y, 0f);

        // Move relative to transform right and up
        Vector3 worldVelocity = transform.TransformDirection(localVelocity);
        m_CharacterController.Move(worldVelocity * Time.deltaTime);
    }
}
