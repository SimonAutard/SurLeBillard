using UnityEngine;

public class SceneManager : MonoBehaviour
{
    private GameManager _gameManager;

    private void OnEnable()
    {
        // subscribe to all events that this component needs to listen to at all time
        EventBus.Subscribe<EventSceneInitialSetupRequest>(HandleSceneInitialSetupRequest);
        EventBus.Subscribe<EventInitialBreakRequest>(HandleInitialBreakRequest);
    }

    private void OnDisable()
    {
        // Unsubscribe from all events before getting destroyed to avoid memory leaks
        EventBus.Unsubscribe<EventSceneInitialSetupRequest>(HandleSceneInitialSetupRequest);
        EventBus.Unsubscribe<EventInitialBreakRequest>(HandleInitialBreakRequest);
    }

    /// <summary>
    /// Sets up the scene in its initial state (balls position, etc).
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleSceneInitialSetupRequest(EventSceneInitialSetupRequest requestEvent)
    {
        // TODO setup the scene
        EventBus.Publish(new EventDisplayGameUIRequest());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleInitialBreakRequest(EventInitialBreakRequest requestEvent)
    {

    }

}
