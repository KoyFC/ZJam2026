using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class RocketBoss : EnemyWeapon
{
    [Header("Rockets")]
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private Projectile _rocketPrefab;
    [SerializeField, Min(1)] private int _poolSize = 16;

    [Header("Burst")]
    [SerializeField, Min(1)] private int _rocketsPerBurst = 8;
    [SerializeField, Min(0f)] private float _rocketInterval = 0.15f;
    [SerializeField, Min(0f)] private float _spreadAngle = 30f;

    private Pool<Projectile> _rocketPool;
    private EnemyShooter _shooter;
    private bool _isFiring;

    public override Vector3 Origin => _shootPoint != null ? _shootPoint.position : transform.position;
    public override float Speed => _rocketPrefab != null ? _rocketPrefab.Speed : 0f;
    public override bool IsFiring => _isFiring;

    private void Awake()
    {
        _shooter = GetComponent<EnemyShooter>();

        if (_shootPoint == null)
        {
            _shootPoint = transform;
        }

        if (_rocketPrefab == null)
        {
            Debug.LogError("RocketBoss requires a rocket projectile prefab.", this);
            return;
        }

        _rocketPool = PoolManager.Instance.GetPool(_rocketPrefab, _poolSize);
    }

    private void OnDisable()
    {
        _isFiring = false;
    }

    public override bool Fire(Quaternion rotation)
    {
        if (_rocketPool == null || _isFiring)
        {
            return false;
        }

        StartCoroutine(FireBurst(rotation));
        return true;
    }

    private IEnumerator FireBurst(Quaternion rotation)
    {
        _isFiring = true;
        int rocketCount = Mathf.Max(1, _rocketsPerBurst);

        for (int i = 0; i < rocketCount; i++)
        {
            if (i > 0)
            {
                yield return new WaitForSeconds(_rocketInterval);
            }

            if (_shooter != null && _shooter.TryGetAim(out Quaternion aim))
            {
                rotation = aim;
            }

            Projectile rocket = _rocketPool.Get(Origin, rotation * GetSpread(i, rocketCount));
            rocket.Launch(_rocketPool);
        }

        _isFiring = false;
    }

    // Even rockets go straight at the player, odd ones circle around them to cut off dodges
    private Quaternion GetSpread(int index, int rocketCount)
    {
        if (index % 2 == 0)
        {
            return Quaternion.identity;
        }

        int ringCount = rocketCount / 2;
        float angle = (index / 2) * 360f / ringCount * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (_spreadAngle * 0.5f);
        return Quaternion.Euler(-offset.y, offset.x, 0f);
    }
}
