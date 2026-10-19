using UnityEngine;

[CreateAssetMenu(fileName = "ZoneDataSO", menuName = "Scriptable Objects/ZoneDataSO")]
public class ZoneDataSO : ScriptableObject
{
    [field: SerializeField] public string ZoneName { get; private set; } = "New Zone";
    [field: SerializeField, Range(0.0f, 1.0f)] public float MinimapRadius { get; private set; } = 1f;

    [field: Header("Waves")]
    [field: SerializeField] public WaveData[] Waves { get; private set; }
    [field: SerializeField, Min(0f)] public float DelayBetweenWaves { get; private set; } = 2f;

    [field: Header("Ground Enemies")]
    [field: SerializeField] public GroundEnemyEntry[] GroundEnemies { get; private set; }
    [field: SerializeField, Min(0)] public int GroundEnemyCount { get; private set; } = 10;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(ZoneName))
        {
            ZoneName = name;
        }
    }
}
