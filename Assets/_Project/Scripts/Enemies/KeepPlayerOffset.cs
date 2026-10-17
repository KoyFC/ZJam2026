using UnityEngine;

public class KeepPlayerOffset : MonoBehaviour
{
    [SerializeField] private float _followSmoothTime = 0.3f;

    private bool _hasOffset;
    private float _angleOffset;
    private float _angleVelocity;

    private void Update()
    {
        if (!PlayerRailFollower.InstanceExists || !ZoneManager.InstanceExists)
        {
            return;
        }

        PlayerRailFollower player = PlayerRailFollower.Instance;
        ZoneManager zone = ZoneManager.Instance;
        Vector3 playerOffset = zone.GetOffsetFromCenter(player.transform.position);
        Vector3 enemyOffset = zone.GetOffsetFromCenter(transform.position);
        float sign = player.Clockwise ? 1f : -1f;

        float currentAngle = Vector3.SignedAngle(playerOffset, enemyOffset, Vector3.up) * sign;
        if (!_hasOffset)
        {
            _angleOffset = currentAngle;
            _hasOffset = true;
        }

        float angle = Mathf.SmoothDamp(currentAngle, _angleOffset, ref _angleVelocity, _followSmoothTime);
        Vector3 direction = Quaternion.AngleAxis(angle * sign, Vector3.up) * playerOffset.normalized;

        Vector3 position = zone.transform.position + direction * enemyOffset.magnitude;
        position.y = transform.position.y;
        transform.position = position;
    }
}
