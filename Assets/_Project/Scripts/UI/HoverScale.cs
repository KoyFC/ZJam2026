using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField, Min(0f)] private float m_HoverScale = 1.1f;
    [SerializeField, Min(0f)] private float m_Duration = 0.15f;
    [SerializeField] private Ease m_Ease = Ease.OutBack;

    private Vector3 m_BaseScale;

    private void Awake()
    {
        m_BaseScale = transform.localScale;
    }

    private void OnDisable()
    {
        transform.DOKill();
        transform.localScale = m_BaseScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ScaleTo(m_BaseScale * m_HoverScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ScaleTo(m_BaseScale);
    }

    private void ScaleTo(Vector3 scale)
    {
        transform.DOKill();
        transform.DOScale(scale, m_Duration).SetEase(m_Ease).SetUpdate(true).SetLink(gameObject);
    }
}
