using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField, Range(0f, 180f)] private float _maxAheadAngle = 60f;

    public Transform Target { get; private set; }
    public bool HasTarget => Target != null;

    private void Update()
    {
        Target = IsAheadOfPlayer() ? PlayerRailFollower.Instance.transform : null;
    }

    private bool IsAheadOfPlayer()
    {
        if (!PlayerRailFollower.InstanceExists || !ZoneManager.InstanceExists)
        {
            return false;
        }

        PlayerRailFollower player = PlayerRailFollower.Instance;
        ZoneManager zone = ZoneManager.Instance;

        Vector3 playerOffset = zone.GetOffsetFromCenter(player.transform.position);
        Vector3 enemyOffset = zone.GetOffsetFromCenter(transform.position);

        float angle = Vector3.SignedAngle(playerOffset, enemyOffset, Vector3.up);
        if (!player.Clockwise)
        {
            angle = -angle;
        }

        return angle > 0f && angle < _maxAheadAngle;
    }

    private void OnDrawGizmosSelected()
    {
        if (HasTarget)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, Target.position);
        }
    }
}
