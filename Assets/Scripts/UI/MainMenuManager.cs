using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private Button startButton;
    [SerializeField]
    private Button settingsButton;
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private RectTransform mainPanel;
    [SerializeField]
    private Canvas menuCanvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startButton.onClick.AddListener(() => {
            GameManager.Instance.TransitionToState(GameManager.EGameState.GAME);
        });
        //settingsButton.onClick.AddListener();
        exitButton.onClick.AddListener(() =>
        {
            GameManager.Instance.TransitionToState(GameManager.EGameState.EXIT);
        });
    }

    public void StartTransition()
    {
        float height = Screen.height;
        Vector3 targetPos = mainPanel.transform.position + Vector3.up * height*2 * menuCanvas.transform.localScale.y;
        StartCoroutine(TransitionScreen(targetPos));
        
    }
    private IEnumerator TransitionScreen(Vector3 targetPos)
    {
        while(Vector3.Distance(targetPos, mainPanel.position) > 0.2)
        {
            yield return null;
            mainPanel.transform.position = Vector3.Lerp(mainPanel.transform.position, targetPos, Time.deltaTime);
        }
        
    }
    public void OpenSettings()
    {

    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
