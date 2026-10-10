using UnityEngine;
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
    
    public static ETransitionState TransitionState { get; private set; } = ETransitionState.NONE;

    public enum EGameState
    {
        INTRO, MAINMENU, EARLYGAME, LATEGAME, POSTGAME, EXIT
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
            case EGameState.EARLYGAME:
            case EGameState.LATEGAME:
                GameRun(); break;
            case EGameState.POSTGAME:
                PostGame(); break;
            default: break;
        }

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
                    return transition(destState);
                }
                break;
                
            case EGameState.MAINMENU:
                if (destState == EGameState.EARLYGAME)
                {
                    // Perform animated transition logic.
                    // change state.
                    mainMenuManager.StartTransition();
                    cameraManager.TransitionIn();
                    gameUIManager.TransitionIn();
                    return transition(destState);
                }
                break;
            case EGameState.EARLYGAME:
                break;
            case EGameState.LATEGAME:
                break;
            case EGameState.POSTGAME:
                break;
        }

        // Returns game state on success otherwise maintains original state.
        return noTransition(destState);
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
}
