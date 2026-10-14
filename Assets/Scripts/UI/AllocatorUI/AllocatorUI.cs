
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllocatorUI : MonoBehaviour
{
    [SerializeField] private RectTransform pointer;
    [SerializeField] private Button[] selectors;
    [SerializeField] private RectTransform center;

    [SerializeField] private Slider allocationSlider;
    [SerializeField] private TMP_Text allocatedValueText;
    [SerializeField] private TMP_Text maxValueText;

    private int selected = -1;
    private int countEnabled = 0;
    private bool updatingSlider = false;

    private int GetPopulation()
    {
        return Mathf.FloorToInt(
            GameState.Instance.GetResourceCount(
                GameDefinitionRegistry.Instance.Registry.PopulationResourceTagName
            )
        );
    }

    private int GetAllocated()
    {
        return Mathf.FloorToInt(
            GameState.Instance.GetResourceCount(
                GameDefinitionRegistry.Instance.Registry.AllocatedPopulationResourceTagName
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
            selectors[i].onClick.AddListener(
                () => PointPointerAt(selectors[index])
            );
        }

        allocationSlider.wholeNumbers = true;
        allocationSlider.minValue = 0;
        allocationSlider.onValueChanged.AddListener(OnAllocationChange);

        RefreshSelectors();

        
        for (int i = 0; i < selectors.Length; i++)
        {
            if (GameState.Instance.GamePlots[i].level > 0)
            {
                PointPointerAt(selectors[i]);
                break;
            }
        }

        if (selected < 0)
            RefreshSlider();
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

        allocationSlider.interactable =
            countEnabled > 0 && selected >= 0;
    }

    public void PointPointerAt(Button button)
    {
        int index = Array.IndexOf(selectors, button);

        if (index < 0 ||
            index >= GameState.Instance.GamePlots.Length ||
            GameState.Instance.GamePlots[index].level <= 0)
            return;

        selected = index;

        Vector3 direction =
            button.transform.position - center.transform.position;

        Vector3 buttonPos = button.transform.position;
        Vector3 centerPos = center.transform.position;
        float angle = Vector3.Angle(Vector3.left, buttonPos - centerPos);
        pointer.transform.localEulerAngles = new Vector3(0, 0, -angle);

        pointer.localEulerAngles = new Vector3(0, 0, -angle);

        RefreshSlider();
    }

    private void RefreshSlider()
    {
        if (selected < 0 ||
            selected >= GameState.Instance.GamePlots.Length)
        {
            updatingSlider = true;
            allocationSlider.minValue = 0;
            allocationSlider.maxValue = 0;
            allocationSlider.value = 0;
            allocatedValueText.text = "0";
            maxValueText.text = "0";
            updatingSlider = false;
            allocationSlider.interactable = false;
            return;
        }

        int currentAssignment = GetPlotAssignment(selected);
        int maxAssignable = GetMaxAssignable(selected);

        updatingSlider = true;

        allocationSlider.minValue = 0;
        allocationSlider.maxValue = maxAssignable;

        // Normally currentAssignment is already valid. Clamp defensively
        // if other game systems changed population or assignments.
        int safeAssignment = Mathf.Clamp(
            currentAssignment, 0, maxAssignable
        );

        allocationSlider.value = safeAssignment;

        allocatedValueText.text = safeAssignment.ToString();
        maxValueText.text = maxAssignable.ToString();

        updatingSlider = false;
        allocationSlider.interactable = countEnabled > 0;
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

        allocatedValueText.text = assigned.ToString();
        maxValueText.text = maxAssignable.ToString();

        
        RefreshSlider();
    }

    private void Update()
    {
        RefreshSelectors();

        if (selected >= 0)
            RefreshSlider();
    }
}
