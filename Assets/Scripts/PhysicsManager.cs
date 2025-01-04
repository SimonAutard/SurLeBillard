using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    private GameManager _gameManager;

    private void OnEnable()
    {
        // subscribe to all events that this component needs to listen to at all time
    }

    private void OnDisable()
    {
        // Unsubscribe from all events before getting destroyed to avoid memory leaks
    }

}
