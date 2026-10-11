using UnityEngine;

[CreateAssetMenu(fileName = "ZoneDataSO", menuName = "Scriptable Objects/ZoneDataSO")]
public class ZoneDataSO : ScriptableObject
{
    [field: SerializeField] public string ZoneName { get; private set; }
    [field: SerializeField] public float Radius { get; private set; }
}
