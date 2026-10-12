using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
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
    public int startingFertilizer = 20;
    [SerializeField]
    public OngoingCost[] resourceCosts;
    private Dictionary<string,OngoingCost> resourceCostsDict;

    public OngoingCost GetResourceOngoingCost(string resource)
    {
        if(resourceCostsDict == null)
        {
            resourceCostsDict = new Dictionary<string,OngoingCost>();
            for(int i = 0; i < resourceCosts.Length; i++)
            {
                OngoingCost orc = resourceCosts[i];
                Debug.Log("ORC:"+orc);
                resourceCostsDict[orc.resourceTag] = orc;
            }
        }
        return resourceCostsDict[resource];
    }

    /// <summary>
    /// Cost Represents the purchase cost to purchase something once. (like an upgrade).
    /// Also represents the maximum capacity of workers a plot can take.
    /// </summary>
    [System.Serializable]
    public class Cost
    {
        [SerializeField]
        public string costTag;
        [SerializeField]
        public Dictionary<string, int> resourceRequirements;
        [SerializeField]
        public int newCapacityIncrease;
    }

    /// <summary>
    /// OngoingCost: Represents the input/output cost for ongoing production. ///
    /// <para>- resourceTag: the resource associated with this ongoing cost. Cost scales with this resource.</para>
    /// <para>- costTag: a unique tag for this specific cost requirement.</para>
    /// <para>- inputResourceCost: the input cost for each instance of this resource. (so if you have 20 potatoes, input * 20).</para>
    /// <para>- outputResourceCost: the output cost for each instance of this reousrce. (so if you have 20 potatoes, output * 20).</para>
    /// <para>- functionTags: a list of tags associated with functions that get called due to the existance of this resource and/or ongoing cost. </para>
    /// <para>(for example: this is meant for logic where input/output costs are modified and/or events get triggered due to the existance of this resource).</para>
    /// <para>(direct example: "POTATO_QUEEN" tag added to enable random events associated with the potato queen due to the existance of potato production).</para>
    /// </summary>
    [System.Serializable]
    public class OngoingCost
    {
        [SerializeField]
        public string resourceTag;
        [SerializeField]
        public string costTag;
        [SerializeField]
        public Dictionary<string, float> inputResourceCost;
        [SerializeField]
        public Dictionary<string, float> outputResourceGain;
        [SerializeField]
        public string[] functionTags;
    }

}
