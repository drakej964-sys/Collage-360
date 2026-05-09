using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float sensitivity = 1.0f;
    [SerializeField]
    private bool willLockCursorWhileRotatingManually = false;

    private Coroutine rotateRoutine;
    private Vector2 currentDragRotation;

    public event Action OnAutoRotationCompleted;
    public event Action OnAutoRotationStopped;

    public float Sensitivity => sensitivity;

    public static bool IsUserRotatingCamera { get; private set; } = false;
    public static bool IsCameraAutoRotating { get; private set; } = false;

    private void Awake()
    {
        IsUserRotatingCamera = false;
        IsCameraAutoRotating = false;
    }

    private void Start()
    {
        Vector2 rot = transform.rotation.eulerAngles;
        currentDragRotation.x = NormalizeAngle(rot.y);
        currentDragRotation.y = NormalizeAngle(rot.x);
    }

    private void Update()
    {
        if (IsCameraAutoRotating)
            return;

        HandleUserCameraRotation();
    }

    private void HandleUserCameraRotation()
    {
        var mouseButton = Mouse.current.rightButton;

        if (mouseButton.wasPressedThisFrame)
        {
            IsUserRotatingCamera = true;

            Cursor.visible = false;
            if (willLockCursorWhileRotatingManually)
                Cursor.lockState = CursorLockMode.Locked;

            SyncCurrentDragRotationWithCameraRotation();
        }

        if (mouseButton.isPressed)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            float mouseX = mouseDelta.x;
            float mouseY = mouseDelta.y;

            currentDragRotation.x += mouseX * sensitivity;
            currentDragRotation.y -= mouseY * sensitivity;

            currentDragRotation.y = Mathf.Clamp(currentDragRotation.y, -80.0f, 80.0f); // Prevent flipping

            transform.rotation = Quaternion.Euler(currentDragRotation.y, currentDragRotation.x, 0.0f);
        }

        if (mouseButton.wasReleasedThisFrame)
        {
            IsUserRotatingCamera = false;

            Cursor.visible = true;
            if (willLockCursorWhileRotatingManually)
                Cursor.lockState = CursorLockMode.None;

            SyncCurrentDragRotationWithCameraRotation();
        }
    }
    
    public void SetRotation(Quaternion newRotation)
    {
        transform.rotation = newRotation;
        SyncCurrentDragRotationWithCameraRotation();
    }

    public void SetSensitivity(float newSensitivity) => sensitivity = newSensitivity;

    public void PlayRotateTween(Quaternion endValue, float duration)
    {
        StopRotateTween();
        rotateRoutine = StartCoroutine(RotateRoutine(endValue, duration));
    }

    public void StopRotateTween()
    {
        if (rotateRoutine != null)
            StopCoroutine(rotateRoutine);

        rotateRoutine = null;

        SyncCurrentDragRotationWithCameraRotation();

        IsCameraAutoRotating = false;

        OnAutoRotationStopped?.Invoke();
    }

    private IEnumerator RotateRoutine(Quaternion endValue, float duration)
    {
        IsCameraAutoRotating = true;
        IsUserRotatingCamera = false;

        float currentTime = 0.0f;
        Quaternion startValue = transform.localRotation;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;

            float t = currentTime / duration;
            t = Mathf.Clamp01(t);

            transform.localRotation = Quaternion.Slerp(startValue, endValue, t);

            yield return null;
        }

        transform.localRotation = endValue;

        SyncCurrentDragRotationWithCameraRotation();

        IsCameraAutoRotating = false;

        OnAutoRotationCompleted?.Invoke();
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360.0f;
        return (angle > 180.0f) ? angle - 360.0f : angle;
    }

    private void SyncCurrentDragRotationWithCameraRotation()
    {
        Vector2 rot = transform.rotation.eulerAngles;
        currentDragRotation.x = NormalizeAngle(rot.y);
        currentDragRotation.y = NormalizeAngle(rot.x);
    }
}
