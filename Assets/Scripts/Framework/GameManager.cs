using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static EGameState GameState { get; private set; } = EGameState.INTRO;

    [SerializeField]
    private MainMenuManager mainMenuManager;
    [SerializeField]
    private CameraManager cameraManager;
    [SerializeField]
    private GameUI gameUIManager;
    [SerializeField]
    private GameState gameState;
    [SerializeField]
    private FunctionDelegator delegator;

    [SerializeField]
    private GameDefinitionRegistry gameDefinitionRegistry;
    public GameDefinitionRegistry GameDefinitionRegistry { get { return gameDefinitionRegistry; } }

    [SerializeField]
    private AudioController audioController;
    public AudioController AudioController {  get { return audioController; } }

    
    public static ETransitionState TransitionState { get; private set; } = ETransitionState.NONE;

    public enum EGameState
    {
        INTRO, MAINMENU, GAME, POSTGAME, EXIT
    }
    public enum ETransitionState { NONE, TRANSITION }


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameStateHandler();
    }


    public void GameStateHandler()
    {
        switch (GameState)
        {
            case EGameState.INTRO:
                Intro();
                break;
            case EGameState.MAINMENU:
                MainMenu(); break;
            case EGameState.GAME:
                GameRun(); break;
            case EGameState.POSTGAME:
                PostGame(); break;
            default: break;
        }

    }

   
    /// <summary>
    /// This function handles on transition logic, enforcing our state machine and triggering transition logic for the correct edges.
    /// </summary>
    /// <param name="destState">The Destination state you want to transition to</param>
    /// <returns>The State transitioned into. Returns original state on failure.</returns>
    public EGameState TransitionToState(EGameState destState)
    {
        if(destState == EGameState.EXIT)
        {
            Application.Quit();
        }
        switch (GameState)
        {
            case EGameState.INTRO:
                if (destState == EGameState.MAINMENU)
                {
                    audioController.targetAudio = 0;
                    return transition(destState);
                }
                break;
                
            case EGameState.MAINMENU:
                if (destState == EGameState.GAME)
                {
                    // Perform animated transition logic.
                    // change state.
                    mainMenuManager.StartTransition();
                    cameraManager.TransitionIn();
                    gameUIManager.TransitionIn();
                    gameState.NewGame();
                    audioController.targetAudio = 1;
                    return transition(destState);
                }
                break;
            case EGameState.GAME:
                if(destState == EGameState.POSTGAME)
                {
                    print("[GAME STATE] GAME OVER! TRANSITIONING INTO POST GAME");
                    // Make sure to add game state transition into which ending...
                    gameUIManager.TransitionOut();
                    gameState.TransitionOut();
                    transition(destState);
                }
                break;

            case EGameState.POSTGAME:

                bool goodEnding = !gameState.FertilizerCreatorEnabled;
                //Todo: Call PlayEnding(goodEnding);
                
                return transition(destState);
               
        }

        // Returns game state on success otherwise maintains original state.
        return noTransition(destState);
    }
    private EGameState transition(EGameState state)
    {
        print($"[Game Manager] Transition out of {GameState} into {state}");
        GameState = state;
        return GameState;
    }

    private EGameState noTransition(EGameState s)
    {
        print($"[Game Manager] Transition from {GameState} to {s} failed!");
        return GameState;
    }

    private void Intro()
    {
        TransitionToState(EGameState.MAINMENU);
    }
    public void MainMenu()
    {
        // Trigger logic in MainMenuManager while in this mode on Update Basis.

    }
    public void GameRun()
    {
        // Trigger logic in GameRunner while in this mode on Update Basis.
        if (gameState.isOver())
        {
            TransitionToState(EGameState.POSTGAME);
        }
    }
    public void PostGame()
    {
        // Trigger logic in EndingManager while in this mode on Update Basis.
    }

    private void OnDestroy()
    {
        // Safety check: Clear reference if destroyed by scene changes or other means
        if (Instance == this)
        {
            Instance = null;
            GameState = EGameState.INTRO;
        }
    }


    public void DelegateEventFunction(string name, float val)
    {
        delegator.DelegateFunction(name, val);
    }
}
