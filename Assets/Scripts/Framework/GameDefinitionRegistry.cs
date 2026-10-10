using System.Collections.Generic;
using UnityEngine;

public class GameDefinitionRegistry : MonoBehaviour
{

    [SerializeField]
    private DefinitionRegistry registry;
    public DefinitionRegistry Registry { get { return registry; } }
    [SerializeField]
    private GameConfig config;
    public GameConfig GameConfig { get { return config; } }

    private static GameDefinitionRegistry instance;
    public static GameDefinitionRegistry Instance {  get { return instance; } }

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        if(instance != null)
        {
            instance = null;

        }
    }
}
