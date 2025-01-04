using UnityEngine;

public class GameStateManager
{
    private GameManager _gameManager;

    public GameStateManager(GameManager gameManager)
    {
        _gameManager = gameManager;
        Initialize();
    }

    private void Initialize()
    {
        // subscribe to all events that this component needs to listen to at all time
        // setup the game before it starts (balls, cheats, etc)
    }

    ~GameStateManager()
    {
        // Unsubscribe from all events before getting destroyed to avoid memory leaks
    }
}
