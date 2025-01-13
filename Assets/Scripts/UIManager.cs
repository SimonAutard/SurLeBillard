using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Design pattern du singleton
    private static UIManager _instance; // instance statique du ui manager

    public static UIManager Instance
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

    // used for debug/testing
    private TextMeshProUGUI _displayText;
    private List<string> _log = new List<string>();

    private void OnEnable()
    {
        // subscribe to all events that this component needs to listen to at all time
        EventBus.Subscribe<EventInitialBreakRequest>(HandleInitialBreakRequest);
        EventBus.Subscribe<EventFeedbackRequest>(HandleFeedbackRequest);
        EventBus.Subscribe<EventNextTurnUIDisplayRequest>(HandleNextTurnUIDisplayRequest);
        EventBus.Subscribe<EventEndGameRoundupRequest>(HandleEndGameRoundupRequest);
    }

    private void OnDisable()
    {
        // Unsubscribe from all events before getting destroyed to avoid memory leaks
        EventBus.Unsubscribe<EventInitialBreakRequest>(HandleInitialBreakRequest);
        EventBus.Unsubscribe<EventFeedbackRequest>(HandleFeedbackRequest);
        EventBus.Unsubscribe<EventNextTurnUIDisplayRequest>(HandleNextTurnUIDisplayRequest);
        EventBus.Unsubscribe<EventEndGameRoundupRequest>(HandleEndGameRoundupRequest);
    }

    private void OnDestroy()
    {
        // Unsubscribe from all events before getting destroyed to avoid memory leaks
        EventBus.Unsubscribe<EventInitialBreakRequest>(HandleInitialBreakRequest);
        EventBus.Unsubscribe<EventFeedbackRequest>(HandleFeedbackRequest);
        EventBus.Unsubscribe<EventNextTurnUIDisplayRequest>(HandleNextTurnUIDisplayRequest);
        EventBus.Unsubscribe<EventEndGameRoundupRequest>(HandleEndGameRoundupRequest);
    }

    /// <summary>
    /// Called by UI when the player actually starts the game (basically: the button "new game" has been clicked)
    /// </summary>
    public void StartNewGame()
    {
        // TODO : UI related stuff if needed
        Debug.Log("UIManager : Requesting the start of a new game");
        EventBus.Publish(new EventNewGameRequest());
    }

    /// <summary>
    /// Displays any needed dialogue between characters during the initial break and play the animation for it
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleInitialBreakRequest(EventInitialBreakRequest requestEvent)
    {
        Debug.Log("UIManager: Displaying everything needed for the initial break.");
        // TODO:
        // - Dialogues and other needed UI elements
        // - Fetch (directly, no event) AI behaviour to know where to place the cue
        // - Automated play animation (non player)
        // - Store the shot angle and force in two floats (can easily be changed as needed, I just went with 2 floats for now)
        
        // placeholder values
        float angle = 0.0f;
        float force = 1.0f;
        Debug.Log("UIManager: Requesting force application.");
        EventBus.Publish(new EventApplyForceToWhiteRequest(angle, force));

    }

    /// <summary>
    /// Display dialogs and updates the UI based on what happenned during the turn
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleFeedbackRequest(EventFeedbackRequest requestEvent)
    {
        Debug.Log("UIManager: Displaying info about what happened during the turn.");
        Debug.Log("UIManager: Displaying dialogs.");
        // TODO
        // - Fetch (directly, no event) needed data from GameStateManager (collisions, pocketings, etc)
        // - Update UI (opacity on pocketed balls, etc)
        // - Fetch (directly, no event) generated prophecies from NarrationManager
        // - Display prophecies recap
        // - Dialogs
        
        Debug.Log("UIManager: Requesting next step.");
        EventBus.Publish(new EventGameloopNextStepRequest());
    }

    /// <summary>
    /// Does everything needed on UI side to handle the start of the turn, depending on the active player
    /// </summary>
    /// <param name="requestEvent">contains _activePlayer</param>
    private void HandleNextTurnUIDisplayRequest(EventNextTurnUIDisplayRequest requestEvent)
    {
        if (requestEvent._activePlayer == ActivePlayerName.Clotho)
        {
            Debug.Log("UIManager: Displaying everything needed for player input");
            // TODO:
            // - Dialogues and other needed UI elements
            // - Cue input handling
            // - Store the shot angle and force in two floats (can easily be changed as needed, I just went with 2 floats for now)
        }
        else
        {
            Debug.Log("UIManager: Displaying everything needed for AI auto play");
            // TODO:
            // - Dialogues and other needed UI elements
            // - Fetch (directly, no event) AI behaviour to know where to place the cue
            // - Automated play animation (non player)
            // - Store the shot angle and force in two floats (can easily be changed as needed, I just went with 2 floats for now)
        }
        // placeholder values
        float angle = 0.0f;
        float force = 1.0f;
        Debug.Log("UIManager: Requesting force application.");
        EventBus.Publish(new EventApplyForceToWhiteRequest(angle, force));
    }

    /// <summary>
    /// Does everything needed on UI side to handle the end of the game
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleEndGameRoundupRequest(EventEndGameRoundupRequest requestEvent)
    {
        Debug.Log("UIManager: Displaying end game recap and dialogs.");
        // TODO:
        // - Display winner (fetch info from GameStateManager)
        // - Display dialogs depending on the winner
        // - Display gameplay recap? (fetch data from GameStateManager)
        // - Display story recap? (fetch data from NarrationManager)

    }
}
