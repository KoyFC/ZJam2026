using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    [SerializeField] private float _turnSpeed = 5f;

    private void Update()
    {
        if (!PlayerRailFollower.InstanceExists)
        {
            return;
        }

        Vector3 directionToPlayer = PlayerRailFollower.Instance.transform.position - transform.position;
        directionToPlayer.y = 0f;
        if (directionToPlayer.sqrMagnitude <= Mathf.Epsilon)
        {
            return;
        }

        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _turnSpeed * Time.deltaTime);
    }
}
