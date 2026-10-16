using UnityEngine;

public class PlayerFiringController : MonoBehaviour
{
    [SerializeField] private Transform m_SpawnTransform;
    [SerializeField] private GameObject m_MissilePrefab;

    private void Awake()
    {
        if (m_SpawnTransform == null)
            m_SpawnTransform = transform;
    }

    private void OnEnable()
    {
        PlayerInputController.OnPrimaryFire += FireMissile;
        PlayerInputController.OnSecondaryFire += FireMissile;
    }

    private void OnDisable()
    {
        PlayerInputController.OnPrimaryFire -= FireMissile;
        PlayerInputController.OnSecondaryFire -= FireMissile;
    }

    private void FireMissile()
    {
        if (m_MissilePrefab == null)
            return;

        var missile = Instantiate(m_MissilePrefab, m_SpawnTransform.position, m_SpawnTransform.rotation);
    }
}
