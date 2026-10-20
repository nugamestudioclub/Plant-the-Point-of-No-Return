using UnityEngine;

public class InteractableZoom : MonoBehaviour
{
    [SerializeField] private CameraOrthographicZoom cameraZoomController;
    [SerializeField] private Transform zoomOffset;

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
        cameraZoomController.OnZoomRequested(zoomOffset.position);
    }
}
