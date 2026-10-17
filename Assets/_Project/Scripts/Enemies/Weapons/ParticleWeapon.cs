using System.Collections;
using UnityEngine;

public class ParticleWeapon : EnemyWeapon
{
    [SerializeField] private ParticleSystem _bullets;
    [SerializeField] private float _particleSpeed = 30f;

    [Header("Burst")]
    [SerializeField, Min(1)] private int _shotsPerBurst = 3;
    [SerializeField] private float _burstInterval = 0.1f;

    public override Vector3 Origin => _bullets.transform.position;
    public override float Speed => _particleSpeed;
    public override bool IsFiring => _isFiring;

    private bool _isFiring;

    private void Awake()
    {
        if (_bullets != null)
        {
            var main = _bullets.main;
            main.startSpeed = _particleSpeed;
        }
    }

    private void OnDisable()
    {
        _isFiring = false;
    }

    public override bool Fire(Quaternion rotation)
    {
        if (_bullets == null || _isFiring)
        {
            return false;
        }

        StartCoroutine(FireBurst(rotation));
        return true;
    }

    private IEnumerator FireBurst(Quaternion rotation)
    {
        _isFiring = true;
        _bullets.transform.rotation = rotation;

        for (int i = 0; i < _shotsPerBurst; i++)
        {
            if (i > 0)
            {
                yield return new WaitForSeconds(_burstInterval);
            }

            _bullets.Emit(1);
        }

        _isFiring = false;
    }
}
