using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static EGameState GameState { get; private set; } = EGameState.INTRO;
    

    public enum EGameState
    {
        INTRO, MAINMENU, EARLYGAME, LATEGAME, POSTGAME
    }

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
    /// <param name="state"></param>
    /// <returns></returns>
    public EGameState TransitionToState(EGameState destState)
    {

        switch (GameState)
        {
            case EGameState.INTRO:
                if(destState == EGameState.MAINMENU)
                {
                    return transition(destState);
                }
                else
                {

                    return noTransition(destState);
                }
            case EGameState.MAINMENU:
                break;
            case EGameState.EARLYGAME:
                break;
            case EGameState.LATEGAME:
                break;
            case EGameState.POSTGAME:
                break;
        }
        
        // Returns game state on success otherwise maintains original state.s
        return GameState;
    }

    private void Intro()
    {
        // TODO: Add actual transitions/animations logic. For now it transitions immediately out.
        TransitionToState(EGameState.MAINMENU);
    }
    public void MainMenu()
    {
        // Trigger logic in MainMenuManager while in this mode.
    }
    public void GameRun()
    {
        // Trigger logic in GameRunner while in this mode.
    }
    public void PostGame()
    {
        // Trigger logic in EndingManager while in this mode.
    }

   
}
