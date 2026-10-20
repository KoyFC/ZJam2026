using System;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(PlayerRunEnd))]
public class PlayerVictory : MonoBehaviour
{
    public static event Action OnVictory;

    [SerializeField, Min(0f)] private float m_MoveDuration = 1.5f;
    [SerializeField] private Ease m_MoveEase = Ease.InOutSine;

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera m_GameplayCamera;
    [SerializeField] private CinemachineCamera m_VictoryCamera;

    private PlayerRunEnd m_RunEnd;

    private void Awake()
    {
        m_RunEnd = GetComponent<PlayerRunEnd>();

        if (m_VictoryCamera != null)
            m_VictoryCamera.enabled = false;
    }

    private void Start()
    {
        if (BossSpawner.InstanceExists)
            BossSpawner.Instance.OnBossDefeated += HandleBossDefeated;
    }

    private void OnDestroy()
    {
        if (BossSpawner.InstanceExists)
            BossSpawner.Instance.OnBossDefeated -= HandleBossDefeated;
    }

    private void HandleBossDefeated()
    {
        m_RunEnd.End();
        SwitchToVictoryCamera();

        transform.DOMove(GetLaneCenter(), m_MoveDuration).SetEase(m_MoveEase).OnComplete(() => OnVictory?.Invoke()).SetLink(gameObject);
    }

    private void SwitchToVictoryCamera()
    {
        if (m_GameplayCamera != null)
            m_GameplayCamera.enabled = false;

        if (m_VictoryCamera != null)
            m_VictoryCamera.enabled = true;
    }

    private Vector3 GetLaneCenter()
    {
        ZoneManager zone = ZoneManager.Instance;
        Vector3 direction = zone.GetOffsetFromCenter(transform.position).normalized;
        Vector3 position = zone.transform.position + direction * zone.GetMiddleRadius();

        PlayerZoneLimiter zoneLimiter = GetComponent<PlayerZoneLimiter>();
        position.y = (zoneLimiter.MinYPosition + zoneLimiter.MaxYPosition) * 0.5f;
        return position;
    }
}
