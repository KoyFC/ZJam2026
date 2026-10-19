using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class MinimapZoneRingUI : MaskableGraphic
{
    [SerializeField, Min(3)] private int m_Segments = 64;
    [SerializeField] private Color m_InactiveColor = new Color(1f, 1f, 1f, 0.5f);
    [Tooltip("Thickness of the inactive zone outlines, in minimap units.")]
    [SerializeField, Min(0.0f)] private float m_InactiveEdgeThickness = 2.0f;

    protected override void Awake()
    {
        base.Awake();
        raycastTarget = false;
    }

    protected override void Start()
    {
        base.Start();
        if (!MinimapControllerUI.InstanceExists || !ZoneManager.InstanceExists)
            return;

        rectTransform.localPosition = MinimapControllerUI.Instance.WorldToMinimapPosition(ZoneManager.Instance.transform.position);
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper helper)
    {
        helper.Clear();

        if (!Application.isPlaying || !MinimapControllerUI.InstanceExists || !ZoneManager.InstanceExists)
            return;

        var zones = ZoneManager.Instance.Zones;
        int currentZoneIndex = ZoneManager.Instance.CurrentZoneIndex;

        for (int i = 0; i < zones.Length; i++)
        {
            if (i == currentZoneIndex)
                AddFilledZone(helper, i);
            else
                AddZoneOuterEdge(helper, i);
        }
    }

    private void AddFilledZone(VertexHelper helper, int zoneIndex)
    {
        var (innerEllipse, outerEllipse) = GetZoneEllipses(zoneIndex);
        AddRing(helper, innerEllipse, outerEllipse, color);
    }

    private void AddZoneOuterEdge(VertexHelper helper, int zoneIndex)
    {
        var (innerEllipse, outerEllipse) = GetZoneEllipses(zoneIndex);
        var edgeInnerEllipse = Vector2.Max(innerEllipse, outerEllipse - Vector2.one * m_InactiveEdgeThickness);
        AddRing(helper, edgeInnerEllipse, outerEllipse, m_InactiveColor);
    }

    private (Vector2, Vector2) GetZoneEllipses(int zoneIndex)
    {
        Vector2 innerEllipse, outerEllipse;

        var minimap = MinimapControllerUI.Instance;
        float innerRadius = minimap.MinimapToWorldRadius(minimap.GetZoneInnerMinimapRadius(zoneIndex));
        float outerRadius = minimap.MinimapToWorldRadius(ZoneManager.Instance.Zones[zoneIndex].MinimapRadius);

        var scale = minimap.GetWorldToMinimapScale();
        innerEllipse = scale * innerRadius;
        outerEllipse = scale * outerRadius;
        return (innerEllipse, outerEllipse);
    }

    private void AddRing(VertexHelper helper, Vector2 innerEllipse, Vector2 outerEllipse, Color32 ringColor)
    {
        int startIndex = helper.currentVertCount;

        for (int i = 0; i <= m_Segments; i++)
        {
            float angle = (float)i / m_Segments * Mathf.PI * 2f;
            var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            helper.AddVert(Vector2.Scale(direction, innerEllipse), ringColor, Vector4.zero);
            helper.AddVert(Vector2.Scale(direction, outerEllipse), ringColor, Vector4.zero);
        }

        for (int i = 0; i < m_Segments; i++)
        {
            int inner = startIndex + i * 2;
            int outer = inner + 1;
            int nextInner = inner + 2;
            int nextOuter = inner + 3;

            helper.AddTriangle(inner, outer, nextOuter);
            helper.AddTriangle(inner, nextOuter, nextInner);
        }
    }
}
