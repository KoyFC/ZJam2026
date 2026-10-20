using UnityEngine;

public class PlayerRailFollower : Singleton<PlayerRailFollower>
{
    [SerializeField] private CharacterController m_CharacterController;
    [SerializeField] private float m_ForwardSpeed = 15f;
    [SerializeField, Range(0f, 360f)] private float m_SpawnAngle = 0f;
    [field: SerializeField] public bool Clockwise { get; private set; } = true;

    protected override void Awake()
    {
        base.Awake();

        if (m_CharacterController == null)
            m_CharacterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        SpawnOnRail();
    }

    private void Update()
    {
        if (!ZoneManager.InstanceExists || SceneLoader.IsBusy)
            return;

        ZoneManager zone = ZoneManager.Instance;
        Vector3 offset = zone.GetOffsetFromCenter(transform.position);
        float radius = offset.magnitude;
        if (radius < Mathf.Epsilon)
            return;

        float angle = m_ForwardSpeed * Time.deltaTime / radius * Mathf.Rad2Deg;
        float sign = Clockwise ? 1f : -1f;
        Vector3 nextOffset = Quaternion.AngleAxis(angle * sign, Vector3.up) * offset;
        m_CharacterController.Move(nextOffset - offset);

        transform.rotation = Quaternion.LookRotation(zone.GetTangent(transform.position, Clockwise), Vector3.up);
    }

    public Vector3 GetDirectionAhead(float angle)
    {
        float sign = Clockwise ? 1f : -1f;
        Vector3 direction = ZoneManager.Instance.GetOffsetFromCenter(transform.position).normalized;
        return Quaternion.AngleAxis(angle * sign, Vector3.up) * direction;
    }

    public float GetAngleAhead(Vector3 position)
    {
        ZoneManager zone = ZoneManager.Instance;
        float sign = Clockwise ? 1f : -1f;
        return Vector3.SignedAngle(zone.GetOffsetFromCenter(transform.position), zone.GetOffsetFromCenter(position), Vector3.up) * sign;
    }

    private void SpawnOnRail()
    {
        if (!ZoneManager.InstanceExists)
            return;

        ZoneManager zone = ZoneManager.Instance;
        Vector3 direction = Quaternion.AngleAxis(m_SpawnAngle, Vector3.up) * Vector3.forward;

        Vector3 position = zone.transform.position + direction * zone.GetMiddleRadius();
        position.y = transform.position.y;

        transform.SetPositionAndRotation(position, Quaternion.LookRotation(zone.GetTangent(position, Clockwise), Vector3.up));
        Physics.SyncTransforms();
    }
}
