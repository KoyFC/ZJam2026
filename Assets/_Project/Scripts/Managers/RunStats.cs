using UnityEngine;

public static class RunStats
{
    public static float DamageDealt { get; private set; }
    public static float ResourcesCollected { get; private set; }
    public static int EnemiesKilled { get; private set; }
    public static float Duration => (s_IsCounting ? Time.time : s_EndTime) - s_StartTime;

    private static float s_StartTime;
    private static float s_EndTime;
    private static bool s_IsCounting;

    static RunStats()
    {
        HealthController.OnAnyDamaged += CountDamage;
        HealthController.OnAnyDestroyed += CountKill;
        ResourceDrop.OnCollected += CountResources;
    }

    public static void Begin()
    {
        DamageDealt = 0f;
        ResourcesCollected = 0f;
        EnemiesKilled = 0;
        s_StartTime = Time.time;
        s_IsCounting = true;
    }

    public static void End()
    {
        s_IsCounting = false;
        s_EndTime = Time.time;
    }

    private static void CountDamage(HealthController target, float damage)
    {
        if (s_IsCounting && !target.CompareTag("Player"))
            DamageDealt += damage;
    }

    private static void CountKill(HealthController target)
    {
        if (s_IsCounting && !target.CompareTag("Player"))
            EnemiesKilled++;
    }

    private static void CountResources(float amount)
    {
        if (s_IsCounting)
            ResourcesCollected += amount;
    }
}
