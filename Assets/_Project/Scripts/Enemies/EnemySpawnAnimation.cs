using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(EnemyShooter))]
public class EnemySpawnAnimation : MonoBehaviour
{
    [SerializeField] private float m_SpawnDepth = -5f;
    [SerializeField, Min(0.1f)] private float m_RiseSpeed = 10f;
    [SerializeField] private Ease m_SpawnEase = Ease.OutBack;
    [SerializeField] private Ease m_DespawnEase = Ease.InBack;

    public bool IsRetreating { get; private set; }

    public event Action<EnemySpawnAnimation> OnDespawn;

    private EnemyShooter m_Shooter;

    private void Awake()
    {
        m_Shooter = GetComponent<EnemyShooter>();
    }

    private void OnDisable()
    {
        transform.DOKill();
    }

    public void Spawn(Vector3 targetPosition)
    {
        IsRetreating = false;
        m_Shooter.enabled = false;

        transform.DOKill();
        transform.position = new Vector3(targetPosition.x, m_SpawnDepth, targetPosition.z);
        transform.DOMoveY(targetPosition.y, m_RiseSpeed).SetSpeedBased().SetEase(m_SpawnEase).OnComplete(() =>
        {
            m_Shooter.enabled = true;
        });
    }

    [ContextMenu("Arise")]
    private void SpawnFromGround()
    {
        Spawn(transform.position);
    }

    [ContextMenu("Despawn")]
    public void Despawn()
    {
        if (IsRetreating)
            return;

        IsRetreating = true;
        m_Shooter.enabled = false;

        transform.DOKill();
        transform.DOMoveY(m_SpawnDepth, m_RiseSpeed).SetSpeedBased().SetEase(m_DespawnEase).OnComplete(() =>
        {
            OnDespawn?.Invoke(this);
        });
    }
}
