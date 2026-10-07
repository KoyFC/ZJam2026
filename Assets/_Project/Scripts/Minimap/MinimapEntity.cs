using UnityEngine;

public class MinimapEntity : MonoBehaviour
{
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField, Min(0.0f)] public float IconSize { get; private set; }

    private void OnEnable()
    {
        MinimapController.Instance.RegisterElement(this);
    }

    private void OnDisable()
    {
        MinimapController.Instance.UnregisterElement(this);
    }
}
