using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;

public class TiltButtonTweener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public RectTransform rectTransform;

    [Header("Tilt")]
    public float maxTiltX = 10.0f;
    public float maxTiltY = 10.0f;
    public float tiltDuration = 0.1f;
    public bool pushAway = false;

    private Quaternion originalRotation;
    private Tween tiltTween;
    private bool isHovering;

    private void Awake()
    {
        originalRotation = rectTransform.localRotation;
        isHovering = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        tiltTween.Stop();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        tiltTween.Stop();
        tiltTween = Tween.LocalRotation(rectTransform, originalRotation.eulerAngles, tiltDuration, Ease.OutSine,
            1, CycleMode.Restart, 0.0f, 0.0f, true);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (!isHovering)
            return;

        UpdateTilt(eventData);
    }

    private void UpdateTilt(PointerEventData eventData)
    {
        if (TryCalculateTilt(eventData.position, eventData.enterEventCamera, out Quaternion tiltRotation))
            RotateTween(tiltRotation);
    }

    private bool TryCalculateTilt(Vector2 position, Camera camera, out Quaternion tiltRotation)
    {
        tiltRotation = Quaternion.identity;
        RectTransform rectTransform = transform as RectTransform;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            position,
            camera,
            out Vector2 localPoint
        )) return false;

        Rect rect = rectTransform.rect;
        Vector2 pivot = rectTransform.pivot;

        Vector2 centerOffset = new(rect.width * (0.5f - pivot.x), rect.height * (0.5f - pivot.y));
        Vector2 centerRelativePoint = localPoint - centerOffset;

        float normalizedX = Mathf.Clamp(centerRelativePoint.x / (rect.width * 0.5f), -1.0f, 1.0f);
        float normalizedY = Mathf.Clamp(centerRelativePoint.y / (rect.height * 0.5f), -1.0f, 1.0f);

        float sign = pushAway ? 1.0f : -1.0f;
        float tiltX = normalizedY * maxTiltX * sign;
        float tiltY = normalizedX * maxTiltY * -sign;

        tiltRotation = Quaternion.Euler(tiltX, tiltY, 0.0f);
        return true;
    }

    private void RotateTween(Quaternion newRotation)
    {
        tiltTween.Stop();
        tiltTween = Tween.LocalRotation(rectTransform, newRotation, tiltDuration, Ease.OutSine,
            1, CycleMode.Restart, 0.0f, 0.0f, true);
    }

    private void RotateInstant(Quaternion newRotation)
    {
        tiltTween.Stop();
        rectTransform.localRotation = newRotation;
    }
}
