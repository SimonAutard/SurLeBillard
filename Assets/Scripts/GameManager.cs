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
    private bool _waitForNextStep = false;
    private int _currentLoopStep = 0;



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
        _gameStartSteps.Add(new UIFeedback(_stepGenId++, _stepGenId));
        _stepGenId = 0;
        _gameLoopSteps.Add(new NextPlayerTurn(_stepGenId++, _stepGenId));
        _gameLoopSteps.Add(new ProphecyGeneration(_stepGenId++, _stepGenId));
        _gameLoopSteps.Add(new UIFeedback(_stepGenId++, _stepGenId));
        _stepGenId = 0;
        _gameEndSteps.Add(new EndGame(_stepGenId++, _stepGenId));
    }

    /// <summary>
    /// The main loop handling the various steps of a game.
    /// </summary>
    /// <returns></returns>
    public IEnumerator MainLoop()
    {
        Debug.Log("GameManager: Starting initialisation steps.");
        _currentLoopStep = 0;
        while (_currentLoopStep < _gameStartSteps.Count)
        {
            _waitForNextStep = _gameStartSteps[_currentLoopStep].Execute();
            while (_waitForNextStep)
            {
                yield return 0;
            }
            _currentLoopStep = _gameStartSteps[_currentLoopStep].NextStep();
        }
        Debug.Log("GameManager: Initialisation steps have ended.");
        Debug.Log("GameManager: Starting main loop steps.");
        _currentLoopStep = 0;
        while (_currentLoopStep < _gameLoopSteps.Count)
        {
            _waitForNextStep =_gameLoopSteps[_currentLoopStep].Execute();
            while (_waitForNextStep)
            {
                yield return 0;
            }
            _currentLoopStep = _gameLoopSteps[_currentLoopStep].NextStep();
        }
        Debug.Log("GameManager: Main loop steps have ended.");
        Debug.Log("GameManager: Starting end loop steps.");
        _currentLoopStep = 0;


    }

    private void HandleGameloopNextStepRequest(EventGameloopNextStepRequest requestEvent)
    {
        _waitForNextStep = false;
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
        if (_gameplayLoop != null)
        {
            StopCoroutine(_gameplayLoop);
        }
        _gameplayLoop = StartCoroutine(MainLoop());
    }
}
