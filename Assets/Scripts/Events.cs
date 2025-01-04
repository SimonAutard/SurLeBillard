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
//  - start the class name with "Event" (it'll be cleaner for auto completion), and end it with "Request" or "Delivery" when it makes sense (it'll be easier to identify what event does what)
//  - try to keep this file clean by puting the event in the right part of the file (ie: don't put UI events with physics events)
//  - just list the parameters you want to the event to carry
//  - be sure to assign the event parameters in the constructor to the class members
//
// To see how to use these events (subscribing, unsubscribing and publishing), check the methods in the EventBus class



//**********************************
//*** GameManager related events ***
//**********************************

/// <summary>
/// Signals that a chain of event is at its end and that the gameloop can proceed with its next step
/// </summary>
public class EventGameloopNextStepRequest
{
    public EventGameloopNextStepRequest()
    {

    }
}


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

/// <summary>
/// Requests the setup of the game in terms of rules
/// </summary>
public class EventNewGameSetupRequest
{
    public EventNewGameSetupRequest()
    {

    }
}

//***********************************
//*** SceneManager related events ***
//***********************************

/// <summary>
/// Requests the initial setup of the scene based on game data
/// </summary>
public class EventSceneInitialSetupRequest
{
    // TODO : Placeholder for now, should be changed depending on what data we need to send to the SceneManager
    public string _gameData { get; }

    public EventSceneInitialSetupRequest(string gameData)
    {
        _gameData = gameData;
    }
}

/// <summary>
/// Requests the initial break to be done
/// </summary>
public class EventInitialBreakRequest
{
    public EventInitialBreakRequest()
    {

    }
}

//********************************
//*** UIManager related events ***
//********************************

/// <summary>
/// Requests the initialization of a new game
/// </summary>
public class EventNewGameRequest
{
    public EventNewGameRequest()
    {

    }
}

/// <summary>
/// Requests the display of the general UI that'll be shown throughout the game, and to activate/place the camera at the scene
/// </summary>
public class EventDisplayGameUIRequest
{
    public EventDisplayGameUIRequest()
    {

    }
}

/// <summary>
/// Requests the display of the UI elements allowing the player to act on his turn
/// </summary>
public class EventDisplayPlayerInputUIRequest
{
    public EventDisplayPlayerInputUIRequest()
    {

    }
}