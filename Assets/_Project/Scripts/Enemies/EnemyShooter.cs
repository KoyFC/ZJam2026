using UnityEngine;

[RequireComponent(typeof(PlayerDetector))]
public class EnemyShooter : MonoBehaviour
{
    [SerializeField] private float _shootInterval = 1f;
    [SerializeField, Range(0f, 1f)] private float _intervalVariance = 0.2f;

    [Header("Telegraph")]
    [SerializeField] private ParticleSystem _telegraph;
    [SerializeField, Min(0f)] private float _telegraphLifetime = 1f;

    private PlayerDetector _detector;
    private EnemyWeapon _weapon;
    private float _lastShootTime;
    private float _currentInterval;
    private float _fireTime;
    private bool _isWindingUp;

    private void Awake()
    {
        _detector = GetComponent<PlayerDetector>();
        _weapon = GetComponent<EnemyWeapon>();
        RollInterval();

        if (_telegraph != null)
        {
            var main = _telegraph.main;
            main.startLifetime = _telegraphLifetime;
        }
    }

    private void OnDisable()
    {
        _isWindingUp = false;
    }

    private void Update()
    {
        if (_weapon == null)
        {
            return;
        }

        if (!_detector.HasTarget)
        {
            _isWindingUp = false;
            return;
        }

        if (!_isWindingUp && !_weapon.IsFiring && Time.time - _lastShootTime >= _currentInterval - _telegraphLifetime)
        {
            StartWindup();
        }

        if (_isWindingUp && Time.time >= _fireTime)
        {
            _isWindingUp = false;
            if (Fire(_detector.Target))
            {
                _lastShootTime = Time.time;
                RollInterval();
            }
        }
    }

    private void RollInterval()
    {
        _currentInterval = _shootInterval * Random.Range(1f - _intervalVariance, 1f + _intervalVariance);
    }

    private void StartWindup()
    {
        _isWindingUp = true;
        _fireTime = Time.time + _telegraphLifetime;

        if (_telegraph != null)
        {
            _telegraph.Emit(1);
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
