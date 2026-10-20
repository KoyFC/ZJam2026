using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    [SerializeField] private Vector3 m_Axis = Vector3.up;
    [Tooltip("Degrees per second.")]
    [SerializeField] private float m_Speed = 5f;
    [SerializeField] private Space m_Space = Space.Self;

    private void Update()
    {
        transform.Rotate(m_Axis, m_Speed * Time.deltaTime, m_Space);
    }
}
