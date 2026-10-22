
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllocatorUI : MonoBehaviour
{
    [SerializeField] private Transform pointer;
    [SerializeField] private Button[] selectors;
    [SerializeField] private float wheelToPointerRatio = 0.25f;
    private ColorBlock[] defaultColors;

    private int selected = -1;
    private int countEnabled = 0;
    private bool updatingSlider = false;

    private float targetRot = -90;

    private int GetPopulation()
    {
        return Mathf.FloorToInt(
            GameState.Instance.GetResourceCount(
                GameDefinitionRegistry.Instance.Registry.PopulationResourceTagName
            )
        );
    }

    private int GetAssignedElsewhere(int plotIndex)
    {
        int total = 0;

        for (int i = 0; i < GameState.Instance.GamePlots.Length; i++)
        {
            if (i != plotIndex)
                total += GameState.Instance.GamePlots[i].assigned;
        }

        return total;
    }

    private int GetPlotAssignment(int index)
    {
        return GameState.Instance.GamePlots[index].assigned;
    }

    private int GetMaxAssignable(int index)
    {
        var plots = GameState.Instance.GamePlots;

        int population = GetPopulation();
        int assignedElsewhere = GetAssignedElsewhere(index);
        int availableForThisPlot = Mathf.Max(
            0, population - assignedElsewhere
        );

        int capacity = Mathf.Max(0, plots[index].capacityLimit);

        return Mathf.Min(capacity, availableForThisPlot);
    }

    private void Start()
    {
        for (int i = 0; i < selectors.Length; i++)
        {
            int index = i; 

            selectors[i].interactable = false;
            selectors[i].onClick.AddListener(() =>
            {
                selected = index;
                UpdateSelectorVisuals();
                AllocatePercent(GameState.Instance.GetPlotAssignmentPercent(index));

            });
        }
        defaultColors = new ColorBlock[selectors.Length];
        for (int i = 0; i < selectors.Length; i++)
            defaultColors[i] = selectors[i].colors;
    }

    private void RefreshSelectors()
    {
        var plots = GameState.Instance.GamePlots;

        countEnabled = 0;
       
        int count = Mathf.Min(selectors.Length, plots.Length);

        for (int i = 0; i < count; i++)
        {
            bool unlocked = plots[i].level > 0;
            selectors[i].interactable = unlocked;

            if (unlocked)
                countEnabled++;
        }

        if(countEnabled <= 0)
        {
            targetRot = -90;
        }
       
        pointer.transform.localEulerAngles = new Vector3(0, 0, targetRot);
    }

    public void RotatePointerTo(float angle)
    {
        targetRot = angle;
      
    }
    public void AllocatePercent(float perc)
    {
        print("CHANGING ALLOC to:"+perc);
        RotatePointerTo(2f * (perc - 0.5f) * 90f);
    }

    public float ConvertAngleToPercent(float angle)
    {
        return (angle / 180f) + 0.5f;
    }


    public void ChangeByAngle(float wheelDelta)
    {
        if (selected < 0) return; // nothing selected, wheel does nothing

        var plot = GameState.Instance.GamePlots[selected];
        if (plot.capacityLimit <= 0) return;

        float newAngle = Mathf.Clamp(targetRot + wheelDelta * wheelToPointerRatio, -90f, 90f);
        float perc = ConvertAngleToPercent(newAngle);

        // Don't let the pointer go past the number of people actually available.
        float maxPerc = (float)GameState.Instance.GetMaxAssignable(selected) / plot.capacityLimit;
        perc = Mathf.Min(perc, maxPerc);

        GameState.Instance.SetPlotAssignment(selected, Mathf.RoundToInt(perc * plot.capacityLimit));
        AllocatePercent(perc);
    }


    public void OnAllocationChange(float value)
    {
        if (updatingSlider || selected < 0)
            return;

        int maxAssignable = GetMaxAssignable(selected);
        int assigned = Mathf.Clamp(
            Mathf.RoundToInt(value), 0, maxAssignable
        );

       
        GameState.Instance.GamePlots[selected]
            .AssignNewAssignmentCount(assigned);

      

    }

    private void HandleSelection()
    {

    }
    private void UpdateSelectorVisuals()
    {
        for (int i = 0; i < selectors.Length; i++)
        {
            ColorBlock cb = defaultColors[i];

            if (i == selected)
            {
                cb.normalColor = cb.selectedColor;
                cb.highlightedColor = cb.selectedColor; 
            }

            selectors[i].colors = cb;
        }
    }
    private void Update()
    {
        if(GameManager.GameState == GameManager.EGameState.GAME)
        {
            RefreshSelectors();

           // if (selected >= 0)
                //RefreshSlider();
        }
       
    }
}
