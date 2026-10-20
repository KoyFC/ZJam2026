using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private TMP_Text m_InstructionText;
    [SerializeField] private TMP_Text m_CompletedText;
    [SerializeField] private float m_TextPunchStrength = 0.15f;
    [SerializeField] private float m_TextPunchDuration = 0.3f;
    [SerializeField] private Color m_HealthColor = new Color(0.09f, 1f, 0f);
    [SerializeField] private Color m_FuelColor = new Color(1f, 0.5f, 0f);
    [SerializeField] private GameObject[] m_HiddenUntilCombat;

    [Header("Steps")]
    [SerializeField, Min(0f)] private float m_StepDelay = 1.5f;
    [SerializeField, Min(0f)] private float m_RequiredMoveTime = 1.5f;
    [SerializeField, Min(0f)] private float m_RequiredFireTime = 1.5f;
    [SerializeField, Min(0f)] private float m_FuelInfoDuration = 5f;
    [SerializeField, Min(0f)] private float m_MinPlayerHealth = 10f;

    [Header("Drops")]
    [SerializeField] private ResourceDrop m_Drop;
    [SerializeField, Min(0f)] private float m_HealthDropAmount = 25f;
    [SerializeField, Min(0f)] private float m_FuelDropAmount = 45f;
    [SerializeField] private float m_DropAheadAngle = 8f;
    [SerializeField] private float m_DropSpacingAngle = 3f;
    [SerializeField] private float m_DropRiseOffset = 5f;
    [SerializeField, Min(0f)] private float m_DropRespawnDelay = 1.5f;

    [Header("End")]
    [SerializeField, Min(0f)] private float m_EndDelay = 4f;
    [SerializeField] private string m_NextScene = "NearSideScene";

    private HealthController m_PlayerHealth;

    private void Start()
    {
        m_PlayerHealth = PlayerRailFollower.Instance.GetComponent<HealthController>();
        m_PlayerHealth.MinHealth = m_MinPlayerHealth;

        RailFuel.Instance.SetPaused(true);
        WaveSpawner.Instance.enabled = false;
        SetCombatObjectsActive(false);
        m_CompletedText.enabled = false;

        StartCoroutine(RunTutorial());
    }

    private void OnDestroy()
    {
        m_InstructionText.transform.DOKill();
        m_CompletedText.transform.DOKill();
    }

    private IEnumerator RunTutorial()
    {
        yield return new WaitWhile(() => SceneLoader.IsBusy);
        PlayerInputController input = PlayerInputController.Instance;

        ShowInstruction("Use <b>WASD</b> to move your ship");
        yield return WaitWhileHeld(() => input.MoveInput.sqrMagnitude > 0.01f, m_RequiredMoveTime);

        ShowInstruction("Hold <b>Left Click</b> to shoot where you aim");
        yield return WaitWhileHeld(() => input.IsPrimaryFireHeld, m_RequiredFireTime);

        ShowInstruction($"Enemies incoming! Shoot them down\nTheir shots damage you, keep an eye on your {Colorize("<b>health</b>", m_HealthColor)}");
        yield return WaitForCombat();

        ShowInstruction($"Your ship burns {Colorize("<b>fuel</b>", m_FuelColor)} while flying around the ring\nIf it runs out, the run is over");
        RailFuel.Instance.SetPaused(false);
        yield return new WaitForSeconds(m_FuelInfoDuration);
        yield return CompleteStep();

        ShowInstruction("Enemies drop cheese, fly into it to collect it\n" +
            $"{Colorize("Sage Derby", m_HealthColor)} restores {Colorize("health", m_HealthColor)}, " +
            $"{Colorize("Cheddar", m_FuelColor)} restores {Colorize("fuel", m_FuelColor)}");
        yield return WaitForDrops();

        RailFuel.Instance.SetPaused(true);
        ShowInstruction("Tutorial complete!\nGood luck on the Far Side");
        yield return new WaitForSeconds(m_EndDelay);

        SceneLoader.Load(m_NextScene);
    }

    private IEnumerator WaitWhileHeld(Func<bool> condition, float duration)
    {
        float heldTime = 0f;
        while (heldTime < duration)
        {
            if (condition())
                heldTime += Time.deltaTime;

            yield return null;
        }

        yield return CompleteStep();
    }

    private IEnumerator WaitForCombat()
    {
        bool wavesCompleted = false;
        Action onCompleted = () => wavesCompleted = true;

        WaveSpawner waveSpawner = WaveSpawner.Instance;
        waveSpawner.OnWavesCompleted += onCompleted;
        waveSpawner.enabled = true;
        SetCombatObjectsActive(true);

        yield return new WaitUntil(() => wavesCompleted);
        waveSpawner.OnWavesCompleted -= onCompleted;

        yield return CompleteStep();
    }

    private IEnumerator WaitForDrops()
    {
        Coroutine health = StartCoroutine(DeliverDrop(ResourceDropType.Health, m_HealthDropAmount, m_DropAheadAngle));
        Coroutine fuel = StartCoroutine(DeliverDrop(ResourceDropType.Fuel, m_FuelDropAmount, m_DropAheadAngle + m_DropSpacingAngle));
        yield return health;
        yield return fuel;

        yield return CompleteStep();
    }

    // Respawns the drop ahead of the player until it gets collected
    private IEnumerator DeliverDrop(ResourceDropType type, float amount, float aheadAngle)
    {
        bool collected = false;
        while (!collected)
        {
            bool released = false;
            Action<bool> onReleased = wasCollected =>
            {
                released = true;
                collected = wasCollected;
            };

            ResourceDrop drop = SpawnDrop(type, amount, aheadAngle);
            drop.OnReleased += onReleased;
            yield return new WaitUntil(() => released);
            drop.OnReleased -= onReleased;

            if (!collected)
                yield return new WaitForSeconds(m_DropRespawnDelay);
        }
    }

    private IEnumerator CompleteStep()
    {
        m_CompletedText.enabled = true;
        Punch(m_CompletedText.transform);

        yield return new WaitForSeconds(m_StepDelay);
    }

    private ResourceDrop SpawnDrop(ResourceDropType type, float amount, float aheadAngle)
    {
        Transform player = PlayerRailFollower.Instance.transform;
        ZoneManager zone = ZoneManager.Instance;

        float radius = zone.GetOffsetFromCenter(player.position).magnitude;
        Vector3 position = zone.transform.position + PlayerRailFollower.Instance.GetDirectionAhead(aheadAngle) * radius;
        position.y = player.position.y - m_DropRiseOffset;

        Pool<ResourceDrop> pool = PoolManager.Instance.GetPool(m_Drop);
        ResourceDrop drop = pool.Get(position, Quaternion.identity);
        drop.Launch(pool, type, amount, player.position.y);
        return drop;
    }

    private void ShowInstruction(string text)
    {
        m_InstructionText.text = text;
        m_CompletedText.enabled = false;
        Punch(m_InstructionText.transform);
    }

    private static string Colorize(string text, Color color)
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
    }

    private void Punch(Transform target)
    {
        target.DOKill(true);
        target.DOPunchScale(Vector3.one * m_TextPunchStrength, m_TextPunchDuration);
    }

    private void SetCombatObjectsActive(bool active)
    {
        foreach (GameObject target in m_HiddenUntilCombat)
            target.SetActive(active);
    }
}
