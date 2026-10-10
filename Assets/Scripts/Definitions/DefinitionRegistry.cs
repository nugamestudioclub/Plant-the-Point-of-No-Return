using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefinitionRegistry", menuName = "PlantGame/DefinitionRegistry")]
public class DefinitionRegistry : ScriptableObject
{
    [SerializeField]
    private Dictionary<string, RandomEvent> randomEvents;
    public Dictionary<string, RandomEvent> RandomEvents { get { return randomEvents; } }

    [SerializeField]
    private Dictionary<string, PlantType> plantTypes;
    public Dictionary<string, PlantType> PlantTypes { get { return plantTypes; } }

    public string PopulationResourceTagName { get; private set; } = "population";
    public string FoodResourceTagName { get; private set; } = "food";
    public string FertilizeResouceTagName { get; private set; } = "fertilizer";

}
