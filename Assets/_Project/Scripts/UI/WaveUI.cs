using DG.Tweening;
using TMPro;
using UnityEngine;

public class WaveUI : MonoBehaviour
{
    [SerializeField] private TMP_Text m_WaveText;
    [SerializeField] private TMP_Text m_EnemiesText;
    [SerializeField] private float m_KillPunchStrength = 0.25f;
    [SerializeField] private float m_KillPunchDuration = 0.25f;
    [SerializeField] private float m_EnemiesTextScaleDuration = 0.25f;

    [Header("Countdown")]
    [SerializeField] private float m_CountdownScale = 1.5f;
    [SerializeField] private float m_CountdownMoveDuration = 0.5f;
    [SerializeField] private Ease m_CountdownEase = Ease.OutCubic;
    [SerializeField, Min(0f)] private float m_CountdownDelay = 1f;

    private Vector3 m_WaveTextRestPosition;
    private bool m_IsWaveTextCentered;
    private bool m_IsEnemiesTextVisible;
    private float m_CountdownStartTime;

    private bool m_WasCountdownActive = true;

    private void Awake()
    {
        m_WaveTextRestPosition = m_WaveText.transform.localPosition;
        m_EnemiesText.transform.localScale = Vector3.zero;
    }

    private void Start()
    {
        if (WaveSpawner.InstanceExists)
            WaveSpawner.Instance.OnEnemyKilled += PunchEnemiesText;
    }

    private void OnDestroy()
    {
        if (WaveSpawner.InstanceExists)
            WaveSpawner.Instance.OnEnemyKilled -= PunchEnemiesText;

        m_EnemiesText.transform.DOKill();
        m_WaveText.transform.DOKill();
    }

    private void Update()
    {
        if (!WaveSpawner.InstanceExists)
            return;

        WaveSpawner waveSpawner = WaveSpawner.Instance;

        bool isBossCountdownActive = BossSpawner.InstanceExists && BossSpawner.Instance.IsCountdownActive;
        bool isCountdownActive = (!waveSpawner.IsWaveActive && !waveSpawner.IsStopped) || isBossCountdownActive;

        if (isCountdownActive && !m_WasCountdownActive)
            m_CountdownStartTime = Time.time;
        m_WasCountdownActive = isCountdownActive;

        SetEnemiesTextVisible(waveSpawner.IsWaveActive);

        if (isCountdownActive && Time.time < m_CountdownStartTime + m_CountdownDelay)
            return;

        if (waveSpawner.IsWaveActive)
            ShowActiveWave(waveSpawner);
        else if (isBossCountdownActive)
            ShowBossCountdown(BossSpawner.Instance);
        else if (waveSpawner.IsStopped)
            ShowStopped(waveSpawner);
        else
            ShowNextWaveCountdown(waveSpawner);

        SetWaveTextCentered(isCountdownActive);
    }

    private void ShowActiveWave(WaveSpawner waveSpawner)
    {
        m_WaveText.text = $"Wave {waveSpawner.CurrentWave}/{waveSpawner.TotalWaves}";
        string enemyText = waveSpawner.EnemiesLeft == 1 ? "enemy" : "enemies";
        m_EnemiesText.text = $"{waveSpawner.EnemiesLeft} {enemyText} left";
    }

    private void ShowStopped(WaveSpawner waveSpawner)
    {
        if (BossSpawner.InstanceExists && BossSpawner.Instance.IsFightActive)
        {
            m_WaveText.text = "Boss";
            return;
        }

        if (BossSpawner.InstanceExists && BossSpawner.Instance.IsDefeated)
        {
            m_WaveText.text = "Boss defeated";
            return;
        }

        // Stopped either because every wave was cleared or because the player ran out of fuel
        m_WaveText.text = waveSpawner.CompletedWaves >= waveSpawner.TotalWaves ? "Waves cleared" : string.Empty;
    }

    private void ShowNextWaveCountdown(WaveSpawner waveSpawner)
    {
        m_WaveText.text = $"Wave {waveSpawner.CurrentWave + 1}/{waveSpawner.TotalWaves} in {Mathf.Ceil(waveSpawner.NextWaveTimeLeft):0}";
    }

    private void ShowBossCountdown(BossSpawner bossSpawner)
    {
        m_WaveText.text = $"Boss in {Mathf.Ceil(bossSpawner.SpawnTimeLeft):0}";
    }

    private void SetWaveTextCentered(bool centered)
    {
        if (m_IsWaveTextCentered == centered)
            return;

        m_IsWaveTextCentered = centered;
        Transform waveText = m_WaveText.transform;
        waveText.DOKill();

        if (centered)
        {
            Vector3 screenCenter = m_WaveText.canvas.rootCanvas.transform.position;
            waveText.DOMove(screenCenter, m_CountdownMoveDuration).SetEase(m_CountdownEase);
            waveText.DOScale(m_CountdownScale, m_CountdownMoveDuration).SetEase(m_CountdownEase);
        }
        else
        {
            waveText.DOLocalMove(m_WaveTextRestPosition, m_CountdownMoveDuration).SetEase(m_CountdownEase);
            waveText.DOScale(1f, m_CountdownMoveDuration).SetEase(m_CountdownEase);
        }
    }

    private void SetEnemiesTextVisible(bool visible)
    {
        if (m_IsEnemiesTextVisible == visible)
            return;

        m_IsEnemiesTextVisible = visible;
        Transform enemiesText = m_EnemiesText.transform;
        enemiesText.DOKill();

        if (visible)
            enemiesText.DOScale(1f, m_EnemiesTextScaleDuration).From(Vector3.zero);
        else
            enemiesText.DOScale(0f, m_EnemiesTextScaleDuration);
    }

    private void PunchEnemiesText()
    {
        m_EnemiesText.transform.DOKill(true);
        m_EnemiesText.transform.DOPunchScale(Vector3.one * m_KillPunchStrength, m_KillPunchDuration);
    }
}
