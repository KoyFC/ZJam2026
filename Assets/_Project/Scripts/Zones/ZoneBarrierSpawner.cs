using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ZoneBarrierSpawner : MonoBehaviour
{
    [SerializeField] private Material m_BarrierMaterial;
    [SerializeField] private float m_Height = 30f;
    [SerializeField] private float m_YOffset = -2f;
    [SerializeField] private int m_Segments = 64;
    [SerializeField, Min(0f)] private float m_ColorFadeDuration = 0.5f;

    private static readonly int s_ColorId = Shader.PropertyToID("_Color");

    private readonly List<GameObject> m_Barriers = new();
    private Mesh m_CylinderMesh;
    private Mesh m_ColliderMesh;

    private Material m_Material;
    private Color m_OriginalColor;

    private readonly List<Vector3> m_Vertices = new();
    private readonly List<Vector3> m_Normals = new();
    private readonly List<Vector2> m_UVs = new();
    private readonly List<int> m_Triangles = new();

    private void Start()
    {
        if (!ZoneManager.InstanceExists)
            return;

        m_CylinderMesh = CreateCylinder(m_Segments, false);
        m_ColliderMesh = CreateCylinder(m_Segments, true);
        m_Material = new Material(m_BarrierMaterial);
        m_OriginalColor = m_Material.GetColor(s_ColorId);

        float innerRadius = ZoneManager.Instance.InnerRadius;
        if (innerRadius > 0f)
            m_Barriers.Add(CreateZoneBarrier("Inner", innerRadius));
        m_Barriers.Add(CreateZoneBarrier("Outer", ZoneManager.Instance.OuterRadius));

        if (BossSpawner.InstanceExists)
        {
            BossSpawner.Instance.OnBossSpawned += ShowBossColor;
            BossSpawner.Instance.OnBossDefeated += RestoreColor;
        }
    }

    private void OnDestroy()
    {
        if (BossSpawner.InstanceExists)
        {
            BossSpawner.Instance.OnBossSpawned -= ShowBossColor;
            BossSpawner.Instance.OnBossDefeated -= RestoreColor;
        }

        if (m_CylinderMesh != null)
            Destroy(m_CylinderMesh);
        if (m_ColliderMesh != null)
            Destroy(m_ColliderMesh);
        if (m_Material != null)
        {
            m_Material.DOKill();
            Destroy(m_Material);
        }
    }

    private void ShowBossColor()
    {
        FadeColor(ZoneManager.Instance.CurrentZone.BossBarrierColor);
    }

    private void RestoreColor()
    {
        FadeColor(m_OriginalColor);
    }

    private void FadeColor(Color color)
    {
        m_Material.DOKill();
        m_Material.DOColor(color, s_ColorId, m_ColorFadeDuration);
    }

    private GameObject CreateZoneBarrier(string barrierType, float radius)
    {
        var barrier = new GameObject($"{barrierType} Zone Barrier");
        barrier.transform.SetParent(transform, false);
        barrier.transform.position = ZoneManager.Instance.transform.position + Vector3.up * m_YOffset;
        barrier.transform.localScale = new Vector3(radius, m_Height, radius); // Mesh has radius 1 and height 1

        barrier.AddComponent<MeshFilter>().sharedMesh = m_CylinderMesh;

        var meshRenderer = barrier.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = m_Material;
        meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;

        barrier.AddComponent<MeshCollider>().sharedMesh = m_ColliderMesh;

        return barrier;
    }

    // Double sided so projectiles hit the wall from inside and outside the ring
    private Mesh CreateCylinder(int segments, bool isDoubleSided)
    {
        m_Vertices.Clear();
        m_Normals.Clear();
        m_UVs.Clear();
        m_Triangles.Clear();

        for (int column = 0; column <= segments; column++)
        {
            float normalizedAngle = (float)column / segments;
            AddColumn(normalizedAngle);
        }

        for (int column = 0; column < segments; column++)
        {
            ConnectColumns(column, column + 1, isDoubleSided);
        }

        var mesh = new Mesh();
        mesh.SetVertices(m_Vertices);
        mesh.SetNormals(m_Normals);
        mesh.SetUVs(0, m_UVs);
        mesh.SetTriangles(m_Triangles, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    private void AddColumn(float normalizedAngle)
    {
        float angle = normalizedAngle * Mathf.PI * 2f;
        var circlePosition = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));

        // Bottom vertex
        m_Vertices.Add(circlePosition);
        m_Normals.Add(circlePosition);
        m_UVs.Add(new Vector2(normalizedAngle, 0f));

        // Top vertex
        m_Vertices.Add(circlePosition + Vector3.up);
        m_Normals.Add(circlePosition);
        m_UVs.Add(new Vector2(normalizedAngle, 1f));
    }

    // Fills the wall between two columns with two triangles
    private void ConnectColumns(int leftColumn, int rightColumn, bool isDoubleSided)
    {
        int bottomLeft = BottomVertex(leftColumn);
        int topLeft = TopVertex(leftColumn);
        int bottomRight = BottomVertex(rightColumn);
        int topRight = TopVertex(rightColumn);

        AddTriangle(bottomLeft, topLeft, bottomRight);
        AddTriangle(topLeft, topRight, bottomRight);

        if (!isDoubleSided)
            return;

        AddTriangle(bottomRight, topLeft, bottomLeft);
        AddTriangle(bottomRight, topRight, topLeft);
    }

    private void AddTriangle(int a, int b, int c)
    {
        m_Triangles.Add(a);
        m_Triangles.Add(b);
        m_Triangles.Add(c);
    }

    private int BottomVertex(int column) => column * 2;
    private int TopVertex(int column) => column * 2 + 1;
}
