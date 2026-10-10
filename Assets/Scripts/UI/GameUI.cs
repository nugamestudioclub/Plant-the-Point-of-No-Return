using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField]
    private Canvas screensCanvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        screensCanvas.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TransitionIn()
    {
        screensCanvas.gameObject.SetActive(true);
    }
}
