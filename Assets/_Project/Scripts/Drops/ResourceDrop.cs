using System;
using DG.Tweening;
using UnityEngine;

public enum ResourceDropType
{
    Health,
    Fuel
}

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class ResourceDrop : MonoBehaviour
{
    [SerializeField] private LayerMask m_PlayerMask;
    [SerializeField] private Renderer m_Renderer;
    [SerializeField] private Material m_HealthMaterial;
    [SerializeField] private Material m_FuelMaterial;
    [SerializeField, Min(0f)] private float m_CollectionPadding = 1f;
    [SerializeField, Min(0f)] private float m_MagnetRadius = 12f;
    [SerializeField, Min(0.01f)] private float m_FlySpeed = 18f;
    [SerializeField, Min(0f)] private float m_Lifetime = 30f;
    [SerializeField, Range(-180f, 0f)] private float m_BehindAngle = -10f;

    [Header("Rise")]
    [SerializeField, Min(0f)] private float m_RiseDuration = 1f;
    [SerializeField] private Ease m_RiseEase = Ease.OutQuad;
    [SerializeField, Min(0f)] private float m_BobAmplitude = 0.5f;
    [SerializeField, Min(0.01f)] private float m_BobDuration = 1f;

    public static event Action<float> OnCollected;

    private Pool<ResourceDrop> m_Pool;
    private float m_Amount;
    private float m_Age;
    private ResourceDropType m_CurrentType;
    private Collider m_DropCollider;
    private bool m_Collected;
    private bool m_IsAttracted;
    private CharacterController m_TargetController;

    private void Awake()
    {
        m_DropCollider = GetComponent<Collider>();

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;

        if (m_Renderer == null)
            m_Renderer = GetComponentInChildren<Renderer>();

        if (m_Renderer == null)
            Debug.LogError($"{name}: ResourceDrop requires a Renderer to display its type.", this);
    }

    private void OnDisable()
    {
        transform.DOKill();
    }

    public void Launch(Pool<ResourceDrop> pool, ResourceDropType type, float amount, float height)
    {
        m_Pool = pool;
        m_Amount = amount;
        m_Age = 0f;
        m_Collected = false;
        m_IsAttracted = false;
        m_TargetController = null;
        ApplyType(type);

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
        if (m_Age >= m_Lifetime)
        {
            Release();
            return;
        }

        if (m_IsAttracted)
            FlyToPlayer();
        else
            TryAttractPlayer();

        if (!m_Collected && IsBehindPlayer())
            Release();

        transform.Rotate(Vector3.up, 90f * Time.deltaTime, Space.World);
    }

    private bool IsBehindPlayer()
    {
        if (!ZoneManager.InstanceExists || !PlayerRailFollower.InstanceExists)
            return false;

        return PlayerRailFollower.Instance.GetAngleAhead(transform.position) < m_BehindAngle;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((m_PlayerMask.value & (1 << other.gameObject.layer)) == 0)
            return;

        CharacterController controller = other.GetComponentInParent<CharacterController>();
        if (controller != null)
            BeginAttraction(controller);
    }

    private void TryAttractPlayer()
    {
        if (m_Collected || m_MagnetRadius <= 0f || !PlayerRailFollower.InstanceExists)
            return;

        PlayerRailFollower player = PlayerRailFollower.Instance;
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller == null)
            return;

        Vector3 closestPoint = controller.bounds.ClosestPoint(transform.position);
        if ((closestPoint - transform.position).sqrMagnitude <= m_MagnetRadius * m_MagnetRadius)
            BeginAttraction(controller);
    }

    private void BeginAttraction(CharacterController controller)
    {
        if (m_Collected || m_IsAttracted)
            return;

        m_TargetController = controller;
        m_IsAttracted = true;
        transform.DOKill();
    }

    private void FlyToPlayer()
    {
        if (m_TargetController == null)
        {
            m_IsAttracted = false;
            return;
        }

        Vector3 targetPosition = m_TargetController.bounds.center;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, m_FlySpeed * Time.deltaTime);

        Bounds pickupBounds = m_DropCollider.bounds;
        pickupBounds.Expand(m_CollectionPadding * 2f);
        if (pickupBounds.Intersects(m_TargetController.bounds))
            Collect(m_TargetController.transform);
    }

    private void Collect(Transform player)
    {
        if (m_Collected)
            return;

        switch (m_CurrentType)
        {
            case ResourceDropType.Health:
                HealthController health = player.GetComponentInParent<HealthController>();
                if (health != null)
                    health.Heal(m_Amount);
                else
                    Debug.LogWarning($"{name}: Health drop reached the player but no HealthController was found.", this);
                break;
            case ResourceDropType.Fuel:
                RailFuel fuel = player.GetComponentInParent<RailFuel>();
                if (fuel != null)
                    fuel.Refuel(m_Amount);
                else
                    Debug.LogWarning($"{name}: Fuel drop reached the player but no RailFuel was found.", this);
                break;
        }

        m_Collected = true;
        OnCollected?.Invoke(m_Amount);

        Release();
    }

    private void ApplyType(ResourceDropType type)
    {
        m_CurrentType = type;
        Material material = type == ResourceDropType.Health ? m_HealthMaterial : m_FuelMaterial;
        if (material == null)
        {
            Debug.LogWarning($"{name}: No material is configured for the {type} drop.", this);
            return;
        }

        if (m_Renderer != null)
            m_Renderer.sharedMaterial = material;
    }

    private void Release()
    {
        if (m_Pool != null)
            m_Pool.Release(this);
        else
            Destroy(gameObject);
    }
}
