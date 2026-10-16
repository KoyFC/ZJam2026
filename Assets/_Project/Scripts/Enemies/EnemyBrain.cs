using UnityEngine;


[RequireComponent(typeof(HealthController))]
[RequireComponent(typeof(SphereCollider))]
public class EnemyBrain : MonoBehaviour
{

    [Header("Attack")]
    [SerializeField] protected Transform _shootPoint;
    [SerializeField] protected Projectile _projectilePrefab;
    [SerializeField] protected int _projectilePoolSize = 10;
    [SerializeField] protected float _shootInterval = 1f;

    protected GameObject Target => _target;

    private HealthController _healthController;
    private SphereCollider _sphereCollider;
    private Pool<Projectile> _projectilePool;
    private float _lastShootTime;
    private GameObject _target;

    protected virtual void Awake()
    {
        _healthController = GetComponent<HealthController>();
        _sphereCollider = GetComponent<SphereCollider>();

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

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _target = other.gameObject;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _target = null;
        }
    }

    protected virtual void Update()
    {
        if (Target != null)
        {
            TryAttackTarget();
        }
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
