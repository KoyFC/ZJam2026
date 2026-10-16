using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCrosshair : Singleton<PlayerCrosshair>
{
    private Camera m_Camera;
    [SerializeField] private RectTransform m_CursorMarker;
    [SerializeField] private RectTransform m_FireMarker;
    [SerializeField] private RectTransform m_FireMarkerBoundingBox;
    [SerializeField] private float m_FollowSpeed = 8f;

    private Vector2 m_FireScreenPosition;
    private readonly Vector3[] m_Corners = new Vector3[4];

    public Ray FireRay { get; private set; }

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
    }

    private void OnDisable()
    {
        Cursor.visible = true;
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
