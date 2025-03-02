using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;

// *****************************
// >>>>>>>>>> READ ME <<<<<<<<<<
// *****************************
// 
// Events don't do anything by themselves.
// They just carry parameters and are broadcasted, so that any module listening to that kind of event can trigger a specific method (which will have use for these parameters) when the broadcast (publish) happens
//
// Each event is a class in this file and only contains the list of members and a constructor assigning to them the parameters provided when the event is published
// To create a new event :
//  - add a class to this file following the example of other events
//  - start the class name with "Event" (it'll be cleaner for auto completion), and end it with "Request" or "Delivery" when it makes sense (it'll be easier to identify what event does what)
//  - try to keep this file clean by puting the event in the right part of the file (ie: don't put UI events with physics events)
//  - just list the parameters you want the event to carry
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
/// Requests the generation prophecies based on what happened during the turn
/// </summary>
public class EventProphecyGenerationRequest
{

    public EventProphecyGenerationRequest()
    {
        
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

/// <summary>
/// Requests the applications of rules after collisions and/or pocketings have happenned
/// </summary>
public class EventApplyRulesRequest
{
    // collisions info : fastest ball id, slowest ball id, positive or not
    public List<Tuple<int, int, bool>> _collisions { get; set; }
    // pocketings info : ball id, pocket id
    public List<Tuple<int, int>> _pocketings { get; set; }

    public EventApplyRulesRequest(List<Tuple<int, int, bool>> collisions, List<Tuple<int, int>> pocketings)
    {
        _collisions = collisions;
        _pocketings = pocketings;
    }
}

/// <summary>
/// Requests the start of the next player's turn
/// </summary>
public class EventNextPlayerTurnStartRequest
{
    public EventNextPlayerTurnStartRequest()
    {

    }
}

//*************************************
//*** PhysicsManager related events ***
//*************************************

/// <summary>
/// Requests the setup of the scene by placing the balls in their initial position
/// </summary>
public class EventInitialBallsSetupRequest
{
    public EventInitialBallsSetupRequest()
    {

    }
}

/// <summary>
/// Request a force to be applied to the white ball
/// </summary>
public class EventApplyForceToWhiteRequest
{
    public Vector3 _vector { get; set; }
    public float _force {  get; set; }

    public EventApplyForceToWhiteRequest(Vector3 vector, float force)
    {
        _vector = vector;
        _force = force;
    }
}

public class EventCollisionSignal
{
    public int _fastestBall { get; set; }
    public int _slowestBall { get; set; }
    public string _fastestBallTheme { get; set; }  
    public string _slowestBallTheme { get; set; }  
    public bool _valence {  get; set; }

    public EventCollisionSignal(int fastestBall, int slowestBall,string fastestBallTheme, string slowestBallTheme,bool valence)
    {
        _fastestBall = fastestBall;
        _slowestBall = slowestBall;
        _fastestBallTheme = fastestBallTheme;
        _slowestBallTheme = slowestBallTheme;
        _valence = valence;
    }
}

public class EventPocketingSignal
{
    public BallRoll _ball { get; set; }
    public int _pocketID { get; set; }

    public EventPocketingSignal(BallRoll ball, int pocketID)
    {
        _ball = ball;
        _pocketID = pocketID;
    }
}

public class EventBallWasCreated
{
    public GameObject _ball { get; set; }

    public EventBallWasCreated(GameObject ball)
    {
        _ball = ball;
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

/// <summary>
/// Requests to replace the white ball on the table
/// </summary>
public class EventReplaceWhiteRequest
{
    public EventReplaceWhiteRequest()
    {

    }
}

/// <summary>
/// Requests to replace the black ball on the table
/// </summary>
public class EventReplaceBlackRequest
{
    public EventReplaceBlackRequest()
    {

    }
}


/// <summary>
/// Signals when any ball bounces on a band of the billiard
/// </summary>
public class EventBounceOnBandSignal
{
    public EventBounceOnBandSignal()
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

public class EventInitialBreakUIRequest
{
    public EventInitialBreakUIRequest()
    {

    }
}

/// <summary>
/// Requests the display of the UI elements depending on which player it is to play
/// </summary>
public class EventNextTurnUIDisplayRequest
{
    public ActivePlayerName _activePlayer {  get; set; }

    public EventNextTurnUIDisplayRequest(ActivePlayerName activePlayer)
    {
        _activePlayer = activePlayer;
    }
}

/// <summary>
/// Requests feedback after a turn has been solved (dialogs, UI update, etc)
/// </summary>
public class EventFeedbackRequest
{
    public EventFeedbackRequest()
    {

    }
}

/// <summary>
/// Requests the display of everything related to the handling of the end of a game
/// </summary>
public class EventEndGameRoundupRequest
{
    public EventEndGameRoundupRequest()
    {

    }
}

/// <summary>
/// Signals when something is clicked in a menu
/// </summary>
public class EventMenuClickSignal
{
    public EventMenuClickSignal()
    {

    }
}


//********************************
//*** AIManager related events ***
//********************************

/// <summary>
/// Requests for the calculation of the next AI play based on current
/// </summary>
public class EventAIShotRequest
{
    public EventAIShotRequest()
    {

    }
}