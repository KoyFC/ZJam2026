using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(HealthController))]
public class PlayerDeath : MonoBehaviour
{
    public event Action OnDied;

    [SerializeField] private Transform m_Ship;
    [SerializeField] private Explosion m_DeathExplosion;
    [SerializeField] private GameObject[] m_HideOnDeath;
    [SerializeField] private ParticleSystem[] m_StopOnDeath;
    [SerializeField] private Behaviour[] m_DisableOnDeath;

    [Header("Fall")]
    [SerializeField] private LayerMask m_GroundMask = 1;
    [SerializeField, Min(0f)] private float m_FallDuration = 1f;
    [SerializeField] private Ease m_FallEase = Ease.InQuad;
    [SerializeField, Min(0f)] private float m_FallbackDistance = 30f;

    private HealthController m_HealthController;

    private void Awake()
    {
        m_HealthController = GetComponent<HealthController>();
        m_HealthController.OnDestroyed += HandleDeath;
    }

    private void OnDestroy()
    {
        m_HealthController.OnDestroyed -= HandleDeath;
    }

    private void HandleDeath()
    {
        foreach (GameObject target in m_HideOnDeath)
            target.SetActive(false);

        foreach (Behaviour behaviour in m_DisableOnDeath)
            behaviour.enabled = false;

        foreach (ParticleSystem particles in m_StopOnDeath)
            particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        if (PlayerInputController.InstanceExists)
            PlayerInputController.Instance.SetInputActive(false);

        if (RailFuel.InstanceExists)
            RailFuel.Instance.Stop();

        if (WaveSpawner.InstanceExists)
            WaveSpawner.Instance.Stop();

        Fall();
    }

    private void Fall()
    {
        Vector3 impactPoint = GetImpactPoint();
        Vector3 heading = Vector3.ProjectOnPlane(m_Ship.forward, Vector3.up);
        if (heading.sqrMagnitude < Mathf.Epsilon)
            heading = Vector3.forward;

        Quaternion facingDown = Quaternion.LookRotation(Vector3.down, heading);

        DOTween.Sequence()
            .Join(transform.DOMove(impactPoint, m_FallDuration).SetEase(m_FallEase))
            .Join(m_Ship.DORotateQuaternion(facingDown, m_FallDuration).SetEase(m_FallEase))
            .OnComplete(Impact)
            .SetLink(gameObject);
    }

    private Vector3 GetImpactPoint()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, m_GroundMask, QueryTriggerInteraction.Ignore))
            return hit.point;

        return transform.position + Vector3.down * m_FallbackDistance;
    }

    private void Impact()
    {
        if (m_DeathExplosion != null)
        {
            Pool<Explosion> pool = PoolManager.Instance.GetPool(m_DeathExplosion);
            pool.Get(m_Ship.position, Quaternion.identity).Launch(pool, false);
        }

        m_Ship.gameObject.SetActive(false);
        OnDied?.Invoke();
    }
}
