using UnityEngine;

[RequireComponent(typeof(HealthController))]
public class DestroyOnDeath : MonoBehaviour
{
    private HealthController _healthController;

    private void Awake()
    {
        _healthController = GetComponent<HealthController>();
        _healthController.OnDestroyed += HandleDeath;
    }

    private void OnDestroy()
    {
        _healthController.OnDestroyed -= HandleDeath;
    }

    private void HandleDeath()
    {
        Destroy(gameObject);
    }
}
