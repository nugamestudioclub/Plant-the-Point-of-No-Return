using UnityEngine;

[CreateAssetMenu(fileName = "EventChain", menuName = "PlantGame/EventChain")]
public class EventChain : RandomEvent
{
    //If this event is not the first event in the event chain, set its value to 0. Use EventChain even if it is the last event in that event chain and doesn't lead anywhere.
    [SerializeField]
    private RandomEvent path1 = null;
    [SerializeField]
    private RandomEvent path2 = null;

    public RandomEvent Path1 { get { return path1; } }
    public RandomEvent Path2 { get { return path2; } }
}
