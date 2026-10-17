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

        Vector3 toPlayer = PlayerRailFollower.Instance.transform.position - transform.position;
        if (toPlayer.sqrMagnitude <= Mathf.Epsilon)
        {
            return;
        }

        Quaternion lookRotation = Quaternion.LookRotation(toPlayer, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _turnSpeed * Time.deltaTime);
    }
}
