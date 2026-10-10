using System.Collections.Generic;
using UnityEngine;

public class DefinitionRegistry : MonoBehaviour
{
    [SerializeField]
    private Dictionary<string, RandomEvent> randomEvents;
    public Dictionary<string,RandomEvent> RandomEvents { get{ return randomEvents; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
