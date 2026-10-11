using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlotScreenComponent : MonoBehaviour
{

    private int index = -1;

    [SerializeField]
    private RectTransform levelUpScreen;
    [SerializeField]
    private Button levelUpConfirmBtn;
    [SerializeField]
    private TMP_Text levelUpBtnText;
    [SerializeField]
    private TMP_Text levelUpRequirementsText;

    private bool isOpen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelUpScreen.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isOpen && Input.GetMouseButtonDown(0))
        {
            if(EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {

                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };

                
                List<RaycastResult> results = new List<RaycastResult>();

                EventSystem.current.RaycastAll(pointerData, results);

                if (results.Count > 0)
                {
                    GameObject clickedUIObject = results[0].gameObject;

                    // Example: Check if it's a specific button or component
                    if (!clickedUIObject.transform.IsChildOf(this.transform))
                    {
                        ExitView();
                    }
                }

            }
        }
    }

    public void LevelUpRequested(int index, Dictionary<string, int> requirementCounts, string[] availableCounts)
    {
        this.index = index;
        levelUpScreen.gameObject.SetActive(true);

        string requirementsText = "<b>Spend:</b>";
        foreach(string s in requirementCounts.Keys)
        {
            if (availableCounts.Contains(s))
            {
                requirementsText += $"<color=green>{requirementCounts[s]} {s}</color> ";
            }
            else 
            {
                requirementsText += $"<color=red>{requirementCounts[s]} {s}</color> ";
            }
        }
        levelUpRequirementsText.text = requirementsText;

        if(requirementCounts.Count != availableCounts.Length)
        {
            levelUpConfirmBtn.interactable = false;
            levelUpBtnText.text = "Insufficient Resources";
        }
        else
        {
            levelUpConfirmBtn.interactable = true;
            levelUpBtnText.text = "Level Up!";
        }
        levelUpConfirmBtn.onClick.RemoveAllListeners();
        levelUpConfirmBtn.onClick.AddListener(() => {
            GameState.Instance.GamePlots[this.index].SetPlant("potato");
            GameState.Instance.GamePlots[this.index].LevelUp();
            this.ExitView();
        });
        isOpen = true;

    }

    public void ExitView()
    {
        levelUpScreen.gameObject.SetActive(false);
        isOpen= false;
    }
  
}
