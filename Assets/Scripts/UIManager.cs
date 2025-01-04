using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private GameManager _gameManager;


    // used for debug/testing
    private TextMeshProUGUI _displayText;
    private List<string> _log = new List<string>();

    private void OnEnable()
    {
        // subscribe to all events that this component needs to listen to at all time
        EventBus.Subscribe<EventDisplayGameUIRequest>(HandleDisplayGameUIRequest);
    }

    private void OnDisable()
    {
        // Unsubscribe from all events before getting destroyed to avoid memory leaks
        EventBus.Unsubscribe<EventDisplayGameUIRequest>(HandleDisplayGameUIRequest);
    }

    /// <summary>
    /// Called by UI when the player actually starts the game
    /// </summary>
    private void StartNewGame()
    {
        // TODO : UI related stuff if needed
        EventBus.Publish(new EventNewGameRequest());
    }

    /// <summary>
    /// Displays/sets up all UI elements that'll be used throughout the game (camera is also setup here).
    /// Anything that needs to be done before the initial break should be done here too.
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleDisplayGameUIRequest(EventDisplayGameUIRequest requestEvent)
    {
        // TODO display UI elements
        // Setup camera
        // Display some dialogue if needed
        EventBus.Publish(new EventGameloopNextStepRequest());
    }
}
