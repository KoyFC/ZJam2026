using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyGrid : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float m_CellSpacing = 5f;
    [SerializeField, Min(1)] private int m_MaxLanes = 20;
    [SerializeField, Min(1)] private int m_MaxRows = 10;
    [SerializeField, Range(0f, 60f)] private float m_AheadAngle = 15f;

    [SerializeField, Min(0f)] private float m_BarrierMargin = 4f;
    [SerializeField, Min(0f)] private float m_HeightMargin = 3f;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private Explosion m_PlayerExplosion;
    [SerializeField, Min(0f)] private float m_EnemySize = 5f;
#endif

    public int FreeCellCount => m_FreeCells.Count;

    private float[] m_LaneDistances = new float[0];
    private float[] m_RowHeights = new float[0];
    private readonly List<Vector2Int> m_FreeCells = new();

    private int LaneCount => m_LaneDistances.Length;
    private int RowCount => m_RowHeights.Length;

    public float GetLaneDistance(int lane) => m_LaneDistances[lane];
    public float GetRowHeight(int row) => m_RowHeights[row];

    private void Start()
    {
        if (!ZoneManager.InstanceExists || !PlayerRailFollower.InstanceExists)
            return;

        PlayerZoneLimiter zoneLimiter = PlayerRailFollower.Instance.GetComponent<PlayerZoneLimiter>();

        ZoneManager zoneManager = ZoneManager.Instance;
        float minRadius = zoneManager.InnerRadius + m_BarrierMargin;
        float maxRadius = zoneManager.OuterRadius - m_BarrierMargin;
        float minHeight = zoneLimiter.MinYPosition + m_HeightMargin;
        float maxHeight = zoneLimiter.MaxYPosition - m_HeightMargin;

        m_LaneDistances = SpaceCells(minRadius, maxRadius, m_MaxLanes);
        m_RowHeights = SpaceCells(minHeight, maxHeight, m_MaxRows);

        m_FreeCells.Clear();
        for (int lane = 0; lane < LaneCount; lane++)
        {
            for (int row = 0; row < RowCount; row++)
                m_FreeCells.Add(new Vector2Int(lane, row));
        }
    }

    private float[] SpaceCells(float min, float max, int maxCount)
    {
        int count = Mathf.Min(maxCount, Mathf.FloorToInt(Mathf.Max(0f, max - min) / m_CellSpacing) + 1);
        float usedLength = (count - 1) * m_CellSpacing;
        float start = (min + max - usedLength) * 0.5f;

        var cells = new float[count];
        for (int i = 0; i < count; i++)
            cells[i] = start + i * m_CellSpacing;

        return cells;
    }

    public bool TryGetRandomFreeCell(out Vector2Int cell)
    {
        if (m_FreeCells.Count == 0)
        {
            cell = default;
            return false;
        }

        int index = Random.Range(0, m_FreeCells.Count);
        cell = m_FreeCells[index];
        m_FreeCells.RemoveAt(index);
        return true;
    }

    public void ReleaseCell(Vector2Int cell)
    {
        if (!m_FreeCells.Contains(cell))
            m_FreeCells.Add(cell);
    }

    public Vector3 GetCellWorldPosition(Vector2Int cell)
    {
        PlayerRailFollower railFollower = PlayerRailFollower.Instance;
        ZoneManager zoneManager = ZoneManager.Instance;

        float sign = railFollower.Clockwise ? 1f : -1f;
        Vector3 playerDirection = zoneManager.GetOffsetFromCenter(railFollower.transform.position).normalized;
        Vector3 direction = Quaternion.AngleAxis(m_AheadAngle * sign, Vector3.up) * playerDirection;

        Vector3 position = zoneManager.transform.position + direction * GetLaneDistance(cell.x);
        position.y = GetRowHeight(cell.y);
        return position;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (m_PlayerExplosion != null && m_CellSpacing >= m_PlayerExplosion.Radius)
            Debug.LogWarning("Cell Spacing is not lower than the explosion radius, missiles won't hit neighbor cells.", this);

        if (m_CellSpacing < m_EnemySize)
            Debug.LogWarning("Cell Spacing is lower than the enemy size, enemies will overlap.", this);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || !ZoneManager.InstanceExists || !PlayerRailFollower.InstanceExists)
            return;

        for (int lane = 0; lane < LaneCount; lane++)
        {
            for (int row = 0; row < RowCount; row++)
            {
                var cell = new Vector2Int(lane, row);
                Gizmos.color = m_FreeCells.Contains(cell) ? Color.green : Color.red;
                Gizmos.DrawWireSphere(GetCellWorldPosition(cell), m_EnemySize * 0.5f);
            }
        }

        if (m_PlayerExplosion == null || LaneCount == 0)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(GetCellWorldPosition(new Vector2Int(LaneCount / 2, RowCount / 2)), m_PlayerExplosion.Radius);
    }
#endif
}
