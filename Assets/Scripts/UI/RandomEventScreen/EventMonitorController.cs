using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventMonitorController : MonoBehaviour
{
    [SerializeField]
    private GameObject NotificationPrefab;

    private List<NotificationScreenUI> screens;
    [SerializeField]
    private float dropDist = 30;

    public static EventMonitorController Instance { get; private set; }

    [SerializeField]
    private Transform contentParent;
    [SerializeField]
    private Transform spawnLoc;

    public bool isInteractable = false;

   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        screens = new List<NotificationScreenUI>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (NotificationScreenUI ui in screens)
        {
            ui.ToggleButton(isInteractable);
        }
        
    }

    public void SubmitEvent(RandomEvent e)
    {
        GameObject spawned = GameObject.Instantiate(NotificationPrefab,contentParent);
        Vector3 curPos = spawned.transform.position;
        spawned.transform.position = spawnLoc.position;
        foreach (NotificationScreenUI notificationScreen in screens)
        {
            notificationScreen.transform.localPosition += Vector3.down*dropDist;
        }
        NotificationScreenUI ui = spawned.GetComponent<NotificationScreenUI>();
        Button notifButton = ui.GetComponent<Button>();
        notifButton.onClick.AddListener(() => {
            ui.Open(ui.transform.position,spawnLoc.position);

        });
       
        ui.LoadContent(e);
        screens.Add(ui);
    }

    private void OnDestroy()
    {
        if (Instance != null)
        {
            Instance = null;

        }
    }

    public void SetInteractable(bool isInteractable)
    {
        this.isInteractable = isInteractable;
        if (isInteractable && GameState.Instance.isTickerRunning)
        {
            GameState.Instance.Pause();
        }
       
    }
}
