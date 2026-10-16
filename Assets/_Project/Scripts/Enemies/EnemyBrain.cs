using UnityEngine;


[RequireComponent(typeof(HealthController))]
[RequireComponent(typeof(SphereCollider))]
public class EnemyBrain : MonoBehaviour
{

    [Header("Attack")]
    [SerializeField] protected Transform _shootPoint;
    [SerializeField] protected GameObject _projectilePrefab;
    [SerializeField] protected float _projectileSpeed = 10f;
    [SerializeField] protected float _shootInterval = 1f;

    protected GameObject Target => _target;

    private HealthController _healthController;
    private SphereCollider _sphereCollider;
    private float _lastShootTime;
    private GameObject _target;

    protected virtual void Awake()
    {
        _healthController = GetComponent<HealthController>();
        _sphereCollider = GetComponent<SphereCollider>();

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
        if (Target == null || _projectilePrefab == null || _shootPoint == null)
        {
            return false;
        }

        Vector3 targetDirection = Target.transform.position - _shootPoint.position;
        Rigidbody targetRigidbody = Target.GetComponent<Rigidbody>();
        if (targetRigidbody != null && _projectileSpeed > 0f)
        {
            float timeToTarget = targetDirection.magnitude / _projectileSpeed;
            Vector3 predictedPosition = Target.transform.position + targetRigidbody.linearVelocity * timeToTarget;
            targetDirection = predictedPosition - _shootPoint.position;
        }

        if (targetDirection.sqrMagnitude <= Mathf.Epsilon)
        {
            return false;
        }

        Quaternion projectileRotation = Quaternion.LookRotation(targetDirection.normalized);
        GameObject projectile = Instantiate(_projectilePrefab, _shootPoint.position, projectileRotation);
        Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
        if (projectileRigidbody != null)
        {
            projectileRigidbody.linearVelocity = targetDirection.normalized * _projectileSpeed;
        }

        return true;
    }
}
