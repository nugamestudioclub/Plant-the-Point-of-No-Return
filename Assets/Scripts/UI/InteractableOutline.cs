using UnityEngine;

public class InteractableOutline : MonoBehaviour
{
    [SerializeField] private GameObject outlineObject;

    void Awake()
    {
        outlineObject.SetActive(false);
    }

    void OnMouseOver()
    {
        
        outlineObject.SetActive(true);
    }

    void OnMouseExit()
    {
        outlineObject.SetActive(false);
    }

    private void Update()
    {
        if (CameraOrthographicZoom.isZoomed)
        {
            outlineObject.SetActive(false);
        }
    }
}
