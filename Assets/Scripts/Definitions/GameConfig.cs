using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "PlantGame/GameConfig")]
public class GameConfig : ScriptableObject
{

    [SerializeField]
    public Cost[] plotUpgradeCosts;

    [SerializeField]
    public Cost[] plotExpansionCosts;

    [SerializeField]
    public Cost[] specialPlotUpgradeCosts;

    [SerializeField]
    public int maxPlotCount = 4;

    [SerializeField]
    public int startingPopulation = 20;
    [SerializeField]
    public int startingFood = 100;
    [SerializeField]
    public int startingFertalizer = 20;

    [System.Serializable]
    public class Cost
    {
        [SerializeField]
        public string milestoneTag;
        [SerializeField]
        public Dictionary<string, int> resourceRequirements;
    }

}
