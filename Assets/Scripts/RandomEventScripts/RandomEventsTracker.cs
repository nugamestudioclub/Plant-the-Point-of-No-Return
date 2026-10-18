using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using UnityEngine;

public class RandomEventsTracker : MonoBehaviour
{

    [SerializeField]
    private Dictionary<RandomEvent, int> eventPool;
    private Dictionary<string, EventPool> eventPoolRegistry;
    private DefinitionRegistry registry;

	private void Start()
	{
		registry = GameDefinitionRegistry.Instance.Registry;
	}

	public void AddPool(string eventTag)
    {
        EventPool instance = eventPoolRegistry[eventTag];
        for (int i = 0; i < instance.Events.Count; i++)
        {
            if (!eventPool.Keys.Contains(instance.Events[i]))
            {
                eventPool[instance.Events[i]] = instance.Events[i].BaseWeight;
            }
        }
    }

    public void RemovePool(string eventTag)
    {
        EventPool instance = eventPoolRegistry[eventTag];
        for (int i = 0; i < instance.Events.Count; i++)
        {
            if (eventPool.Keys.Contains(instance.Events[i]))
            {
                eventPool.Remove(instance.Events[i]);
            }
        }
    }

    public void LoadEventsFromRegistry()
    {
        eventPoolRegistry = registry.RandomEvents;
    }

    public RandomEvent PullRandomEvent()
    {
        while (true)
        {
            List<int> eventWeights = eventPool.Values.ToList();
            int sum = 0;

            for (int j = 0; j < eventWeights.Count; j++)
            {
                sum += eventWeights[j];
            }
            int instance = Random.Range(0, sum) + 1;
            for (int i = 0; i < eventPool.Count; i++)
            {
                instance -= eventPool[eventPool.Keys.ToList()[i]];
                if (instance <= 0)
                {
                    return eventPool.Keys.ToList()[i];
                }
            }
        }
    }
    public void ResolveEvent(RandomEvent randEvent, int choice)
    {
        //put in actual stuff of what each key to function does here, in my head its basically just a series of if statements which each call to other functions but idk how best to implement it
        //

        //
        if (randEvent is EventChain chain)
        {
            eventPool.Remove(randEvent);
            if (choice == 0 && chain.Path1 != null)
            {
                //sets the next option to have much higher weight
                eventPool[chain.Path1] = 100;
            }
            if (choice == 1 && chain.Path2 != null)
            {
                eventPool[chain.Path2] = 100;
            }
        }
    }
}
