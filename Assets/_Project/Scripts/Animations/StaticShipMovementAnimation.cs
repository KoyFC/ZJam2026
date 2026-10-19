using UnityEngine;

public class StaticShipMovementAnimation : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField, Min(0f)] private float m_PitchAmplitude = 3f;
    [SerializeField, Min(0f)] private float m_YawAmplitude = 2f;
    [SerializeField, Min(0f)] private float m_RollAmplitude = 8f;
    [SerializeField, Min(0.01f)] private float m_MinFrequency = 0.35f;
    [SerializeField, Min(0.01f)] private float m_MaxFrequency = 0.7f;

    private Quaternion m_InitialRotation;
    private Vector3 m_Frequency;
    private Vector3 m_Phase;
    private float m_AnimationTime;

    private void Awake()
    {
        m_InitialRotation = transform.localRotation;

        float minFrequency = Mathf.Min(m_MinFrequency, m_MaxFrequency);
        float maxFrequency = Mathf.Max(m_MinFrequency, m_MaxFrequency);
        m_Frequency = new Vector3(
            Random.Range(minFrequency, maxFrequency),
            Random.Range(minFrequency, maxFrequency),
            Random.Range(minFrequency, maxFrequency));
        m_Phase = new Vector3(
            Random.Range(0f, Mathf.PI * 2f),
            Random.Range(0f, Mathf.PI * 2f),
            Random.Range(0f, Mathf.PI * 2f));
    }

    private void Update()
    {
        m_AnimationTime += Time.deltaTime;

        Vector3 rotationOffset = new Vector3(
            Mathf.Sin(m_AnimationTime * m_Frequency.x + m_Phase.x) * m_PitchAmplitude,
            Mathf.Sin(m_AnimationTime * m_Frequency.y + m_Phase.y) * m_YawAmplitude,
            Mathf.Sin(m_AnimationTime * m_Frequency.z + m_Phase.z) * m_RollAmplitude);

        transform.localRotation = m_InitialRotation * Quaternion.Euler(rotationOffset);
    }
}
