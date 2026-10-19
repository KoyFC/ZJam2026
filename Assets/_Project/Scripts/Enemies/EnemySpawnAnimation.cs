using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(EnemyShooter))]
public class EnemySpawnAnimation : MonoBehaviour
{
    [SerializeField] private float m_SpawnDepth = -5f;
    [SerializeField, Min(0.1f)] private float m_RiseSpeed = 10f;
    [SerializeField] private Ease m_SpawnEase = Ease.OutBack;

    [Header("Outline")]
    [SerializeField, Min(0f)] private float m_OutlineWidth = 4f;
    [SerializeField, Min(0f)] private float m_OutlineFadeDuration = 0.3f;
    [SerializeField] private Ease m_OutlineFadeEase = Ease.InQuad;

    private EnemyShooter m_Shooter;
    private Outline m_Outline;

    private void Awake()
    {
        m_Shooter = GetComponent<EnemyShooter>();
        m_Outline = GetComponentInChildren<Outline>();
    }

    private void OnDisable()
    {
        transform.DOKill();
    }

    public void Spawn(Vector3 targetPosition)
    {
        m_Shooter.enabled = false;
        ShowOutline();

        transform.DOKill();
        transform.position = new Vector3(targetPosition.x, m_SpawnDepth, targetPosition.z);
        transform.DOMoveY(targetPosition.y, m_RiseSpeed).SetSpeedBased().SetEase(m_SpawnEase).OnComplete(() =>
        {
            m_Shooter.enabled = true;
            FadeOutline();
        });
    }

    private void ShowOutline()
    {
        if (m_Outline == null)
            return;

        m_Outline.DOKill();
        m_Outline.OutlineWidth = m_OutlineWidth;
    }

    private void FadeOutline()
    {
        if (m_Outline == null)
            return;

        m_Outline.DOKill();
        m_Outline.DOWidth(0f, m_OutlineFadeDuration).SetEase(m_OutlineFadeEase);
    }

    [ContextMenu("Arise")]
    private void SpawnFromGround()
    {
        Spawn(transform.position);
    }
}
