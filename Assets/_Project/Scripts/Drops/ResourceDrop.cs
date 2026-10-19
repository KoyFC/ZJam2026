using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ResourceDrop : MonoBehaviour
{
    [SerializeField] private LayerMask m_PlayerMask;
    [SerializeField, Min(0f)] private float m_Lifetime = 30f;
    [SerializeField, Range(-180f, 0f)] private float m_BehindAngle = -90f;

    [Header("Rise")]
    [SerializeField, Min(0f)] private float m_RiseDuration = 1f;
    [SerializeField] private Ease m_RiseEase = Ease.OutQuad;
    [SerializeField, Min(0f)] private float m_BobAmplitude = 0.5f;
    [SerializeField, Min(0.01f)] private float m_BobDuration = 1f;

    private Pool<ResourceDrop> m_Pool;
    private float m_Amount;
    private float m_Age;

    private void OnDisable()
    {
        transform.DOKill();
    }

    public void Launch(Pool<ResourceDrop> pool, float amount, float height)
    {
        m_Pool = pool;
        m_Amount = amount;
        m_Age = 0f;

        transform.DOKill();
        if (!Mathf.Approximately(height, transform.position.y))
            transform.DOMoveY(height, m_RiseDuration).SetEase(m_RiseEase).OnComplete(Bob);
    }

    private void Bob()
    {
        if (m_BobAmplitude > 0f)
            transform.DOMoveY(m_BobAmplitude, m_BobDuration).SetRelative().SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    private void Update()
    {
        m_Age += Time.deltaTime;
        if (m_Age >= m_Lifetime || IsBehindPlayer())
            Release();

        transform.Rotate(Vector3.up, 90f * Time.deltaTime, Space.World);
    }

    private bool IsBehindPlayer()
    {
        if (!ZoneManager.InstanceExists || !PlayerRailFollower.InstanceExists)
            return false;

        PlayerRailFollower railFollower = PlayerRailFollower.Instance;
        ZoneManager zoneManager = ZoneManager.Instance;

        float sign = railFollower.Clockwise ? 1f : -1f;
        Vector3 playerDirection = zoneManager.GetOffsetFromCenter(railFollower.transform.position);
        Vector3 direction = zoneManager.GetOffsetFromCenter(transform.position);
        return Vector3.SignedAngle(playerDirection, direction, Vector3.up) * sign < m_BehindAngle;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((m_PlayerMask.value & (1 << other.gameObject.layer)) == 0)
            return;

        if (Player_Stats.InstanceExists)
        {
            Player_Stats stats = Player_Stats.Instance;
            stats.SetStat(UpgradeType.resources, stats.CurrentResources + m_Amount);
        }

        Release();
    }

    private void Release()
    {
        if (m_Pool != null)
            m_Pool.Release(this);
        else
            Destroy(gameObject);
    }
}
