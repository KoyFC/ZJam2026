using UnityEngine;

public class ParticleWeapon : EnemyWeapon
{
    [SerializeField] private ParticleSystem _bullets;
    [SerializeField] private float _particleSpeed = 30f;
    [SerializeField] private int _particlesPerShot = 1;

    public override Vector3 Origin => _bullets.transform.position;
    public override float Speed => _particleSpeed;

    private void Awake()
    {
        if (_bullets != null)
        {
            var main = _bullets.main;
            main.startSpeed = _particleSpeed;
        }
    }

    public override bool Fire(Quaternion rotation)
    {
        if (_bullets == null)
        {
            return false;
        }

        _bullets.transform.rotation = rotation;
        _bullets.Emit(_particlesPerShot);
        return true;
    }
}
