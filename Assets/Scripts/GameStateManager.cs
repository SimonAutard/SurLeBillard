using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private GameManager _gameManager;

    // Design pattern du singleton
    private static GameStateManager _instance; // instance statique du game state manager

    public static GameStateManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void OnEnable()
    {
        // subscribe to all events that this component needs to listen to at all time
        EventBus.Subscribe<EventNewGameSetupRequest>(HandleNewGameSetupRequest);
    }

    private void OnDisable()
    {
        // Unsubscribe from all events to avoid memory leaks
        EventBus.Unsubscribe<EventNewGameSetupRequest>(HandleNewGameSetupRequest);
    }

    /// <summary>
    /// Sets up the initial state of a game (data only) and then publish an event for the scene to be set up based on this state
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleNewGameSetupRequest(EventNewGameSetupRequest requestEvent)
    {
        // TODO : Setup initial state of the game (balls status, cheats, etc)
        Debug.Log($"GameStateManager: Initializing game state");
        // TODO : replace placeholder when data package structure is established
        string placeholder = GenerateGameDataPackage();
        EventBus.Publish(new EventSceneInitialSetupRequest(placeholder));
    }

    /// <summary>
    /// Placeholder method.
    /// TODO : We should have something to create a package of data that could be published with an event.
    /// It might be useful to make it so it can create custom packages depending on parameters (basically what data is needed)
    /// </summary>
    /// <returns>placeholder string</returns>
    private string GenerateGameDataPackage()
    {
        return "Placeholder game data package";
    }
    
}
