using UnityEngine;
using UnityEngine.Events;

public class InteractableZoom : MonoBehaviour
{
    [SerializeField] private CameraOrthographicZoom cameraZoomController;
    [SerializeField] private Transform zoomOffset;
    [SerializeField] private float overrideVal = -1;

    [SerializeField] private UnityEvent onSelected;

    public class Comment : MonoBehaviour
    {
        [TextArea(5, 20)]
        public string readMeText;
    }

    void Awake()
    {
        zoomOffset = transform.Find("CameraZoomTarget");
    }

    void OnMouseDown()
    {
        cameraZoomController.OnZoomRequested(zoomOffset.position,overrideVal);
        
        onSelected.Invoke();
        
    }
}
