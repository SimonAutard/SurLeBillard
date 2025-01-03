using UnityEngine;

// *****************************
// >>>>>>>>>> READ ME <<<<<<<<<<
// *****************************
// 
// Events don't do anything by themselves.
// They just carry parameters and are broadcasted, so that any module listening to that kind of event can trigger a specific method (which will have use for these parameters)
//
// Each event is a class in this file and only contains the list of members and a constructor assigning to them the parameters provided when the event is published
// To create a new event :
//  - add a class to this file following the example of other events
//  - try to keep this file clean by puting the event in the right part of the file (ie: don't put UI events with physics events)
//  - just list the parameters you want to the event to carry
//  - be sure to assign the event parameters in the constructor to the class members
//
// To see how to use these events (subscribing, unsubscribing and publishing), check the methods in the EventBus class





//***************************************
//*** NarrationManager related events ***
//***************************************

/// <summary>
/// Requests the generation of a story bit and provides the data needed to its generation
/// </summary>
public class EventStoryBitGenerationRequest
{
    // the first theme of the story bit
    public string _wordA { get; }
    // the second theme of the story bit
    public string _wordB { get; }
    // is the story bit positive or not
    public bool _positive { get; }

    public EventStoryBitGenerationRequest(string wordA, string wordB, bool positive)
    {
        _wordA = wordA;
        _wordB = wordB;
        _positive = positive;
    }
}

/// <summary>
/// Signals the delivery of a story bit that has been generated and provides it as a parameter
/// </summary>
public class EventStoryBitGenerationDelivery
{
    // the generated story bit
    public string _storyBit { get; }

    public EventStoryBitGenerationDelivery(string storyBit)
    {
        _storyBit = storyBit;
    }
}

/// <summary>
/// Requests an update to the lore.
/// It's just an example of event with no parameter for now
/// </summary>
public class EventStoryUpdateRequest
{
    // TODO depends on what's in the "lore" and how it's handled

    public EventStoryUpdateRequest()
    {

    }
}

/// <summary>
/// Requests specific data from the lore.
/// It's just an example of event with no parameter for now
/// </summary>
public class EventLoreRequest
{
    // TODO depends on what's in the "lore" and how it's handled

    public EventLoreRequest()
    {

    }
}

/// <summary>
/// Signals the delivery of requested lore and provides it as a parameter
/// </summary>
public class EventLoreDelivery
{
    // TODO depends on what's in the "lore" and how it's handled. It's just a string for now
    public string _lore { get; }

    public EventLoreDelivery(string lore)
    {
        _lore = lore;
    }
}


//***************************************
//*** GameStateManager related events ***
//***************************************



//*************************************
//*** PhysicsManager related events ***
//*************************************



//********************************
//*** UIManager related events ***
//********************************