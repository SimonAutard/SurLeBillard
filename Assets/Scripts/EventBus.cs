using System;
using System.Collections.Generic;

public static class EventBus
{
    // Each entry of this dictionary is a list containing all subscribers to a specific type of event
    private static readonly Dictionary<Type, List<Delegate>> _subscribers = new();

    /// <summary>
    /// Subscribing to an event will trigger the specified method when the event is broadcasted
    /// Example of use: EventBus.Subscribe<EventStoryBitGenerationDelivery>(HandleStoryBitDelivery);
    /// EventBus can be called anywhere since it's static.
    /// In this example we subscribe the method HandleStoryBitDelivery to the event EventStoryBitGenerationDelivery.
    /// To see how the events work and to create more, see the READ ME section at the top of the Events.cs file
    /// </summary>
    /// <typeparam name="T">The type of the event you're subscribing to that will trigger the specified method</typeparam>
    /// <param name="handler">The specified method that will be executed when the event is broadcasted</param>
    public static void Subscribe<T>(Action<T> handler) where T : class
    {
        var eventType = typeof(T);
        // if no entry exists for the event type (meaning noone is subscribed to this event yet), an entry is created
        if (!_subscribers.ContainsKey(eventType))
        {
            _subscribers[eventType] = new List<Delegate>();
        }
        _subscribers[eventType].Add(handler);
    }

    /// <summary>
    /// Unsuscribing to an event means that the specified method will no longer trigger when the event is broadcasted (see Subscribe)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="handler"></param>
    public static void Unsubscribe<T>(Action<T> handler) where T : class
    {
        var eventType = typeof(T);
        if (_subscribers.ContainsKey(eventType))
        {
            _subscribers[eventType].Remove(handler);
            // after removing the subscriber from the entry, if the entry doesn't have any subscriber anymore, it's deleted to keep the dictionary clean
            if (_subscribers[eventType].Count == 0)
            {
                _subscribers.Remove(eventType);
            }
        }
    }

    /// <summary>
    /// Publishing an event broadcasts it, meaning that all methods subscribed to it will trigger.
    /// Example of use: EventBus.Publish(new EventStoryBitGenerationRequest(wordA, wordB, positive));
    /// EventBus can be called anywhere since it's static.
    /// The parameter of the Publish method is the constructor of the event with all the necessary parameters.
    /// In this example we publish EventStoryBitGenerationRequest with 3 parameters (wordA, wordB and positive).
    /// Every method previously subscribed to this event will trigger and be able to extract the parameters from the event to use them as they see fit
    /// To see how the events work and to create more, see the READ ME section at the top of the Events.cs file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventToPublish"></param>
    public static void Publish<T>(T eventToPublish) where T : class
    {
        var eventType = typeof(T);
        // if the event we're trying to publish has subscribers (the entry shouldn't exist otherwise)
        if (_subscribers.ContainsKey(eventType))
        {
            // each subscribers to the published event
            foreach (var handler in _subscribers[eventType])
            {
                // run the method associated to this event
                ((Action<T>)handler)?.Invoke(eventToPublish);
            }
        }
    }
}
