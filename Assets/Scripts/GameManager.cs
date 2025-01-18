using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance; // instance statique du game manager
    private Coroutine _gameplayLoop = null;
    private List<GameLoopStep> _gameStartSteps = new List<GameLoopStep>();
    private List<GameLoopStep> _gameLoopSteps = new List<GameLoopStep>();
    private List<GameLoopStep> _gameEndSteps = new List<GameLoopStep>();
    private int _stepGenId = 0;
    [SerializeField] private bool _waitForNextStep = false;
    private int _currentLoopStep = 0;
    private bool _gameToStart = false;
    public bool _initialisationPhase { get; private set; }
    public bool _mainPhase { get; private set; }
    public bool _endPhase { get; private set; }


    public static GameManager Instance
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
        StepInit();        
    }

    private void OnEnable()
    {
        // subscribe to all events that this component needs to listen to at all time
        EventBus.Subscribe<EventNewGameRequest>(HandleNewGameRequest);
        EventBus.Subscribe<EventGameloopNextStepRequest>(HandleGameloopNextStepRequest);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EventNewGameRequest>(HandleNewGameRequest);
        EventBus.Unsubscribe<EventGameloopNextStepRequest>(HandleGameloopNextStepRequest);
    }

    private void OnDestroy()
    {
        // Unsubscribe from all events before getting destroyed to avoid memory leaks
        EventBus.Unsubscribe<EventNewGameRequest>(HandleNewGameRequest);
        EventBus.Unsubscribe<EventGameloopNextStepRequest>(HandleGameloopNextStepRequest);
    }

    private void Start()
    {

    }

    private void StepInit()
    {
        _gameStartSteps.Add(new NewGameSetup(_stepGenId++, _stepGenId));
        _gameStartSteps.Add(new BallsSetup(_stepGenId++, _stepGenId));
        _gameStartSteps.Add(new InitialBreak(_stepGenId++, _stepGenId));
        _gameStartSteps.Add(new InitialBreakUI(_stepGenId++, _stepGenId));
        _gameStartSteps.Add(new UIFeedback(_stepGenId++, _stepGenId));
        _stepGenId = 0;
        _gameLoopSteps.Add(new NextPlayerTurn(_stepGenId++, _stepGenId));
        _gameLoopSteps.Add(new ProphecyGeneration(_stepGenId++, _stepGenId));
        _gameLoopSteps.Add(new UIFeedback(_stepGenId++, _stepGenId));
        _stepGenId = 0;
        _gameEndSteps.Add(new EndGame(_stepGenId++, _stepGenId));
        _initialisationPhase = false;
        _mainPhase = false;
        _endPhase = false;
    }

    /// <summary>
    /// The main loop handling the various steps of a game.
    /// </summary>
    /// <returns></returns>
    private void Update()
    {
        if (_gameToStart)
        {
            _gameToStart = false;
            _initialisationPhase = true;
            Debug.Log("GameManager: Starting initialisation steps.");
            _currentLoopStep = 0;
        }
        else if (_initialisationPhase)
        {
            if (_waitForNextStep)
            {
                // do nothing, we're waiting for the end of the execution of the step before going next
            }
            else
            {
                if (_currentLoopStep < _gameStartSteps.Count)
                {
                    _gameStartSteps[_currentLoopStep].Execute();
                    _currentLoopStep = _gameStartSteps[_currentLoopStep].NextStep();
                    Debug.Log($"next step Id = {_currentLoopStep}");
                }
                else
                {
                    Debug.Log("GameManager: Initialisation steps have ended.");
                    _initialisationPhase = false;
                    _mainPhase = true;
                    Debug.Log("GameManager: Starting main loop steps.");
                    _currentLoopStep = 0;
                }
            } 
        }
        else if (_mainPhase)
        {
            if (_waitForNextStep)
            {
                // do nothing, we're waiting for the end of the execution of the step before going next
            }
            else
            {
                if (_currentLoopStep < _gameLoopSteps.Count)
                {
                    _gameLoopSteps[_currentLoopStep].Execute();
                    _currentLoopStep = _gameLoopSteps[_currentLoopStep].NextStep();
                    Debug.Log($"next step Id = {_currentLoopStep}");
                }
                else
                {
                    Debug.Log("GameManager: Main loop steps have ended.");
                    _mainPhase = false;
                    _endPhase = true;
                    Debug.Log("GameManager: Starting end loop steps.");
                    _currentLoopStep = 0;
                }
            }
        }
        else if (_endPhase)
        {
            if (_waitForNextStep)
            {
                // do nothing, we're waiting for the end of the execution of the step before going next
            }
            else
            {
                if (_currentLoopStep < _gameEndSteps.Count)
                {
                    _gameEndSteps[_currentLoopStep].Execute();
                    _currentLoopStep = _gameEndSteps[_currentLoopStep].NextStep();
                    Debug.Log($"next step Id = {_currentLoopStep}");
                }
                else
                {
                    Debug.Log("GameManager: End of game steps have ended.");
                    _endPhase = false;
                }
            }
        }
    }

    private void HandleGameloopNextStepRequest(EventGameloopNextStepRequest requestEvent)
    {
        _waitForNextStep = false;
    }

    public void WaitForNextStep(bool wait)
    {
        _waitForNextStep = wait;
    }


    //****************************************
    //*** NarrationManager related methods ***
    //****************************************

    

    //****************************************
    //*** GameStateManager related methods ***
    //****************************************



    //***************************************
    //*** PhysiscsManager related methods ***
    //***************************************



    //*********************************
    //*** UIManager related methods ***
    //*********************************

    /// <summary>
    /// Starts the gameplay loop
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleNewGameRequest(EventNewGameRequest requestEvent)
    {
        Debug.Log("GameManager: Starting a new game.");
        _gameToStart = true;
    }
}
