using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    [Header("Target Design Resolution")]
    [SerializeField] private float targetWidth = 1920f;
    [SerializeField] private float targetHeight = 1080f;

    [SerializeField] private Vector2[] bounds = {new Vector2(-0.5f,-0.5f),new Vector2(0.5f,0.5f)};

    private Camera cam;
    private Vector2 mouseTracking;

    void Start()
    {
        cam = GetComponent<Camera>();
        ScaleCamera();

    }

    void ScaleCamera()
    {
        float targetAspect = targetWidth / targetHeight;
        float currentAspect = (float)Screen.width / Screen.height;
        float baseSize = targetHeight / 2f / 100f;

        if (currentAspect < targetAspect)
        {
            cam.orthographicSize = baseSize * (targetAspect / currentAspect);
        }
        else
        {
            cam.orthographicSize = baseSize;
        }
    }

    void ShiftCamera()
    {
        mouseTracking = Input.mousePosition;
        print(mouseTracking);

    }
}
