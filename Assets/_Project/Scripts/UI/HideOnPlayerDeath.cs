using UnityEngine;

public class HideOnPlayerDeath : MonoBehaviour
{
    private void Awake()
    {
        PlayerDeath.OnDeathStarted += Hide;
    }

    private void OnDestroy()
    {
        PlayerDeath.OnDeathStarted -= Hide;
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
