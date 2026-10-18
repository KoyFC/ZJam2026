using UnityEngine;

public class HitEffect : MonoBehaviour
{
    private ParticleSystem[] m_ParticleSystems;

    private void Awake()
    {
        m_ParticleSystems = GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem particles in m_ParticleSystems)
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void Play(Vector3 position)
    {
        transform.position = position;

        foreach (ParticleSystem particles in m_ParticleSystems)
        {
            var emission = particles.emission;
            int burstCount = emission.burstCount == 0 ? 1 : Mathf.RoundToInt(emission.GetBurst(0).count.constant);
            particles.Emit(burstCount);
        }
    }
}
