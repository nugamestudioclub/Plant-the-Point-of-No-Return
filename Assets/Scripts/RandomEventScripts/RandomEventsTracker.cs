using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

public class RandomEventsTracker : MonoBehaviour
{

    [SerializeField]
    private Dictionary<RandomEvent, int> eventPool;
    private Dictionary<string, EventPool> eventPoolRegistry;
    private DefinitionRegistry registry;
    private readonly HashSet<RandomEvent> pending = new HashSet<RandomEvent>();

    [SerializeField]
    private UnityEvent onEventPopup;

	private void Start()
	{
		registry = GameDefinitionRegistry.Instance.Registry;
        eventPool = new Dictionary<RandomEvent, int>();
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

    public List<RandomEvent> RemovePool(string eventTag)
    {
        var stale = new List<RandomEvent>();
        foreach (var ev in eventPoolRegistry[eventTag].Events)
        {
            eventPool.Remove(ev);
            if (pending.Remove(ev)) stale.Add(ev);
        }
        return stale;
    }

    public void LoadEventsFromRegistry()
    {
        eventPoolRegistry = registry.RandomEvents;
    }

    public RandomEvent PullRandomEvent()
    {
        
        var candidates = eventPool.Where(kv => kv.Value > 0 && !pending.Contains(kv.Key)).ToList();
        if (candidates.Count == 0) return null;

        int roll = Random.Range(0, candidates.Sum(kv => kv.Value));
        foreach (var kv in candidates)
        {
            roll -= kv.Value;
            if (roll < 0)
            {
                pending.Add(kv.Key);
                onEventPopup.Invoke();
                return kv.Key;
            }
        }
        return null;
    }
    public void ResolveEvent(RandomEvent randEvent, int choice)
    {
        if (randEvent == null)
        {
            print("[RANDOM EVENT TRACKER] Resolve called with NULL event");
            return;
        }
        if (!pending.Remove(randEvent))
        {
            print($"[RANDOM EVENT TRACKER] {randEvent.Title} not pending, ignored");
            return;
        }

        bool first = choice != 1;
        print($"[RANDOM EVENT TRACKER] Resolving {randEvent.Title}, choice {choice}, using {(first ? "Choice1" : "Choice2")}Effects");

        if (randEvent is EventChain chain)
        {
            eventPool.Remove(randEvent);
            RandomEvent next = first ? chain.Path1 : chain.Path2;
            if (next != null) eventPool[next] = 100;
        }

        var effects = first ? randEvent.Choice1Effects : randEvent.Choice2Effects;
        foreach (var kv in effects) resolveEvent(kv.Key, kv.Value);
    }
    private void resolveEvent(string effect, float val)
    {
        if (GameState.Instance.IsResource(effect))
        {
            GameState.Instance.Spend(effect, val);
        }
        else
        {
            GameManager.Instance.DelegateEventFunction(effect, val);
        }
    }
}
