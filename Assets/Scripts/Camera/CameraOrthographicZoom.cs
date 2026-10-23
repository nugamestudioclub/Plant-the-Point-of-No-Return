using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CameraOrthographicZoom : MonoBehaviour
{
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private float cameraZoomSpeed = 5f;
    [SerializeField] private float zoomedOrthoSize = 3f;   // size used when zoomed in

    public static bool isZoomed = false;

    private Vector3 startingPosition;
    private Vector3 zoomTarget;
    private float targetOrthoSize;
    private bool zoomRequested;

    public UnityEvent onDeselect;

    void Awake()
    {
        startingPosition = transform.position;
        zoomTarget = startingPosition;
        targetOrthoSize = cameraManager.BaseOrthoSize;
    }

    void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame || Input.GetKeyDown(KeyCode.Escape))
        {
            ResetZoom();
        }
    }

    void LateUpdate()
    {
        if (zoomRequested) ZoomCameraToTarget();
    }

    public void OnZoomRequested(Vector3 targetWorldPosition, float zoomOverride = -1)
    {
        cameraManager.IsZoomed = true;

        Vector3 forward = cameraManager.transform.forward;
        Vector3 toTarget = targetWorldPosition - startingPosition;
        float distanceAlongLook = Vector3.Dot(toTarget, forward);

        // Same lateral position as the target, at the camera's starting depth
        zoomTarget = targetWorldPosition - forward * distanceAlongLook;

        targetOrthoSize = zoomOverride > 0 ? zoomOverride : zoomedOrthoSize;
        zoomRequested = true;
        isZoomed = true;
    }

    public void ResetZoom()
    {
        cameraManager.IsZoomed = false;
        zoomTarget = startingPosition;
        targetOrthoSize = cameraManager.BaseOrthoSize;
        zoomRequested = true;
        isZoomed = false;
        onDeselect.Invoke();
    }

    private void ZoomCameraToTarget()
    {
        float t = 1f - Mathf.Exp(-cameraZoomSpeed * Time.deltaTime);

        transform.position = Vector3.Lerp(transform.position, zoomTarget, t);

        float newSize = Mathf.Lerp(cameraManager.GetOrthoSize(), targetOrthoSize, t);
        cameraManager.SetOrthoSize(newSize);

        bool positionDone = Vector3.Distance(transform.position, zoomTarget) < 0.01f;
        bool sizeDone = Mathf.Abs(newSize - targetOrthoSize) < 0.01f;

        if (positionDone && sizeDone)
        {
            transform.position = zoomTarget;
            cameraManager.SetOrthoSize(targetOrthoSize);
            zoomRequested = false;
        }
    }
}