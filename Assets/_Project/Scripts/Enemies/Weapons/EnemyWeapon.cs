using UnityEngine;

[DisallowMultipleComponent]
public abstract class EnemyWeapon : MonoBehaviour
{
    public abstract Vector3 Origin { get; }
    public abstract float Speed { get; }

    public abstract bool Fire(Quaternion rotation);
}
