using UnityEngine;

public class PhysicsManager
{
    private GameManager _gameManager;

    public PhysicsManager(GameManager gameManager)
    {
        _gameManager = gameManager;
        Initialize();
    }

    private void Initialize()
    {
        // subscribe to all events that this component needs to listen to at all time
    }

    ~PhysicsManager()
    {
        // Unsubscribe from all events before getting destroyed to avoid memory leaks
    }

}
