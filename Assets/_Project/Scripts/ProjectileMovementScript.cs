using UnityEngine;

public class ProjectileMovementScript : MonoBehaviour
{
    [SerializeField] private float m_Speed = 10f;
    private ParticleSystem m_ParticleSystem;

    private void Awake()
    {
        m_ParticleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void OnEnable()
    {
        if (m_ParticleSystem != null)
            m_ParticleSystem.Play();
    }

    private void OnDisable()
    {
        if (m_ParticleSystem != null)
            m_ParticleSystem.Stop();
    }

    void Update()
    {
        transform.position += transform.forward * m_Speed * Time.deltaTime;
    }
}
