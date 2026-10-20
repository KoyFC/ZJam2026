using UnityEngine;

[RequireComponent(typeof(EnemyShooter))]
public class BossMovement : MonoBehaviour
{
    [SerializeField] private Vector2 m_MoveSpeed = new Vector2(8f, 4f);
    [SerializeField, Min(0f)] private float m_RingMargin = 20f;
    [SerializeField] private Vector2 m_HeightRange = new Vector2(12f, 40f);
    [SerializeField, Min(0f)] private float m_ShootPause = 0.4f;

    private EnemyShooter m_Shooter;
    private Vector2 m_Target;
    private bool m_HasTarget;
    private float m_ResumeTime;

    private void Awake()
    {
        m_Shooter = GetComponent<EnemyShooter>();
    }

    private void OnEnable()
    {
        m_HasTarget = false;
    }

    private void Update()
    {
        if (!ZoneManager.InstanceExists || !CanMove())
            return;

        ZoneManager zone = ZoneManager.Instance;
        Vector3 offset = zone.GetOffsetFromCenter(transform.position);
        float radius = offset.magnitude;
        if (radius < Mathf.Epsilon)
            return;

        Vector2 current = new Vector2(radius, transform.position.y);
        if (!m_HasTarget || (current - m_Target).sqrMagnitude < 0.01f)
            PickTarget(zone);

        float nextRadius = Mathf.MoveTowards(current.x, m_Target.x, m_MoveSpeed.x * Time.deltaTime);
        float nextHeight = Mathf.MoveTowards(current.y, m_Target.y, m_MoveSpeed.y * Time.deltaTime);

        Vector3 position = zone.transform.position + offset / radius * nextRadius;
        position.y = nextHeight;
        transform.position = position;
    }

    private bool CanMove()
    {
        if (!m_Shooter.enabled || m_Shooter.IsSuspended)
            return false;

        if (m_Shooter.IsBusy)
        {
            m_ResumeTime = Time.time + m_ShootPause;
            return false;
        }

        return Time.time >= m_ResumeTime;
    }

    private void PickTarget(ZoneManager zone)
    {
        float minRadius = zone.InnerRadius + m_RingMargin;
        float maxRadius = Mathf.Max(minRadius, zone.OuterRadius - m_RingMargin);
        m_Target = new Vector2(Random.Range(minRadius, maxRadius), Random.Range(m_HeightRange.x, m_HeightRange.y));
        m_HasTarget = true;
    }
}
