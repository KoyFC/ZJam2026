using UnityEngine;

public class HideOnRunEnd : MonoBehaviour
{
    private void Awake()
    {
        PlayerRunEnd.OnRunEnded += Hide;
    }

    private void OnDestroy()
    {
        PlayerRunEnd.OnRunEnded -= Hide;
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
