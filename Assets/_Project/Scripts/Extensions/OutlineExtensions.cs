using DG.Tweening;

public static class OutlineExtensions
{
    public static Tween DOWidth(this Outline outline, float endValue, float duration)
    {
        return DOTween.To(() => outline.OutlineWidth, width => outline.OutlineWidth = width, endValue, duration).SetTarget(outline);
    }
}
