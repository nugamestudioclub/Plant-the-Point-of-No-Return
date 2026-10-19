using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private CameraManager cameraManager;  // drag the child camera here
    [SerializeField] private float cameraZoomSpeed = 5f;

    private Vector3 startingPosition;
    private Vector3 zoomTarget;
    private float zoomSizeMultiplier = 1f;   // 1 = normal, 0.5 = zoomed in 2x
    private bool zoomRequested;

    void Awake()
    {
        startingPosition = transform.position;
        zoomTarget = startingPosition;
    }

    void Update()
    {
        // Right click: return to the starting view
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            ResetZoom();
        }
    }

    void LateUpdate()
    {
        if (zoomRequested) ZoomCameraToTarget();
    }

    // Call this from other scripts: zoomIn.OnZoomRequested(point, 0.5f);
    public void OnZoomRequested(Vector3 targetPosition, float sizeMultiplier = 1f)
    {
        zoomTarget = targetPosition;
        zoomSizeMultiplier = sizeMultiplier;
        zoomRequested = true;
    }

    public void ResetZoom()
    {
        OnZoomRequested(startingPosition, 1f);
    }

    private void ZoomCameraToTarget()
    {
        float t = 1f - Mathf.Exp(-cameraZoomSpeed * Time.deltaTime);

        // Move the rig
        transform.position = Vector3.Lerp(transform.position, zoomTarget, t);

        // Change the orthographic size (the real "zoom" for an orthographic camera)
        float targetSize = cameraManager.BaseOrthoSize * zoomSizeMultiplier;
        float newSize = Mathf.Lerp(cameraManager.GetOrthoSize(), targetSize, t);
        cameraManager.SetOrthoSize(newSize);

        bool positionDone = Vector3.Distance(transform.position, zoomTarget) < 0.01f;
        bool sizeDone = Mathf.Abs(newSize - targetSize) < 0.01f;

        if (positionDone && sizeDone)
        {
            transform.position = zoomTarget;
            cameraManager.SetOrthoSize(targetSize);
            zoomRequested = false;
        }
    }
}