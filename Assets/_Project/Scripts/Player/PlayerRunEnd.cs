using System;
using System.Collections;
using UnityEngine;

public class PlayerRunEnd : MonoBehaviour
{
    public static event Action OnRunEnded;

    [SerializeField] private ParticleSystem[] m_StopParticles;
    [SerializeField] private Behaviour[] m_DisableBehaviours;

    public bool HasEnded { get; private set; }

    private IEnumerator Start()
    {
        yield return new WaitWhile(() => SceneLoader.IsBusy);
        RunStats.Begin();
    }

    public void End()
    {
        if (HasEnded)
            return;

        HasEnded = true;
        RunStats.End();

        foreach (Behaviour behaviour in m_DisableBehaviours)
            behaviour.enabled = false;

        foreach (ParticleSystem particles in m_StopParticles)
            particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        if (PlayerInputController.InstanceExists)
            PlayerInputController.Instance.SetInputActive(false);

        if (RailFuel.InstanceExists)
            RailFuel.Instance.Stop();

        if (WaveSpawner.InstanceExists)
            WaveSpawner.Instance.Stop();

        OnRunEnded?.Invoke();
    }
}
