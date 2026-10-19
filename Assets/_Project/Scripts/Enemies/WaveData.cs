using System;
using UnityEngine;

[Serializable]
public class WaveEntry
{
    public EnemySpawnAnimation Prefab;
    [Min(1)] public int Count = 1;
}

[Serializable]
public class WaveData
{
    public WaveEntry[] Enemies;
    [Min(1f)] public float TimeLimit = 30f;
}
