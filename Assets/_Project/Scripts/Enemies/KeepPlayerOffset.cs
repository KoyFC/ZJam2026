using UnityEngine;

public class KeepPlayerOffset : MonoBehaviour
{
    [SerializeField] private float _followSmoothTime = 0.3f;

    private bool _hasOffset;
    private float _angleOffset;
    private float _radiusOffset;
    private float _heightOffset;
    private Vector3 _velocity;

    private void Update()
    {
        if (!PlayerRailFollower.InstanceExists || !ZoneManager.InstanceExists)
        {
            return;
        }

        PlayerRailFollower player = PlayerRailFollower.Instance;
        ZoneManager zone = ZoneManager.Instance;
        Vector3 playerOffset = zone.GetOffsetFromCenter(player.transform.position);
        float sign = player.Clockwise ? 1f : -1f;

        if (!_hasOffset)
        {
            Vector3 enemyOffset = zone.GetOffsetFromCenter(transform.position);
            _angleOffset = Vector3.SignedAngle(playerOffset, enemyOffset, Vector3.up) * sign;
            _radiusOffset = enemyOffset.magnitude - playerOffset.magnitude;
            _heightOffset = transform.position.y - player.transform.position.y;
            _hasOffset = true;
        }

        Vector3 direction = Quaternion.AngleAxis(_angleOffset * sign, Vector3.up) * playerOffset.normalized;
        Vector3 desiredPosition = zone.transform.position + direction * (playerOffset.magnitude + _radiusOffset);
        desiredPosition.y = player.transform.position.y + _heightOffset;

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocity, _followSmoothTime);
    }
}
