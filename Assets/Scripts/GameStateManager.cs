using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private ActivePlayerName _activePlayer = ActivePlayerName.Atropos;
    private bool _gameEnded = false;
    private ActivePlayerName _winner;
    private int _turnCount = 0;

    // Design pattern du singleton
    private static GameStateManager _instance; // instance statique du game state manager

    public static GameStateManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // subscribe to all events that this component needs to listen to at all time
        EventBus.Subscribe<EventNewGameSetupRequest>(HandleNewGameSetupRequest);
        EventBus.Subscribe<EventApplyRulesRequest>(HandleRulesApplicationRequest);
        EventBus.Subscribe<EventNextPlayerTurnStartRequest>(HandleNextPlayerTurnStartRequest);
    }

    private void OnDisable()
    {
        // Unsubscribe from all events to avoid memory leaks
        EventBus.Unsubscribe<EventNewGameSetupRequest>(HandleNewGameSetupRequest);
        EventBus.Unsubscribe<EventApplyRulesRequest>(HandleRulesApplicationRequest);
        EventBus.Unsubscribe<EventNextPlayerTurnStartRequest>(HandleNextPlayerTurnStartRequest);
    }

    private void OnDestroy()
    {
        // Unsubscribe from all events to avoid memory leaks
        EventBus.Unsubscribe<EventNewGameSetupRequest>(HandleNewGameSetupRequest);
        EventBus.Unsubscribe<EventApplyRulesRequest>(HandleRulesApplicationRequest);
        EventBus.Unsubscribe<EventNextPlayerTurnStartRequest>(HandleNextPlayerTurnStartRequest);
    }

    /// <summary>
    /// Sets up the initial state of a game (data only)
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleNewGameSetupRequest(EventNewGameSetupRequest requestEvent)
    {
        // TODO : Setup initial state of the game (balls status, cheats, who plays next, etc)
        _turnCount = 0;
        Debug.Log($"GameStateManager: Initializing game state");
    }

    /// <summary>
    /// Applies rules based on collisions and/or pocketings
    /// </summary>
    /// <param name="requestEvent">contains _collisions and _pocketings</param>
    private void HandleRulesApplicationRequest(EventApplyRulesRequest requestEvent)
    {
        Debug.Log($"GameStateManager: Applying rules and updating game state");
        // TODO:
        // - Store info on last collisions (and maybe pocketings) so it can easily be accessed by the narration manager
        EventBus.Publish(new EventGameloopNextStepRequest());
    }

    /// <summary>
    /// Determines who plays next
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleNextPlayerTurnStartRequest(EventNextPlayerTurnStartRequest requestEvent)
    {
        Debug.Log($"GameStateManager: Determining who's to play next.");
        // TODO:
        // - Check if game has ended or not (and who won if it has)
        // - Determine which player turn it is

        if (_activePlayer == ActivePlayerName.Clotho)
        {
            _turnCount++;
        }

        // Placeholder win condition (autowin after n turns) for testing purpose
        if (_turnCount > 5)
        {
            _gameEnded = true;
            _winner = ActivePlayerName.Clotho;
        }

        if (_gameEnded)
        {
            // TODO "rules" related things to handle the end of the game (if needed)
        }
        else
        {
            // Placeholder basic turn swapping for now
            if (_activePlayer == ActivePlayerName.Atropos)
            {
                _activePlayer = ActivePlayerName.Clotho;
            }
            else
            {
                _activePlayer = ActivePlayerName.Atropos;
                EventBus.Publish(new EventAIShotRequest());
            }
            Debug.Log($"GameStateManager: Requesting the start of the turn on UI side");
            EventBus.Publish(new EventNextTurnUIDisplayRequest(_activePlayer));
        }
    }

    public bool GameHasEnded()
    {
        return _gameEnded;
    }

    
}
