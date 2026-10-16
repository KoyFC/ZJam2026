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

        Vector3 position = m_SpawnTransform.position;
        Quaternion rotation = PlayerCrosshair.InstanceExists
            ? PlayerCrosshair.Instance.GetAimRotation(position)
            : m_SpawnTransform.rotation;

        Instantiate(m_MissilePrefab, position, rotation);
    }
}
