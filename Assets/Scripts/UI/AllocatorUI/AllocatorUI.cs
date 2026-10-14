using System;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AllocatorUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform pointer;

    [SerializeField]
    private Button[] selectors;
    private int selected = -1;
    [SerializeField]
    private RectTransform center;

    [SerializeField]
    private Slider allocationSlider;
    [SerializeField]
    private TMP_Text allocatedValueText;
    [SerializeField]
    private TMP_Text maxValueText;

    private float allocationSliderValue = 0f;
    private float maxValue = 40f;
    private int plotCapacity;

    private int countEnabled = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int index = 0;
        foreach(Button b in selectors)
        {
            b.onClick.AddListener(() =>
            {
                PointPointerAt(b);
            });

            selectors[index].interactable = false;

            index++;
        }
        allocationSlider.onValueChanged.AddListener(OnAllocationChange);
        
    }

    private float GetPopulation()
    {
        return Mathf.Floor(GameState.Instance.GetResourceCount(GameDefinitionRegistry.Instance.Registry.PopulationResourceTagName));
    }
    private float GetAllocated()
    {
        return Mathf.Floor(GameState.Instance.GetResourceCount(GameDefinitionRegistry.Instance.Registry.AllocatedPopulationResourceTagName));
    }

    private void OnSelectPlot(int index)
    {
        int level = GameState.Instance.GamePlots[index].level;
        int capacity = GameState.Instance.GamePlots[index].capacityLimit;
        this.maxValue = Mathf.Min(capacity,GetPopulation());
      
    }

    public void PointPointerAt(Button b)
    {
        Vector3 buttonPos = b.transform.position;
        Vector3 centerPos = center.transform.position;
        float angle = Vector3.Angle(Vector3.left, buttonPos-centerPos);
        pointer.transform.localEulerAngles = new Vector3(0,0, -angle);
        selected= Array.IndexOf(selectors, b);
        OnSelectPlot(selected);
    }

    public void OnAllocationChange(float value)
    {
        if(value > this.GetPopulation())
        {
            value = this.GetPopulation();
        }
        allocationSliderValue = value;
        int assigned = Mathf.RoundToInt((float)allocationSliderValue);
        allocatedValueText.text =assigned.ToString();
        GameState.Instance.GamePlots[selected].AssignNewAssignmentCount(assigned);
    }

    // Update is called once per frame
    void Update()
    {
        maxValueText.text = maxValue.ToString();
        countEnabled = 0;
        int lastSel = -1;
        
        for(int i =0; i < GameState.Instance.GamePlots.Length; i++)
        {
            selectors[i].interactable = GameState.Instance.GamePlots[i].level > 0;
            if (GameState.Instance.GamePlots[i].level > 0)
            {
                countEnabled += 1;
                lastSel = i;
            }
            
        }
        if(selected >= 0)
        {
            this.plotCapacity = GameState.Instance.GamePlots[selected].capacityLimit;
            this.allocationSlider.maxValue = GetPopulation();
            this.allocationSlider.minValue = 0;
        }
            

        allocationSlider.interactable = countEnabled > 0;
        
    }
}
