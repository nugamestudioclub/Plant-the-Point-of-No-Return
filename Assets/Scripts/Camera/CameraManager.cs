using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    [Header("Target Design Resolution")]
    [SerializeField] private float targetWidth = 1920f;
    [SerializeField] private float targetHeight = 1080f;

    
    private Camera cam;
    private Vector2 mouseTracking;
    private float xDiff = 0;

    void Start()
    {
        cam = GetComponent<Camera>();
        ScaleCamera();
       
    }
    private void Update()
    {
        if ( GameManager.GameState == GameManager.EGameState.EARLYGAME || GameManager.GameState == GameManager.EGameState.LATEGAME)
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
        mouseTracking = Input.mousePosition / new Vector2(Screen.width,Screen.height);
        float diff = 0;
        if (mouseTracking.x < 0.2f)
        {
             diff = 0.2f - mouseTracking.x;
             diff = -diff;
        }
        if(mouseTracking.x > 0.8f)
        {
            diff = mouseTracking.x - 0.8f;

        }
        diff *= 5;
        Vector3 targetPos = new Vector3(diff,cam.transform.position.y,cam.transform.position.z);
        cam.transform.position = Vector3.Lerp(cam.transform.position, targetPos, Time.deltaTime*10f);
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
