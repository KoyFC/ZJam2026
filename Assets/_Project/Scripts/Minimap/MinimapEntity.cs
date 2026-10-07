using UnityEngine;
using System.Collections;

public class MinimapEntity : MonoBehaviour
{
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField, Min(0.0f)] public float IconScale { get; private set; } = 1.0f;

    private bool m_AddPending = false;

    private void OnEnable()
    {
        if (MinimapControllerUI.InstanceExists)
        {
            MinimapControllerUI.Instance.RegisterElement(this);
        }
        else
        {
            m_AddPending = true;
        }
    }

    private IEnumerator Start()
    {
        if (m_AddPending)
        {
            yield return new WaitUntil(() => MinimapControllerUI.InstanceExists);
            MinimapControllerUI.Instance.RegisterElement(this);
            m_AddPending = false;
        }
    }

    private void OnDisable()
    {
        if (MinimapControllerUI.InstanceExists)
        {
            MinimapControllerUI.Instance.UnregisterElement(this);
        }
    }
}
