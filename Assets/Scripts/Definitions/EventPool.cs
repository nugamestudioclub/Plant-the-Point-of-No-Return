using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventPool", menuName = "PlantGame/EventPool")]
public class EventPool : ScriptableObject
{
    [SerializeField]
    private string tag;
    [SerializeField]
    private List<RandomEvent> events;
    public List<RandomEvent> Events { get { return events; } }
}
