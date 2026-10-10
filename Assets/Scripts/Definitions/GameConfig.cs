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

    [System.Serializable]
    public class Cost
    {
        [SerializeField]
        public string milestoneTag;
        [SerializeField]
        public Dictionary<string, int> resourceRequirements;
    }

}
