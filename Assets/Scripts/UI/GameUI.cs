using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    
    [SerializeField]
    private Canvas screensCanvas;

    [SerializeField]
    private List<GameObject> screensObjects;

    [SerializeField]
    private Button[] plotButtons;

    [SerializeField, Tooltip("Sorted in the same order as plots")]
    private Button[] plotUpgradeButtons;

    [SerializeField]
    private TMP_Text resourceTexts;

    [SerializeField, Tooltip("Sorted in the same order as plots")]
    private TMP_Text[] plotLevelTexts;

    [SerializeField]
    private PlotScreenComponent[] plotScreenComponents;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject monitor in screensObjects)
        {
            monitor.SetActive(false);
        }

        screensCanvas.gameObject.SetActive(false);

       
        for (int i = 0; i < plotButtons.Length; i++)
        {
            Button btn = plotUpgradeButtons[i];
            plotUpgradeButtons[i].onClick.AddListener(() =>
            {
                RequestLevelUp(btn);
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.GameState == GameManager.EGameState.GAME)
        {
            DisplayResourceState();
            UpdatePlotState();
            DisplayPlotLevels();
        }
    }

    public void TransitionIn()
    {
        foreach (GameObject monitor in screensObjects)
        {
            monitor.SetActive(true);
        }

        screensCanvas.gameObject.SetActive(true);
    }

    private void DisplayResourceState()
    {
        string displayText = "";
        string[] resources = GameState.Instance.GetResources();
        foreach (string resource in resources)
        {
            float count = Mathf.RoundToInt(GameState.Instance.GetResourceCount(resource));
            displayText += $"<color=white><b>{resource}:";
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

    private void DisplayPlotLevels()
    {
        for (int i = 0; i < plotButtons.Length; i++)
        {
            string text = "Not In Use";
            if (GameState.Instance.GamePlots[i].level == 0)
            {
                plotButtons[i].interactable = false;
                
            }
            else
            {
                plotButtons[i].interactable = true;
                text = $"Level {GameState.Instance.GamePlots[i].level}";
            }
            plotLevelTexts[i].text = text;
        }
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
    private void RequestLevelUp(Button plotUpgradeButton)
    {
        int index = Array.IndexOf(plotUpgradeButtons, plotUpgradeButton);
        GameState.Instance.Pause();
        int level = GameState.Instance.GamePlots[index].level;
        
        Dictionary<string, int> requirements = GameState.Instance.GetUpgradeRequirements(level+1);
        if(level == 0)
        {
            int count = GameState.Instance.GetNumberOfActivePlots();
            requirements = GameState.Instance.GetExpansionRequirements(count);
            
        }
        List<string> availableResources = new List<string>();
        foreach (string resource in requirements.Keys)
        {
            // Check if the resource is sufficient for upgrade.
            if(GameState.Instance.GetResourceCount(resource) >= requirements[resource])
            {
                availableResources.Add(resource);
            }
        }
        plotScreenComponents[index].LevelUpRequested(index, requirements, availableResources.ToArray());
    }
   
}
