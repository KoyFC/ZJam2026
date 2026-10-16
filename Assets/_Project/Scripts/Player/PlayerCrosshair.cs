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

    private Vector2 m_FireScreenPosition;
    private readonly Vector3[] m_Corners = new Vector3[4];

    public Ray FireRay { get; private set; }
    public Vector3 AimPoint { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        m_Camera = Camera.main;

        m_FireScreenPosition = new Vector2(Screen.width, Screen.height) * 0.5f;
    }

    private void OnEnable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;

        PlayerFiringController.OnFired += PunchMarkers;
    }

    private void OnDisable()
    {
        Cursor.visible = true;

        PlayerFiringController.OnFired -= PunchMarkers;

        m_FireMarker.DOKill(true);
        m_CursorMarker.DOKill(true);
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
        if (Mouse.current == null)
            return;

        Rect screen = new Rect(0f, 0f, Screen.width, Screen.height);
        Rect fireArea = m_FireMarkerBoundingBox != null ? GetScreenRect(m_FireMarkerBoundingBox) : GetPivotArea(screen, m_FireMarker);

        Vector2 cursor = ClampInside(Mouse.current.position.ReadValue(), GetPivotArea(screen, m_CursorMarker));
        float lerp = 1f - Mathf.Exp(-m_FollowSpeed * Time.deltaTime);
        m_FireScreenPosition = ClampInside(Vector2.Lerp(m_FireScreenPosition, cursor, lerp), fireArea);

        m_CursorMarker.position = cursor;
        m_FireMarker.position = m_FireScreenPosition;

        FireRay = m_Camera.ScreenPointToRay(m_FireScreenPosition);
        AimPoint = GetAimPoint(FireRay);
    }

    public Quaternion GetAimRotation(Vector3 origin)
    {
        return Quaternion.LookRotation(AimPoint - origin, m_Camera.transform.up);
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
