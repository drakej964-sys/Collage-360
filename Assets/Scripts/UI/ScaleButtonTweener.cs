using UnityEngine;
using UnityEngine.EventSystems;
using PrimeTween;

public class ScaleButtonTweener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform rectTransform;

    [Header("Scale")]
    public TweenSettings<float> enterScaleTween = new(1.1f, 0.2f, Ease.OutSine, 1, CycleMode.Restart, 0.0f, 0.0f, true);
    public TweenSettings<float> exitScaleTween = new(1.0f, 0.2f, Ease.OutSine, 1, CycleMode.Restart, 0.0f, 0.0f, true);

    private Tween hoverTween;
    private Vector3 originalScale;

    private void Awake()
    {
        if (rectTransform == null)
            rectTransform = transform as RectTransform;

        originalScale = (rectTransform == null) ? rectTransform.localScale : Vector3.one;
    }

    private void OnDisable()
    {
        hoverTween.Stop();

        if (rectTransform != null)
            rectTransform.localScale = originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverTween.Stop();

        if (rectTransform != null)
            hoverTween = Tween.Scale(rectTransform, enterScaleTween);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverTween.Stop();

        if (rectTransform != null)
            hoverTween = Tween.Scale(rectTransform, exitScaleTween);
    }
}
