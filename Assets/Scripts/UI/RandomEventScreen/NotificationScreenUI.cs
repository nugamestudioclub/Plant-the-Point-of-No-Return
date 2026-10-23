
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationScreenUI : MonoBehaviour
{
    [SerializeField]
    private int targetPage = 0;

    [SerializeField]
    private List<float> pageElevations;
    [SerializeField]
    private RectTransform mainContent;
    [SerializeField]
    private Image exclamation;
    private float targetPos;

    private float parentStartPos;
    /// <summary>
    /// Parent Stored marks where the parent was when moved.
    /// </summary>
    private float parentStoredPos;
    /// <summary>
    /// Parent target pos is current active target for lerping toggled between Start and Stored.
    /// </summary>
    private float parentTargPos;
    [SerializeField]
    private float speed = 4f;
    [SerializeField]
    private float waitDuration = 5;


    [SerializeField]
    private TMP_Text notifTitle;
    [SerializeField]
    private TMP_Text notifText;
    [SerializeField]
    private Image contentImage;
    [SerializeField]
    private Image characterImage;
    [SerializeField]
    public Button optionA;
    [SerializeField]
    public Button optionB;
    private bool selected = false;

    public bool IsResolved { get; private set; }=false;

    private bool isOpen = false;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        if (pageElevations.Count > targetPage)
            targetPos = pageElevations[targetPage];
        

        optionA.onClick.AddListener(() =>
        {
            if (selected)
            {
                return;
            }
            GameState.Instance.EventOptionPicked(1);
            this.Close();
            selected = true;
        });
        optionB.onClick.AddListener(() =>
        {
            if (selected)
            {
                return;
            }
            GameState.Instance.EventOptionPicked(2);
            this.Close();
            selected = true;
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (pageElevations.Count > targetPage)
        {
            targetPos = Mathf.Lerp(targetPos, pageElevations[targetPage], Time.deltaTime * speed);

        }
            
        mainContent.transform.localPosition = new Vector3(mainContent.transform.localPosition.x, targetPos,mainContent.transform.localPosition.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x,parentTargPos,transform.localPosition.z),Time.deltaTime*speed);
        
    }

    public void LoadContent(RandomEvent e, Vector3 targetPos)
    {
        string title = e.Title;
        this.notifTitle.text = title;

        string text = e.TextDisplay;
        this.notifText.text = text;

        Sprite art = e.Art;
        this.contentImage.sprite = art;

        Sprite cg = e.CharacterArt;
        this.characterImage.sprite = cg;

        this.optionA.GetComponent<TMP_Text>().text = "- "+e.Choice1;
        this.optionB.GetComponent<TMP_Text>().text = "- "+e.Choice2;
        parentStartPos = targetPos.y;

    }
    public void ToggleButton(bool interactive)
    {
        if (!this.selected && !this.isOpen)
        {
            this.GetComponent<Button>().enabled = interactive;
        }
        else
        {
            this.GetComponent<Button>().enabled = false;
        }
    }
    public void Open(Vector3 originPos, Vector3 topPos)
    {
        
        ToggleButton(false);
        isOpen = true;

        this.targetPage = 1;
        exclamation.gameObject.SetActive(false);
        StartCoroutine(RunOpenAnim());
        parentStoredPos = transform.localPosition.y;
        
        UpdatePos(topPos);
        GameState.Instance.Pause();
        GameState.Instance.LockPause();
    }

    public void UpdatePos(Vector3 pos)
    {
        parentTargPos = pos.y;
        print("Updated position from:" + parentStoredPos + " to " + parentTargPos);
    }

    public void OpenOptions()
    {
        this.targetPage = 2;
    }
    public void Close()
    {
        this.targetPage = 0;
        selected = true;
        IsResolved = true;
        GameState.Instance.UnlockPause();
    }

    private IEnumerator RunOpenAnim()
    {
        yield return new WaitForSeconds(waitDuration);
        OpenOptions();
    }
}
