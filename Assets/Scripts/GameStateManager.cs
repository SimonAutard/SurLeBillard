using System.Collections.Generic;
using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private ActivePlayerName _activePlayer = ActivePlayerName.Atropos;
    private ActivePlayerName _player1 = ActivePlayerName.Clotho;
    private ActivePlayerName _player2 = ActivePlayerName.Atropos;
    private List<PenaltyType> _currentTurnPenalties = new List<PenaltyType>();
    public bool _gameInitialised {  get; private set; }
    private bool _gameEnded = false;
    public ActivePlayerName _winner { get; private set; }
    private int _turnCount = 0;
    private List<int> _ballsInPlay = new List<int>();
    private List<int> _ballsPocketed = new List<int>();
    private List<Tuple<int, int, bool>> _lastCollisions = new List<Tuple<int, int, bool>>();
    private List<Tuple<int, int, bool>> _gameCollisions = new List<Tuple<int, int, bool>>();
    private List<Tuple<int, int>> _lastPocketings = new List<Tuple<int, int>>();
    private List<Tuple<int, int>> _gamePocketings = new List<Tuple<int, int>>();

    //Paramètres des billes
    public int whiteBallID { get; private set; }
    public int blackBallID { get; private set; }

    // Design pattern du singleton
    private static GameStateManager _instance; // instance statique du game state manager

    public static GameStateManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        whiteBallID = 0;
        blackBallID = 4;
    }

    private void OnEnable()
    {
        // subscribe to all events that this component needs to listen to at all time
        EventBus.Subscribe<EventNewGameSetupRequest>(HandleNewGameSetupRequest);
        EventBus.Subscribe<EventApplyRulesRequest>(HandleRulesApplicationRequest);
        EventBus.Subscribe<EventNextPlayerTurnStartRequest>(HandleNextPlayerTurnStartRequest);
    }

    private void OnDisable()
    {
        // Unsubscribe from all events to avoid memory leaks
        EventBus.Unsubscribe<EventNewGameSetupRequest>(HandleNewGameSetupRequest);
        EventBus.Unsubscribe<EventApplyRulesRequest>(HandleRulesApplicationRequest);
        EventBus.Unsubscribe<EventNextPlayerTurnStartRequest>(HandleNextPlayerTurnStartRequest);
    }

    private void OnDestroy()
    {
        // Unsubscribe from all events to avoid memory leaks
        EventBus.Unsubscribe<EventNewGameSetupRequest>(HandleNewGameSetupRequest);
        EventBus.Unsubscribe<EventApplyRulesRequest>(HandleRulesApplicationRequest);
        EventBus.Unsubscribe<EventNextPlayerTurnStartRequest>(HandleNextPlayerTurnStartRequest);
    }

    /// <summary>
    /// Sets up the initial state of a game (data only)
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleNewGameSetupRequest(EventNewGameSetupRequest requestEvent)
    {
        // TODO : Setup initial state of the game (balls status, cheats, who plays next, etc)
        // clearing the lists and then filling the balls in play with ids from 0 to 15 (0 is the white ball, 8 is the black ball)
        _ballsInPlay.Clear();
        _ballsPocketed.Clear();
        _lastCollisions.Clear();
        _gameCollisions.Clear();
        _lastPocketings.Clear();
        _gamePocketings.Clear();
        for(int i = 0; i < 16; i++)
        {
            _ballsInPlay.Add(i);
        }
        _turnCount = 0;
        _gameInitialised = true;
        _gameEnded = false;
        _activePlayer = ActivePlayerName.Atropos;
        Debug.Log($"GameStateManager: Initializing game state");
    }

    /// <summary>
    /// Applies rules based on collisions and/or pocketings
    /// </summary>
    /// <param name="requestEvent">contains _collisions and _pocketings</param>
    private void HandleRulesApplicationRequest(EventApplyRulesRequest requestEvent)
    {
        Debug.Log($"GameStateManager: Applying rules and updating game state");
        _lastCollisions.Clear();
        _lastPocketings.Clear();
        foreach (Tuple<int, int, bool> collision in requestEvent._collisions)
        {
            if (collision.Item1 != 0 && collision.Item2 != 0)
            {
                _lastCollisions.Add(collision);
                _gameCollisions.Add(collision);
            }
            else
            {
                // Just don't record the collision. No penalty incured for directly colliding with opponent balls or black ball since it would be annoying story wise.
            }
        }
        foreach (Tuple<int, int> pocketing in requestEvent._pocketings)
        {
            // checking for white pocketing
            if (pocketing.Item1 == 0)
            {
                _currentTurnPenalties.Add(PenaltyType.WhitePocketing);
                EventBus.Publish(new EventReplaceWhiteRequest());
            }
            // checking for black pocketing and if there is, if it's a legit one or a foul
            else if (pocketing.Item1 == 8)
            {
                bool penalty = false;
                if (_activePlayer == _player1)
                {
                    foreach (int ball in _ballsInPlay)
                    {
                        // checking if there are still balls that need to be pocketed
                        if (ball > 0 && ball < 8)
                        {
                            penalty = true;
                        }
                    }
                }
                else
                {
                    foreach (int ball in _ballsInPlay)
                    {
                        // checking if there are still balls that need to be pocketed
                        if (ball > 8)
                        {
                            penalty = true;
                        }
                    }
                }
                if (penalty)
                {
                    _currentTurnPenalties.Add(PenaltyType.BlackPocketing);
                    EventBus.Publish(new EventReplaceBlackRequest());
                }
                // if no penalty, it was a legit pocketing, so the game is won
                else
                {
                    _lastPocketings.Add(pocketing);
                    _gameEnded = true;
                    _winner = _activePlayer;
                }
            }
            // checking other pocketings
            else
            {
                // if opponent ball pocketed
                if ((_activePlayer == _player1 && pocketing.Item1 > 8) || (_activePlayer == _player2 && pocketing.Item1 > 0 && pocketing.Item1 < 8))
                {
                    _currentTurnPenalties.Add(PenaltyType.OpponentBallPocketing);
                }
                _lastPocketings.Add(pocketing);
                _gamePocketings.Add(pocketing);
                _ballsInPlay.Remove(pocketing.Item1);
                _ballsPocketed.Add(pocketing.Item1);
            }
        }

        // don't bother applying penalty if the game has ended
        if (!_gameEnded)
        {
            // Only applying the highest penalty. No stacking.
            PenaltyType highestPenalty = PenaltyType.NoPenalty;
            foreach (PenaltyType penalty in _currentTurnPenalties)
            {
                if (penalty > highestPenalty)
                {
                    highestPenalty = penalty;
                }
            }
            // TODO apply penalties (placeholder for now) >> don't forget to force player swapping at the end of the turn when the ability to keep playing will be in the game
            switch (highestPenalty)
            {
                case PenaltyType.NoPenalty:
                    Debug.Log("GameStateManager: No foul detected, play will resume without penalty.");
                    break;
                case PenaltyType.OpponentBallPocketing:
                    Debug.Log("GameStateManager: At least one opponent balls has been pocketed, small penalty will be applied.");
                    break;
                case PenaltyType.WhitePocketing:
                    Debug.Log("GameStateManager: White ball was pocketed, small penalty will be applied.");
                    break;
                case PenaltyType.BlackPocketing:
                    Debug.Log("GameStateManager: Black ball was pocketed, big penalty will be applied.");
                    break;
                default:
                    break;
            }
        }
        

        Debug.Log("GameStateManager: calling NextStep");
        EventBus.Publish(new EventGameloopNextStepRequest());
    }

    /// <summary>
    /// Determines who plays next
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleNextPlayerTurnStartRequest(EventNextPlayerTurnStartRequest requestEvent)
    {
        Debug.Log($"GameStateManager: Determining who's to play next.");
        // TODO:
        // - Check if game has ended or not (and who won if it has)
        // - Determine which player turn it is

        if (_activePlayer == ActivePlayerName.Clotho)
        {
            _turnCount++;
            Debug.Log($"Clotho has played {_turnCount} times.");
        }

        // Placeholder win condition (autowin after n turns) for testing purpose
        if (_turnCount > 1)
        {
            Debug.Log("The game will end now.");
            _gameEnded = true;
            _winner = ActivePlayerName.Clotho;
        }

        if (_gameEnded)
        {
            // TODO "rules" related things to handle the end of the game (if needed)
            
        }
        else
        {
            // Placeholder basic turn swapping for now
            if (_activePlayer == ActivePlayerName.Atropos)
            {
                _activePlayer = ActivePlayerName.Clotho;
            }
            else
            {
                _activePlayer = ActivePlayerName.Atropos;
                EventBus.Publish(new EventAIShotRequest());
            }
        }
        Debug.Log("GameStateManager: calling NextStep");
        EventBus.Publish(new EventGameloopNextStepRequest());
    }

    public bool GameHasEnded()
    {
        return _gameEnded;
    }

    public List<Tuple<int, int, bool>> LastCollisions()
    {
        return _lastCollisions;
    }

    public List<Tuple<int, int>> LastPockettings()
    {
        return _lastPocketings;
    }

    public ActivePlayerName ActivePlayer()
    {
        return _activePlayer;
    }


}
