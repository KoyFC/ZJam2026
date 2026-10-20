using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum RunOutcome
{
    Victory,
    Defeat
}

public class RunSummaryUI : MonoBehaviour
{
    [SerializeField] private RunOutcome m_Outcome;
    [SerializeField] private CanvasGroup m_Panel;
    [SerializeField] private TMP_Text m_TitleText;
    [SerializeField] private TMP_Text m_StatsText;
    [SerializeField] private string m_Title = "Zone cleared";
    [SerializeField, Min(0f)] private float m_FadeDuration = 0.5f;
    [SerializeField] private string m_ReturnScene = "NearSideScene";

    private readonly StringBuilder m_Builder = new();

    private void Awake()
    {
        SetPanelVisible(false);

        if (m_Outcome == RunOutcome.Victory)
            PlayerVictory.OnVictory += Show;
        else
            PlayerDeath.OnDied += Show;
    }

    private void OnDestroy()
    {
        PlayerVictory.OnVictory -= Show;
        PlayerDeath.OnDied -= Show;
        m_Panel.DOKill();
    }

    private void Show()
    {
        m_TitleText.text = m_Title;
        m_StatsText.text = BuildStats();

        SetPanelVisible(true);
        m_Panel.DOFade(1f, m_FadeDuration).From(0f).SetUpdate(true);
        Cursor.visible = true;
    }

    private void SetPanelVisible(bool visible)
    {
        m_Panel.alpha = visible ? 1f : 0f;
        m_Panel.interactable = visible;
        m_Panel.blocksRaycasts = visible;
    }

    private string BuildStats()
    {
        m_Builder.Clear();

        int minutes = Mathf.FloorToInt(RunStats.Duration / 60f);
        int seconds = Mathf.FloorToInt(RunStats.Duration % 60f);
        m_Builder.AppendLine($"Time: {minutes:00}:{seconds:00}");
        m_Builder.AppendLine($"Damage dealt: {RunStats.DamageDealt:0}");
        m_Builder.AppendLine($"Enemies killed: {RunStats.EnemiesKilled}");
        m_Builder.AppendLine($"Resources collected: {RunStats.ResourcesCollected:0}");

        if (WaveSpawner.InstanceExists)
            m_Builder.AppendLine($"Waves cleared: {WaveSpawner.Instance.CompletedWaves}/{WaveSpawner.Instance.TotalWaves}");

        if (RailFuel.InstanceExists)
            m_Builder.AppendLine($"Fuel left: {(1f - RailFuel.Instance.Normalized) * 100f:0}%");

        PlayerMechanic mechanic = ZoneManager.InstanceExists ? ZoneManager.Instance.CurrentZone.UnlockedMechanic : PlayerMechanic.None;
        if (m_Outcome == RunOutcome.Victory && mechanic != PlayerMechanic.None)
            m_Builder.AppendLine($"Unlocked: {mechanic}");

        return m_Builder.ToString();
    }

    public void ReturnToBase()
    {
        SceneManager.LoadScene(m_ReturnScene);
    }
}
