using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameState : MonoBehaviour
{
    
    private DefinitionRegistry registry;
    protected GameConfig config;
    private static GameState instance;
    public static GameState Instance { get { return instance; } }

    /// <summary>
    /// Information about each plot we currently have. Initialized during entry into Early Game State.
    /// </summary>
    private Plot[] plots;
    public Plot[] GamePlots { get { return plots; }  }
    /// <summary>
    /// The Number of each type of resource currently in the supply
    /// </summary>
    private Dictionary<string, float> resourceSupplyCounts;
    /// <summary>
    /// List of relevant plantTags loaded out of registry during entry into Early Game State.
    /// </summary>
    private string[] plantTags;

    /// <summary>
    /// Set false to pause the simulation, set true to enable the simulation.
    /// </summary>
    public bool isTickerRunning = false;

    /// <summary>
    /// Number of real world seconds per tick.
    /// </summary>
    public float tickDuration = 5;

    public UnityEvent onTick;


    private float _t = 0;


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
        if(GameManager.GameState == GameManager.EGameState.GAME)
        {
            if (Input.GetButtonDown("Jump"))
            {
                TogglePause();
            }

            Ticker();
        }
    }

    public void NewGame()
    {
        print("[GAME STATE] Creating new game!");
        LoadFromRegistry();
    }

    public void TogglePause()
    {
        this.isTickerRunning = !this.isTickerRunning;
        print($"[Game State] Ticker Paused: {isTickerRunning}");
    }
    public void Pause()
    {
        this.isTickerRunning = false;
        print($"[Game State] Ticker Paused: {isTickerRunning}");
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
        resourceSupplyCounts = new Dictionary<string, float>();

        // Apply starting resources
        resourceSupplyCounts[registry.PopulationResourceTagName] = config.startingPopulation;
        resourceSupplyCounts[registry.FoodResourceTagName] = config.startingFood;
        resourceSupplyCounts[registry.FertilizeResouceTagName] = config.startingFertilizer;


    }

    
    private void Ticker()
    {
        if (isTickerRunning)
        {
            _t += Time.deltaTime;
            if(_t > tickDuration)
            {
                _t = 0;
                OnTick();
                onTick.Invoke();
            }
        }
    }


    /// <summary>
    /// Whenever we hit a tick we run a produce operation on all plots.
    /// </summary>
    private void OnTick()
    {
        foreach (Plot p in plots)
        {
            p.Produce();
        }
    }

    public string[] GetResources()
    {
        return this.resourceSupplyCounts.Keys.ToArray();
    }

    public float GetResourceCount(string resource)
    {
        return this.resourceSupplyCounts[resource];
    }
    
    public void Spend(string resource, float count)
    {
        float curCount = this.resourceSupplyCounts[resource];
        if (curCount - count > 0)
        {
            this.resourceSupplyCounts[resource] -= count;
        }
        else
        {
            this.resourceSupplyCounts[resource] = 0;
        }
        
    }

    public Dictionary<string,int> GetUpgradeRequirements(int level)
    {
        level = level - 2;
        if(level >= config.plotUpgradeCosts.Length || level < 0)
        {
            print($"[Game State] Invalid Level: {level} for resource upgrade request.");
            return new Dictionary<string,int>();
        }
        return config.plotUpgradeCosts[level].resourceRequirements;
    }
    public int GetUpgradeCapcityIncrease(int level)
    {
        level = level - 2;
        if (level >= config.plotUpgradeCosts.Length || level < 0)
        {
            print($"[Game State] Invalid Level: {level} for capacity increase request.");
            return 0;
        }
        return config.plotUpgradeCosts[level].newCapacityIncrease;
    }
    public Dictionary<string, int> GetExpansionRequirements(int alreadyExisitingCount)
    {
        return config.plotExpansionCosts[alreadyExisitingCount].resourceRequirements;
    }
    public int GetExpansionCapacityIncrease(int alreaydExistingCount)
    {
        return config.plotExpansionCosts[alreaydExistingCount].newCapacityIncrease;
    }
    public int GetNumberOfActivePlots()
    {
        int count = 0;
        foreach(var plot in this.plots)
        {
            if(plot.level > 0)
            {
                count += 1;
            }
        }
        return count;
    }
    
    private void OnDestroy()
    {
        if (instance != null)
        {
            instance = null;

        }
    }
    
    
    public class Plot
    {

        public int level = 0;
        // inputs per day
        public Dictionary<string, float> inputs;
        // outputs per day
        public Dictionary<string, float> outputs;

        public string plant;
        public int capacityLimit;
        public int assigned;

        public Plot()
        {
            level = 0;
            inputs = new Dictionary<string, float>();
            outputs = new Dictionary<string, float>();
            plant = "";
            capacityLimit = 0;
            assigned = 0;

        }
        /// <summary>
        /// Sets the # of people assigned to current plot. Returns # of overflow people that could not be assigned.
        /// </summary>
        /// <param name="count"># of people being assigned</param>
        /// <returns>Overflow count of people that were not assigned.</returns>
        public int AssignNewAssignmentCount(int count)
        {
            if(count >= capacityLimit)
            {
                this.assigned = capacityLimit;
                return this.capacityLimit - count;
            }
            this.assigned = count;
            return 0;
        }

        private float ScaleOutputByResourceLeft(string resource, float pull, int level)
        {
            //Example: say we pull 15
            float mod = pull * level;
            // but say we only have 10 left.
            float curSupply = Instance.resourceSupplyCounts[resource];
            // neg_overflow = -5
            float neg_overflow = Mathf.Min(0, curSupply - mod);
            // pos_overflow = 5
            float pos_overflow = Mathf.Abs(neg_overflow);
            // if there is no supply don't pull/push resources.
            if (curSupply < 0)
            {
                return 0;
            }
            // perc = 5/15 = 1/3
            float perc = pos_overflow / mod;
            // returns 0.6666 (2/3).
            return 1 - perc;
        }

        public void Produce()
        {
            if(inputs!=null && outputs != null && level >= 1)
            {
                float scale = 1;
                foreach(string input in inputs.Keys)
                {
                    float mod = inputs[input] * level;
                    scale = ScaleOutputByResourceLeft(input, scale, level);
                    print($"[Game State] Plot spent {mod*scale} {input}");
                    Instance.resourceSupplyCounts[input] -= mod*scale;
                }
                print($"[Game State] To produce:");
                foreach(string output in outputs.Keys)
                {
                    float mod = outputs[output] * level*scale;
                    print($"[Game State] {mod} {output}");
                    Instance.resourceSupplyCounts[output] += mod;
                    
                }
            }
        }

        public void SetPlant(string plant)
        {   
            GameConfig.OngoingCost costData = Instance.config.GetResourceOngoingCost(plant);
            this.plant = plant;
            this.inputs = costData.inputResourceCost;
            this.outputs = costData.outputResourceGain;
            print($"[Game State] Set Plot's Plant to {plant}.");
        }

        public void LevelUp()
        {
            Dictionary<string, int> costs;
            if(this.level == 0)
            {
                int activeCount = GameState.Instance.GetNumberOfActivePlots();
                costs = GameState.Instance.GetExpansionRequirements(activeCount);
                this.capacityLimit += GameState.Instance.GetExpansionCapacityIncrease(activeCount);
            }
            else
            {
                costs = GameState.Instance.GetUpgradeRequirements(this.level-1);
                this.capacityLimit += GameState.Instance.GetUpgradeCapcityIncrease(this.level - 1);
            }
            foreach (string name in costs.Keys)
            {
                GameState.Instance.Spend(name,costs[name]);
            }
            
            this.level += 1;
            print($"[Game State] Set Plot's Level to {level}.");
        }
    }


}


