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
    [SerializeField]
    private Image fieldImage;

    [SerializeField]
    private TMP_Dropdown plantSelectionDropDown;

    private string selectedPlant;

    private bool isUpgradeOpen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelUpScreen.gameObject.SetActive(false);
        selectedPlant = "";
        LoadPlantOptions();

        plantSelectionDropDown.onValueChanged.AddListener((value) => {
            UpdatePlantSelection(value);
        });
    }

    // Update is called once per frame
    void Update()
    {
        if(isUpgradeOpen && Input.GetMouseButtonDown(0))
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

       
        plantSelectionDropDown.gameObject.SetActive(index >= 0 && GameState.Instance.GamePlots[index].level > 0);
        SetFieldImage();
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
        isUpgradeOpen = true;

    }

    public void ExitView()
    {
        levelUpScreen.gameObject.SetActive(false);
        isUpgradeOpen= false;
    }
  
    private void LoadPlantOptions()
    {
        Dictionary<string, PlantType> plantTypes = GameManager.Instance.GameDefinitionRegistry.Registry.PlantTypes;
        string[] names = plantTypes.Keys.ToArray();

        
        List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();
        foreach(string name in names)
        {
            newOptions.Add(new TMP_Dropdown.OptionData(name));
        }

        plantSelectionDropDown.AddOptions(newOptions);
    }

    private void UpdatePlantSelection(int sel)
    {
        string option = plantSelectionDropDown.options[sel].text;
        if (option != null && GameManager.Instance.GameDefinitionRegistry.Registry.PlantTypes.ContainsKey(option))
        {
            selectedPlant = option;
            GameState.Instance.GamePlots[index].SetPlant(option);
        }
    }

    private void SetFieldImage()
    {
        // has a name
        if(selectedPlant.Length>2)
        {
            Sprite image = GameManager.Instance.GameDefinitionRegistry.Registry.PlantTypes[selectedPlant].FieldIcon;
            fieldImage.sprite = image;
        }
    }
}
