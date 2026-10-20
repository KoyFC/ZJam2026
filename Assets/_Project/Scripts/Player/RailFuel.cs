using System;
using UnityEngine;

public class RailFuel : Singleton<RailFuel>
{
    public float TraveledAngle { get; private set; }
    public float Normalized => TraveledAngle / 360f;
    public bool IsStopped { get; private set; }
    public bool IsPaused { get; private set; }

    public event Action OnFuelEmpty;

    private HealthController m_Health;
    private Vector3 m_PreviousOffset;

    protected override void Awake()
    {
        base.Awake();
        m_Health = GetComponent<HealthController>();
    }

    private void Update()
    {
        if (IsStopped || !ZoneManager.InstanceExists)
            return;

        Vector3 offset = ZoneManager.Instance.GetOffsetFromCenter(transform.position);
        if (!IsPaused)
            TraveledAngle += Vector3.Angle(m_PreviousOffset, offset);
        m_PreviousOffset = offset;

        if (TraveledAngle >= 360f)
            HandleFuelDepleted();
    }

    public void Stop()
    {
        IsStopped = true;
    }

    public void SetPaused(bool paused)
    {
        IsPaused = paused;
    }

    private void HandleFuelDepleted()
    {
        TraveledAngle = 360f;
        Stop();
        OnFuelEmpty?.Invoke();

        if (m_Health != null)
            m_Health.ReceiveDamage(m_Health.MaxHealth);
    }
}
