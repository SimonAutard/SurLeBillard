using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private GameManager _gameManager;

    private void OnEnable()
    {
        // subscribe to all events that this component needs to listen to at all time
        EventBus.Subscribe<EventGameStateSetup>(InitializeGame);
    }

    private void OnDisable()
    {
        // Unsubscribe from all events to avoid memory leaks
        EventBus.Unsubscribe<EventGameStateSetup>(InitializeGame);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestEvent"></param>
    private void InitializeGame(EventGameStateSetup requestEvent)
    {

    }
    
}
