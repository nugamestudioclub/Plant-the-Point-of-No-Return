using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameState : MonoBehaviour
{
    
    private DefinitionRegistry registry;
    private GameConfig config;
    private static GameState instance;
    public static GameState Instance { get { return instance; } }

    /// <summary>
    /// Information about each plot we currently have. Initialized during entry into Early Game State.
    /// </summary>
    private Plot[] plots;
    /// <summary>
    /// The Number of each type of resource currently in the supply
    /// </summary>
    private Dictionary<string, int> resourceSupplyCounts;
    /// <summary>
    /// List of relevant plantTags loaded out of registry during entry into Early Game State.
    /// </summary>
    private string[] plantTags;



    private void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        registry = GameDefinitionRegistry.Instance.Registry;
        config = GameDefinitionRegistry.Instance.GameConfig;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewGame()
    {
        print("[GAME STATE] Creating new game!");
        LoadFromRegistry();
    }

    /// <summary>
    /// This function loads the types out of registry to create trackable resource tags for internal tracking.
    /// </summary>

    private void LoadFromRegistry()
    {
        //Create a number of plots equal to teh maxPlotCount and start them at level 0 (aka not in operation).
        int plotCount = config.maxPlotCount;
        plots=  new Plot[plotCount];
        int i;
        for (i = 0; i < plotCount; i++)
        {
            plots[i]= new Plot();
        }
        print($"[Game State] Create {plotCount} plots!");

        // Load relevant Plant Tags
        plantTags = registry.PlantTypes.Keys.ToArray<string>();
        
        // Initialize reosurce tracking.
        resourceSupplyCounts = new Dictionary<string, int>();

        // Apply starting resources
        resourceSupplyCounts[registry.PopulationResourceTagName] = config.startingPopulation;
        resourceSupplyCounts[registry.FoodResourceTagName] = config.startingFood;
        resourceSupplyCounts[registry.FertilizeResouceTagName] = config.startingFertilizer;


    }

    private void OnDestroy()
    {
        if (instance != null)
        {
            instance = null;

        }
    }


}


class Plot
{
    
    public int level = 1;
    // inputs per day
    public Dictionary<string, int> inputs;
    // outputs per day
    public Dictionary<string, int> outputs;

    public Plot()
    {
        level = 0;
        inputs = new Dictionary<string, int>();
        outputs = new Dictionary<string, int>();
    }
}
