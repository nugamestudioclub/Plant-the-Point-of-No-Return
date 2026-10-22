using System.Collections;
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
        List<NotificationScreenUI> rem = new List<NotificationScreenUI>();
        foreach (NotificationScreenUI ui in screens)
        {
            ui.ToggleButton(isInteractable);
            if (ui.IsResolved)
            {
               rem.Add(ui);
            }
        }
        foreach(var ui in rem)
        {
            screens.Remove(ui);
            StartCoroutine(WaitDestroy(ui.gameObject));
        }
        // readjust.
        if(rem.Count > 0)
        {
            int y = 0;
            foreach (NotificationScreenUI notificationScreen in screens)
            {
                //notificationScreen.transform.position = spawnLoc.position;
                notificationScreen.UpdatePos(spawnLoc.localPosition + Vector3.down * dropDist * y);
                //notificationScreen.transform.localPosition += Vector3.down * dropDist*y;
                y++;
            }
        }
        
    }

    private IEnumerator WaitDestroy(GameObject ui)
    {
        yield return new WaitForSeconds(1);
        Destroy(ui);
    }

    public void SubmitEvent(RandomEvent e)
    {
        GameObject spawned = GameObject.Instantiate(NotificationPrefab,contentParent);
        Vector3 curPos = spawned.transform.position;
        spawned.transform.position = spawnLoc.position;
        foreach (NotificationScreenUI notificationScreen in screens)
        {
            //notificationScreen.transform.localPosition += Vector3.down*dropDist;
            notificationScreen.UpdatePos(notificationScreen.transform.localPosition + Vector3.down * dropDist);
        }
        NotificationScreenUI ui = spawned.GetComponent<NotificationScreenUI>();
        Button notifButton = ui.GetComponent<Button>();
        notifButton.onClick.AddListener(() => {
            ui.Open(ui.transform.position,spawnLoc.localPosition);

        });
       
        ui.LoadContent(e,spawnLoc.position);
        screens.Add(ui);

        
        int y = 0;
        foreach (NotificationScreenUI notificationScreen in screens)
        {
            notificationScreen.UpdatePos(spawnLoc.localPosition + Vector3.down * dropDist * y);
            y++;
        }
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
