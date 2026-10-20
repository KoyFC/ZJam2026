using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCrosshair : Singleton<PlayerCrosshair>
{
    private Camera m_Camera;
    [SerializeField] private RectTransform m_CursorMarker;
    [SerializeField] private RectTransform m_FireMarker;
    [SerializeField] private RectTransform m_FireMarkerBoundingBox;
    [SerializeField] private float m_FollowSpeed = 8f;

    [Header("Aim")]
    [SerializeField] private float m_AimDistance = 400f;
    [SerializeField] private LayerMask m_AimMask = ~0;

    [Header("Fire Punch")]
    [SerializeField] private float m_FirePunch = -0.3f;
    [SerializeField] private float m_CursorPunch = -0.2f;
    [SerializeField] private float m_FirePunchDuration = 0.2f;

    [Header("Hit Marker")]
    [SerializeField] private RectTransform m_HitMarker;
    [SerializeField] private float m_HitMarkerDuration = 0.15f;
    [SerializeField] private float m_HitMarkerFade = 0.3f;
    [SerializeField] private float m_HitMarkerScale = 1.5f;

    private CanvasGroup m_HitMarkerGroup;
    private Vector2 m_FireScreenPosition;
    private readonly Vector3[] m_Corners = new Vector3[4];

    public Ray FireRay { get; private set; }
    public Vector3 AimPoint { get; private set; }
    public Vector2 CursorScreenPosition { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        m_FireScreenPosition = new Vector2(Screen.width, Screen.height) * 0.5f;

        if (m_HitMarker != null)
        {
            m_HitMarkerGroup = m_HitMarker.GetComponent<CanvasGroup>();
            if (m_HitMarkerGroup == null)
                m_HitMarkerGroup = m_HitMarker.gameObject.AddComponent<CanvasGroup>();

            m_HitMarkerGroup.alpha = 0f;
        }
    }

    private void OnEnable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;

        PlayerFiringController.OnFired += PunchMarkers;
        EnemyHitFeedback.OnEnemyHit += ShowHitMarker;
    }

    private void OnDisable()
    {
        Cursor.visible = true;

        PlayerFiringController.OnFired -= PunchMarkers;
        EnemyHitFeedback.OnEnemyHit -= ShowHitMarker;

        m_FireMarker.DOKill(true);
        m_CursorMarker.DOKill(true);

        if (m_HitMarker != null)
        {
            m_HitMarker.DOKill(true);
            m_HitMarkerGroup.DOKill(true);
        }
    }

    private void ShowHitMarker()
    {
        if (m_HitMarker == null)
            return;

        m_HitMarker.DOKill();
        m_HitMarkerGroup.DOKill();

        m_HitMarker.localScale = Vector3.one * m_HitMarkerScale;
        m_HitMarker.DOScale(1f, m_HitMarkerDuration);

        m_HitMarkerGroup.alpha = 1f;
        m_HitMarkerGroup.DOFade(0f, m_HitMarkerFade).SetDelay(m_HitMarkerDuration);
    }

    private void PunchMarkers()
    {
        Punch(m_FireMarker, m_FirePunch);
        Punch(m_CursorMarker, m_CursorPunch);
    }

    private void Punch(RectTransform marker, float punch)
    {
        marker.DOKill(true);
        marker.DOPunchScale(Vector3.one * punch, m_FirePunchDuration);
    }

    private void Update()
    {
        RotateMarkers();

        if (Mouse.current == null)
            return;

        Rect screen = new Rect(0f, 0f, Screen.width, Screen.height);
        Rect fireArea = m_FireMarkerBoundingBox != null ? GetScreenRect(m_FireMarkerBoundingBox) : GetPivotArea(screen, m_FireMarker);

        Vector2 cursor = ClampInside(Mouse.current.position.ReadValue(), GetPivotArea(screen, m_CursorMarker));
        float lerp = 1f - Mathf.Exp(-m_FollowSpeed * Time.deltaTime);
        m_FireScreenPosition = ClampInside(Vector2.Lerp(m_FireScreenPosition, cursor, lerp), fireArea);

        CursorScreenPosition = cursor;
        m_CursorMarker.position = cursor;
        m_FireMarker.position = m_FireScreenPosition;

        if (m_HitMarker != null)
            m_HitMarker.position = cursor;

        if (!HasCamera())
            return;

        FireRay = m_Camera.ScreenPointToRay(m_FireScreenPosition);
        AimPoint = GetAimPoint(FireRay);
    }

    private void RotateMarkers()
    {
        if (m_FireMarker != null)
            m_FireMarker.Rotate(0f, 0f, Time.deltaTime * 90f);
        if (m_CursorMarker != null)
            m_CursorMarker.Rotate(0f, 0f, -Time.deltaTime * 45f);
        if (m_HitMarker != null)
            m_HitMarker.Rotate(0f, 0f, Time.deltaTime * 90f);
    }

    public Quaternion GetAimRotation(Vector3 origin)
    {
        Vector3 up = HasCamera() ? m_Camera.transform.up : Vector3.up;
        return Quaternion.LookRotation(AimPoint - origin, up);
    }

    private bool HasCamera()
    {
        if (m_Camera == null)
            m_Camera = Camera.main;

        return m_Camera != null;
    }

    private Vector3 GetAimPoint(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, m_AimDistance, m_AimMask))
            return hit.point;

        return ray.GetPoint(m_AimDistance);
    }

    private Vector2 ClampInside(Vector2 position, Rect area)
    {
        position.x = Mathf.Clamp(position.x, area.xMin, area.xMax);
        position.y = Mathf.Clamp(position.y, area.yMin, area.yMax);
        return position;
    }

    private Rect GetPivotArea(Rect area, RectTransform marker)
    {
        Vector2 size = marker.rect.size * (Vector2)marker.lossyScale;
        Vector2 min = area.min + size * marker.pivot;
        Vector2 max = area.max - size * (Vector2.one - marker.pivot);
        return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
    }

    private Rect GetScreenRect(RectTransform rectTransform)
    {
        rectTransform.GetWorldCorners(m_Corners);
        return Rect.MinMaxRect(m_Corners[0].x, m_Corners[0].y, m_Corners[2].x, m_Corners[2].y);
    }
}
