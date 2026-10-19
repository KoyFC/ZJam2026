using DG.Tweening;
using TMPro;
using UnityEngine;

public class WaveUI : MonoBehaviour
{
    [SerializeField] private TMP_Text m_WaveText;
    [SerializeField] private TMP_Text m_EnemiesText;
    [SerializeField] private float m_KillPunchStrength = 0.3f;
    [SerializeField] private float m_KillPunchDuration = 0.25f;

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
    }

    private void Update()
    {
        if (!WaveSpawner.InstanceExists)
            return;

        WaveSpawner waveSpawner = WaveSpawner.Instance;

        if (waveSpawner.IsWaveActive)
            ShowActiveWave(waveSpawner);
        else if (waveSpawner.IsStopped)
            ShowStopped(waveSpawner);
        else
            ShowNextWaveCountdown(waveSpawner);
    }

    private void ShowActiveWave(WaveSpawner waveSpawner)
    {
        m_WaveText.text = $"Wave {waveSpawner.CurrentWave}/{waveSpawner.TotalWaves}";
        string enemyText = waveSpawner.EnemiesLeft == 1 ? "enemy" : "enemies";
        m_EnemiesText.text = $"{waveSpawner.EnemiesLeft} {enemyText} left";
    }

    private void ShowStopped(WaveSpawner waveSpawner)
    {
        // Stopped either because every wave was cleared or because the player ran out of fuel
        m_WaveText.text = waveSpawner.CompletedWaves >= waveSpawner.TotalWaves ? "Waves cleared" : string.Empty;
        m_EnemiesText.text = string.Empty;
    }

    private void ShowNextWaveCountdown(WaveSpawner waveSpawner)
    {
        m_WaveText.text = $"Wave {waveSpawner.CurrentWave + 1}/{waveSpawner.TotalWaves} in {Mathf.Ceil(waveSpawner.NextWaveTimeLeft):0}";
        m_EnemiesText.text = string.Empty;
    }

    private void PunchEnemiesText()
    {
        m_EnemiesText.transform.DOKill(true);
        m_EnemiesText.transform.DOPunchScale(Vector3.one * m_KillPunchStrength, m_KillPunchDuration);
    }
}
