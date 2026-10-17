using UnityEngine;

[RequireComponent(typeof(PlayerDetector))]
public class EnemyShooter : MonoBehaviour
{
    [SerializeField] private float _shootInterval = 1f;

    private PlayerDetector _detector;
    private EnemyWeapon _weapon;
    private float _lastShootTime;

    private void Awake()
    {
        _detector = GetComponent<PlayerDetector>();
        _weapon = GetComponent<EnemyWeapon>();
    }

    private void Update()
    {
        if (_weapon == null || !_detector.HasTarget || Time.time - _lastShootTime < _shootInterval)
        {
            return;
        }

        if (Fire(_detector.Target))
        {
            _lastShootTime = Time.time;
        }
    }

    private bool Fire(Transform target)
    {
        Vector3 targetDirection = target.position - _weapon.Origin;
        Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
        if (targetRigidbody != null && _weapon.Speed > 0f)
        {
            float timeToTarget = targetDirection.magnitude / _weapon.Speed;
            Vector3 predictedPosition = target.position + targetRigidbody.linearVelocity * timeToTarget;
            targetDirection = predictedPosition - _weapon.Origin;
        }

        if (targetDirection.sqrMagnitude <= Mathf.Epsilon)
        {
            return false;
        }

        return _weapon.Fire(Quaternion.LookRotation(targetDirection.normalized));
    }
}
