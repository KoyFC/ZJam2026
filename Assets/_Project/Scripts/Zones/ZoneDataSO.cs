using UnityEngine;

[CreateAssetMenu(fileName = "ZoneDataSO", menuName = "Scriptable Objects/ZoneDataSO")]
public class ZoneDataSO : ScriptableObject
{
    [field: SerializeField] public string ZoneName { get; private set; } = "New Zone";
    [field: SerializeField] public float Radius { get; private set; } = 0f;
    [field: SerializeField, Range(0.0f, 1.0f)] public float MinimapRadius { get; private set; } = 1f;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(ZoneName))
        {
            ZoneName = name;
        }
    }
}
