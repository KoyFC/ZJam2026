using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [SerializeField] private float m_Lifetime = 0.3f;

    private ParticleSystem[] m_ParticleSystems;
    private Pool<HitEffect> m_Pool;
    private float m_Age;

    private void Awake()
    {
        m_ParticleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    public void Play(Pool<HitEffect> pool)
    {
        m_Pool = pool;
        m_Age = 0f;

        foreach (ParticleSystem particles in m_ParticleSystems)
        {
            particles.Clear();
            particles.Play();
        }
    }

    private void Update()
    {
        m_Age += Time.deltaTime;
        if (m_Age >= m_Lifetime)
            Release();
    }

    private void Release()
    {
        if (m_Pool != null)
            m_Pool.Release(this);
        else
            Destroy(gameObject);
    }
}
