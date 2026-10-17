using UnityEngine;


[RequireComponent(typeof(HealthController))]
public class EnemyBrain : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField, Range(0f, 180f)] protected float _maxAheadAngle = 60f;

    [Header("Attack")]
    [SerializeField] protected Transform _shootPoint;
    [SerializeField] protected Projectile _projectilePrefab;
    [SerializeField] protected int _projectilePoolSize = 10;
    [SerializeField] protected float _shootInterval = 1f;

    protected GameObject Target => _target;

    private HealthController _healthController;
    private Pool<Projectile> _projectilePool;
    private float _lastShootTime;
    private GameObject _target;

    protected virtual void Awake()
    {
        _healthController = GetComponent<HealthController>();

        if (_projectilePrefab != null)
        {
            Transform poolContainer = new GameObject($"{name} Projectile Pool").transform;
            _projectilePool = new Pool<Projectile>(_projectilePrefab, poolContainer, _projectilePoolSize);
        }

        _healthController.OnDestroyed += HandleDeath;
        _healthController.OnDamaged += HandleDamage;
    }

    protected virtual void OnDestroy()
    {
        _healthController.OnDestroyed -= HandleDeath;
        _healthController.OnDamaged -= HandleDamage;
    }

    protected virtual void HandleDeath()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }

    protected virtual void HandleDamage(float damage)
    {
        Debug.Log($"{gameObject.name} took {damage} damage. Current health: {_healthController.CurrentHealth}");
    }

    protected virtual void Update()
    {
        _target = IsAheadOfPlayer() ? PlayerRailFollower.Instance.gameObject : null;

        if (Target != null)
        {
            TryAttackTarget();
        }
    }

    protected virtual bool IsAheadOfPlayer()
    {
        if (!PlayerRailFollower.InstanceExists || !ZoneManager.InstanceExists)
        {
            return false;
        }

        PlayerRailFollower playerRailFollower = PlayerRailFollower.Instance;
        ZoneManager zone = ZoneManager.Instance;

        Vector3 playerOffset = zone.GetOffsetFromCenter(playerRailFollower.transform.position);
        Vector3 enemyOffset = zone.GetOffsetFromCenter(transform.position);

        float angle = Vector3.SignedAngle(playerOffset, enemyOffset, Vector3.up);
        if (!playerRailFollower.Clockwise)
        {
            angle = -angle;
        }

        return angle > 0f && angle < _maxAheadAngle;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (Target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, Target.transform.position);
        }
    }

    protected virtual void TryAttackTarget()
    {
        if (Time.time - _lastShootTime >= _shootInterval)
        {
            if (AttackTarget())
            {
                _lastShootTime = Time.time;
            }
        }
    }

    protected virtual bool AttackTarget()
    {
        return FireProjectile();
    }

    protected virtual bool FireProjectile()
    {
        if (Target == null || _projectilePool == null || _shootPoint == null)
        {
            return false;
        }

        Vector3 targetDirection = Target.transform.position - _shootPoint.position;
        Rigidbody targetRigidbody = Target.GetComponent<Rigidbody>();
        if (targetRigidbody != null && _projectilePrefab.Speed > 0f)
        {
            float timeToTarget = targetDirection.magnitude / _projectilePrefab.Speed;
            Vector3 predictedPosition = Target.transform.position + targetRigidbody.linearVelocity * timeToTarget;
            targetDirection = predictedPosition - _shootPoint.position;
        }

        if (targetDirection.sqrMagnitude <= Mathf.Epsilon)
        {
            return false;
        }

        Quaternion projectileRotation = Quaternion.LookRotation(targetDirection.normalized);
        Projectile projectile = _projectilePool.Get(_shootPoint.position, projectileRotation);
        projectile.Launch(_projectilePool);

        return true;
    }
}
