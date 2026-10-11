using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField]
    private Canvas screensCanvas;

    [SerializeField]
    private Button[] plotButtons;

    [SerializeField]
    private Button[] plotUpgradeButtons;

    

    [SerializeField]
    private TMP_Text resourceTexts;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        screensCanvas.gameObject.SetActive(false);

        for (int i = 0; i < plotButtons.Length; i++)
        {
            /*plotButtons[i].onClick.AddListener(() => {
                
            });
            Button b = plotUpgradeButtons[i];
            plotUpgradeButtons[i].onClick.AddListener(() => {
                HandleLevelUp(b);
            });*/
            // not implemented yet.
        }    
    }

    private void HandleLevelUp(Button b)
    {
        int index = Array.IndexOf(plotUpgradeButtons, b);
        GameState.Instance.GamePlots[index].SetPlant("potato");
        GameState.Instance.GamePlots[index].LevelUp(1);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.GameState == GameManager.EGameState.GAME)
        {
            DisplayResourceState();
            UpdatePlotState();
        }
    }

    public void TransitionIn()
    {
        screensCanvas.gameObject.SetActive(true);
    }

    private void DisplayResourceState()
    {
        string displayText = "";
        string[] resources = GameState.Instance.GetResources();
        foreach (string resource in resources)
        {
            float count = GameState.Instance.GetResourceCount(resource);
            displayText += $"<color=black><b>{resource}:";
            if (count < 21) {
                displayText += "<color=red>" + count + "</color>\n";
            } else if (count < 30) {
                displayText += "<color=#B78900>" + count + "</color>\n";
            }
            else
            {
                displayText += "<color=#0A5700>" + count + "</color>\n";
            }
        }
        resourceTexts.text = displayText;
    }

    private void UpdatePlotState()
    {
        for(int i =0;i < plotButtons.Length; i++)
        {
            if (GameState.Instance.GamePlots[i].level == 0)
            {
                plotButtons[i].enabled = false;
            }
            else
            {
                plotButtons[i].enabled = true;
            }
        }
    }
}
