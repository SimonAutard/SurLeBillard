using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UIElements;

public class NarrationManager
{
    private GameManager _gameManager;

    public NarrationManager(GameManager gameManager)
    {
        _gameManager = gameManager;
        Initialize();
    }

    private void Initialize()
    {
        // subscribe to all events that this component needs to listen to at all time
        EventBus.Subscribe<EventStoryBitGenerationRequest>(HandleStoryBitRequest);
        EventBus.Subscribe<EventStoryUpdateRequest>(HandleStoryUpdateRequest);
        EventBus.Subscribe<EventLoreRequest>(HandleLoreRequest);
    }

    ~NarrationManager()
    {
        // Unsubscribe from all events before getting destroyed to avoid memory leaks
        EventBus.Unsubscribe<EventStoryBitGenerationRequest>(HandleStoryBitRequest);
        EventBus.Unsubscribe<EventStoryUpdateRequest>(HandleStoryUpdateRequest);
        EventBus.Unsubscribe<EventLoreRequest>(HandleLoreRequest);
    }

    /// <summary>
    /// Handles EventStoryUpdateRequest from the reception of the event to the publishing of the delivery
    /// </summary>
    /// <param name="requestEvent">The event containing the data relative to what type of event is requested</param>
    private void HandleStoryBitRequest(EventStoryBitGenerationRequest requestEvent)
    {
        string pos = (requestEvent._positive) ? "positive" : "negative";
        Debug.Log($"NarrationManager: Received a {pos} story bit generation request with '{requestEvent._wordA}' and '{requestEvent._wordB}'");
        string storyBit = GenerateStoryBit(requestEvent._wordA, requestEvent._wordB, requestEvent._positive);
        Debug.Log($"NarrationManager: Publishing story bit generated: {storyBit}");
        EventBus.Publish(new EventStoryBitGenerationDelivery(storyBit));
    }

    /// <summary>
    /// Internal method used to generate a story bit
    /// </summary>
    /// <param name="wordA">The first theme the story bit is based on</param>
    /// <param name="wordB">The second theme the story bit is based on</param>
    /// <param name="positive">States if the story bit should be positive or not</param>
    /// <returns>The generated story bit</returns>
    private string GenerateStoryBit(string wordA, string wordB, bool positive)
    {
        // TODO everything related to the generation of the story bit, be it Cave of Qud algo or LLM prompt
        string pos = (positive) ? "positive" : "negative";
        return $"Placeholder {pos} story bit based on '{wordA}' and '{wordB}'";
    }

    /// <summary>
    /// Example of method that could be used to handle requests to update the lore. Doesn't do anything for now
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleStoryUpdateRequest(EventStoryUpdateRequest requestEvent)
    {
        Debug.Log($"NarrationManager: Updating story with...");
        // TODO update the story with whatever we sent
    }

    /// <summary>
    /// Example of method that could be used to handle requests to deliver lore data (from the reception of the event to the publishing of the delivery). 
    /// Delivers a placeholder string for now
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleLoreRequest(EventLoreRequest requestEvent)
    {
        Debug.Log($"NarrationManager: Received lore request about...");
        string lore = BuildLoreDelivery();
        Debug.Log($"NarrationManager: Publishing lore requested: {lore}");
        EventBus.Publish(new EventLoreDelivery(lore));
    }

    /// <summary>
    /// Example of method that could be used to build the lore data requested, in a specific format
    /// </summary>
    /// <returns>Placeholder string for now</returns>
    private string BuildLoreDelivery()
    {
        // TODO assemble a string or whatever format we decide, containing all the lore info requested.
        string lore = "Placeholder lore";
        return lore;
    }
}
