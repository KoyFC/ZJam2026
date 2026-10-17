using UnityEngine;

public class ProjectileWeapon : EnemyWeapon
{
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private int _projectilePoolSize = 10;

    public override Vector3 Origin => _shootPoint.position;
    public override float Speed => _projectilePrefab.Speed;

    private Pool<Projectile> _projectilePool;

    private void Awake()
    {
        if (_shootPoint == null)
        {
            _shootPoint = transform;
        }

        if (_projectilePrefab != null)
        {
            Transform poolContainer = new GameObject($"{name} Projectile Pool").transform;
            _projectilePool = new Pool<Projectile>(_projectilePrefab, poolContainer, _projectilePoolSize);
        }
    }

    public override bool Fire(Quaternion rotation)
    {
        if (_projectilePool == null)
        {
            return false;
        }

        Projectile projectile = _projectilePool.Get(_shootPoint.position, rotation);
        projectile.Launch(_projectilePool);
        return true;
    }
}
