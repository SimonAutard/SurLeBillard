using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameStateManager _gameStateManager;
    private NarrationManager _narrationManager;
    private PhysicsManager _physicsManager;
    private UIManager _UIManager;
    

    private void Awake()
    {
        // initialize all specialized managers
        //_narrationManager = new NarrationManager(this);
        _gameStateManager = new GameStateManager(this);
        _physicsManager = new PhysicsManager(this);
        _UIManager = new UIManager(this);

        // subscribe to all events that this component needs to listen to at all time
        EventBus.Subscribe<EventStoryBitGenerationDelivery>(HandleStoryBitDelivery);
        EventBus.Subscribe<EventLoreDelivery>(HandleLoreDelivery);

    }

    private void OnDestroy()
    {
        // Unsubscribe from all events before getting destroyed to avoid memory leaks
        EventBus.Unsubscribe<EventStoryBitGenerationDelivery>(HandleStoryBitDelivery);
        EventBus.Unsubscribe<EventLoreDelivery>(HandleLoreDelivery);
    }

    private void Start()
    {
        RequestStoryBitGeneration("Love", "Work", true);
        RequestStoryUpdate();
        RequestLoreData();
        RequestStoryBitGeneration("Finances", "Friendship", false);
    }


    //****************************************
    //*** NarrationManager related methods ***
    //****************************************

    /// <summary>
    /// Requests the generation of a story bit
    /// </summary>
    /// <param name="wordA">The first theme the story bit is based on</param>
    /// <param name="wordB">The second theme the story bit is based on</param>
    /// <param name="positive">States if the story bit should be positive or not</param>
    public void RequestStoryBitGeneration(string wordA, string wordB, bool positive)
    {
        string pos = (positive) ? "positive" : "negative";
        Debug.Log($"GameManager: Requesting a {pos} story bit generation with '{wordA}' and '{wordB}'");
        EventBus.Publish(new EventStoryBitGenerationRequest(wordA, wordB, positive));
    }

    /// <summary>
    /// Handles EventStoryBitGenerationDelivery from the reception of the delivery to the publishing of the content. 
    /// For now it only passes the data without adding anything but if we need to do something to the data between the publishing from the NarrativeManager
    /// and the reception from the UIManager, we should do it here
    /// </summary>
    /// <param name="deliveryEvent">The event containing the published story bit</param>
    private void HandleStoryBitDelivery(EventStoryBitGenerationDelivery deliveryEvent)
    {
        Debug.Log($"GameManager: Received story bit: {deliveryEvent._storyBit}");
        // publish event containing the story bit (UIManager would be listening to that one)
    }

    /// <summary>
    /// Example of an event we could use to request an update to the lore. We could add parameters to this depending on what we need to update and how
    /// </summary>
    public void RequestStoryUpdate() // TODO define what we can send to the NarrationManager and in what form
    {
        Debug.Log($"GameManager: Requesting story update with...");
        EventBus.Publish(new EventStoryUpdateRequest());
    }

    /// <summary>
    /// Example of an event we could use to request specific data from the lore. We could add parameters to this depending on what we need
    /// </summary>
    public void RequestLoreData() // TODO define what we can ask and in what form
    {
        Debug.Log($"GameManager: Requesting lore about...");
        EventBus.Publish(new EventLoreRequest());
    }

    /// <summary>
    /// Handles EventLoreDelivery from the reception of the delivery to the publishing of the content.
    /// For now it only passes the data without adding anything but if we need to do something to the data between the publishing from the NarrativeManager
    /// and the reception from the UIManager, we should do it here
    /// </summary>
    /// <param name="responseEvent">The event containing the published lore</param>
    public void HandleLoreDelivery(EventLoreDelivery responseEvent)
    {
        Debug.Log($"GameManager: Received lore: {responseEvent._lore}");
        // publish event containing the lore (UIManager would be listening to that one)
    }

    //****************************************
    //*** GameStateManager related methods ***
    //****************************************



    //**************************************
    //*** PhysicsManager related methods ***
    //**************************************



    //*********************************
    //*** UIManager related methods ***
    //*********************************
}
