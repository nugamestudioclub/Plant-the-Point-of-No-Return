using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    [Header("Target Resolution")]
    [SerializeField] private float targetWidth = 1920f;
    [SerializeField] private float targetHeight = 1080f;

    [Space]
    [Header("Edge Pan Settings")]
    [SerializeField, Range(0.05f, 0.5f)] private float edgeZone = 0.2f; 
    [SerializeField] private float maxShift = 1f;                       
    [SerializeField] private float smoothing = 10f;                     

    public float BaseOrthoSize { get; private set; }
    public bool IsZoomed { get; set; } = false;

    private Camera cam;
    private Vector2 mouseTracking;
    private float baseLocalX;
    private bool isTransitioning;

    void Awake()
    {
        cam = GetComponent<Camera>();
        baseLocalX = transform.localPosition.x;
        ScaleCamera();
    }

    private void Update()
    {
        if (GameManager.GameState == GameManager.EGameState.GAME && !isTransitioning && !IsZoomed)
        {
            ShiftCamera();
        }
    }

    void ScaleCamera()
    {
        float targetAspect = targetWidth / targetHeight;
        float currentAspect = (float)Screen.width / Screen.height;
        float baseSize = targetHeight / 2f / 100f;

        if (currentAspect < targetAspect)
            cam.orthographicSize = baseSize * (targetAspect / currentAspect);
        else
            cam.orthographicSize = baseSize;

        BaseOrthoSize = cam.orthographicSize;
    }

    private void ShiftCamera()
    {
        mouseTracking = Input.mousePosition / new Vector2(Screen.width, Screen.height);
        float mouseX = Mathf.Clamp01(mouseTracking.x);

        float diff = 0f;
        if (mouseX < edgeZone)
            diff = -(edgeZone - mouseX);
        else if (mouseX > 1f - edgeZone)
            diff = mouseX - (1f - edgeZone);
        diff *= maxShift / edgeZone;

        Vector3 local = transform.localPosition;
        Vector3 targetPos = new Vector3(baseLocalX + diff, local.y, local.z);
        float t = 1f - Mathf.Exp(-smoothing * Time.deltaTime);
        transform.localPosition = Vector3.Lerp(local, targetPos, t);
    }

    public void SetOrthoSize(float size)
    {
        cam.orthographicSize = size;
    }

    public float GetOrthoSize()
    {
        return cam.orthographicSize;
    }

    public void TransitionIn()
    {
        Vector3 targetLocal = transform.localPosition;
        transform.localPosition += cam.orthographicSize * 2f * Vector3.up;
        StartCoroutine(RunInTransition(targetLocal));
    }

    private IEnumerator RunInTransition(Vector3 targetLocal)
    {
        isTransitioning = true;

        while (Vector3.Distance(transform.localPosition, targetLocal) > 0.01f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocal, Time.deltaTime);
            yield return null;
        }

        transform.localPosition = targetLocal;
        isTransitioning = false;
    }
}
