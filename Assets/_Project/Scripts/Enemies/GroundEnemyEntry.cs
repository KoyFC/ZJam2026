using System;
using UnityEngine;

[Serializable]
public class GroundEnemyEntry
{
    public EnemySpawnAnimation Prefab;
    [Min(0f)] public float Weight = 1f;
}
