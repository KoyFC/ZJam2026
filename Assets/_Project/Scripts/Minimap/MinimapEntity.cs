using UnityEngine;

public class MinimapEntity : MonoBehaviour
{
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField, Min(0.0f)] public float IconScale { get; private set; }

    private void OnEnable()
    {
        MinimapControllerUI.Instance.RegisterElement(this);
    }

    private void OnDisable()
    {
        MinimapControllerUI.Instance.UnregisterElement(this);
    }
}
