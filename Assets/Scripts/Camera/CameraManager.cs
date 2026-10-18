using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    [Header("Target Resolution")]
    [SerializeField] private float targetWidth = 1920f;
    [SerializeField] private float targetHeight = 1080f;
    
    [Space]
    [Header("Edge Pan Settings")]
    [SerializeField, Range(0.05f, 0.5f)] private float edgeZone = 0.2f;  //the fraction of screen width on each side that triggers the panning
    [SerializeField] private float maxShift = 1f;                        //max camera offset in world units
    [SerializeField] private float smoothing = 10f;                      //higher value => snappier camera

    private Camera cam;
    private Vector2 mouseTracking;

    void Start()
    {
        cam = GetComponent<Camera>();
        ScaleCamera();
       
    }
    private void Update()
    {
        if (GameManager.GameState == GameManager.EGameState.GAME)
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
        {
            cam.orthographicSize = baseSize * (targetAspect / currentAspect);
        }
        else
        {
            cam.orthographicSize = baseSize;
        }
    }

    private void ShiftCamera()
    {
        mouseTracking = Input.mousePosition / new Vector2(Screen.width, Screen.height);
        float mouseX = Mathf.Clamp01(mouseTracking.x);

        float diff = 0f;
        if (mouseX < edgeZone)
        {
            diff = -(edgeZone - mouseX);
        }
        else if (mouseX > 1f - edgeZone)
        {
            diff = mouseX - (1f - edgeZone);
        }
        diff *= maxShift / edgeZone;

        Vector3 targetPos = new Vector3(diff, cam.transform.position.y, cam.transform.position.z);
        float t = 1f - Mathf.Exp(-smoothing * Time.deltaTime);
        cam.transform.position = Vector3.Lerp(cam.transform.position, targetPos, t);
    }

    public void TransitionIn()
    {
        Vector3 targetPos = cam.transform.position;
        cam.transform.position = cam.transform.position + cam.orthographicSize * Vector3.up*2;
        StartCoroutine(RunInTransition(targetPos));
    }

    private IEnumerator RunInTransition(Vector3 targetPos)
    {
        while(Vector3.Distance(cam.transform.position,targetPos)> 0.01)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, targetPos, Time.deltaTime);
            yield return null;
        }
        cam.transform.position = targetPos;
    }
}
