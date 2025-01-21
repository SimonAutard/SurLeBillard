using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PhysicsManager : MonoBehaviour
{
    // Design pattern du singleton
    private static PhysicsManager _instance; // instance statique du game state manager

    //Gestion des phases
    private bool dispersionPhase = false;
    public float minSpeedForBalls {  get; private set; }

    // collisions that have happened during the turn (all of them, even white)
    private List<Tuple<int, int, bool>> _turnCollisions = new List<Tuple<int, int, bool>>();
    // pocketings that have happened during the turn (all of them, even white and black)
    private List<Tuple<int, int>> _turnPocketings = new List<Tuple<int, int>>();

    //Array des billes restantes
    List<BallRoll> RemainingBalls = new List<BallRoll>();

    public static PhysicsManager Instance
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
    }

    private void Start()
    {
        minSpeedForBalls = 0.1f;
    }

    private void Update()
    {
        // Vérification qu'on est en phase de dispersion
        if (dispersionPhase) {
            // Ceci revient à vérifier que toutes les billes sont arretees
            bool BallsAllMotionless = true;
            foreach (BallRoll ball in RemainingBalls) 
            {
                // ON controle la vitesse de toutes les billes existantes
                if(ball.speed > minSpeedForBalls)
                {
                    //Des qu'on trouve une bille qui n'est pas arretee, on peut arreter la vérification ici
                    BallsAllMotionless = false;
                    break;
                }
            }
            // Si toutes les billes sont arretees, on leve l'evenement signalant la fin de la phase de dispersion
            if (BallsAllMotionless) {
                dispersionPhase = false;

                Debug.Log("Physics Manager: Requesting next step.");
                EventBus.Publish(new EventGameloopNextStepRequest());
            }
        }
        
    }

    private void OnEnable()
    {
        // subscribe to all events that this component needs to listen to at all time
        EventBus.Subscribe<EventInitialBallsSetupRequest>(HandleBallsInitialSetupRequest);
        EventBus.Subscribe<EventApplyForceToWhiteRequest>(HandleForceApplicationToWhiteRequest);
        EventBus.Subscribe<EventReplaceWhiteRequest>(HandleReplaceWhiteRequest);
        EventBus.Subscribe<EventReplaceBlackRequest>(HandleReplaceBlackRequest);
        EventBus.Subscribe<EventNewGameSetupRequest>(HandleNewGameSetupRequest);
        
        // abonnement à l'evenement empochement de billes
        BallRoll.BallPocketed += UnregisterBall;
    }

    private void OnDisable()
    {
        // Unsubscribe from all events before getting destroyed to avoid memory leaks
        EventBus.Unsubscribe<EventInitialBallsSetupRequest>(HandleBallsInitialSetupRequest);
        EventBus.Unsubscribe<EventApplyForceToWhiteRequest>(HandleForceApplicationToWhiteRequest);
        EventBus.Unsubscribe<EventReplaceWhiteRequest>(HandleReplaceWhiteRequest);
        EventBus.Unsubscribe<EventReplaceBlackRequest>(HandleReplaceBlackRequest);
        EventBus.Unsubscribe<EventNewGameSetupRequest>(HandleNewGameSetupRequest);
        // abonnement à l'evenement empochement de billes
        BallRoll.BallPocketed += UnregisterBall;
    }

    private void OnDestroy()
    {
        // Unsubscribe from all events before getting destroyed to avoid memory leaks
        EventBus.Unsubscribe<EventInitialBallsSetupRequest>(HandleBallsInitialSetupRequest);
        EventBus.Unsubscribe<EventApplyForceToWhiteRequest>(HandleForceApplicationToWhiteRequest);
        EventBus.Unsubscribe<EventReplaceWhiteRequest>(HandleReplaceWhiteRequest);
        EventBus.Unsubscribe<EventReplaceBlackRequest>(HandleReplaceBlackRequest);
        EventBus.Unsubscribe<EventNewGameSetupRequest>(HandleNewGameSetupRequest);
    }

    /// <summary>
    /// Sets up the balls in their initial state
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleBallsInitialSetupRequest(EventInitialBallsSetupRequest requestEvent)
    {
        // TODO setup the scene
        Debug.Log("Physics Manager: Placing the balls in their initial position.");
        Debug.Log("Physics Manager: Requesting next step.");
        EventBus.Publish(new EventGameloopNextStepRequest());
    }

    /// <summary>
    /// Applies force on the white ball based on angle and force sent through the event
    /// </summary>
    /// <param name="requestEvent">Contains _angle and _force</param>
    private void HandleForceApplicationToWhiteRequest(EventApplyForceToWhiteRequest requestEvent)
    {
        dispersionPhase = true;
        Debug.Log("Physics Manager: Applying force to white ball.");
        // TODO:
        // - Apply force
        // - Resolve physics
        // - Record collisions (balls and positive/negative status)
        // - Record pocketings
        // - Record new positions of balls
    }

    private void UnregisterBall(BallRoll ball)
    {
        RemainingBalls.Remove(ball);
    }

    /// <summary>
    /// Replaces the white ball at a random position
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleReplaceWhiteRequest(EventReplaceWhiteRequest requestEvent)
    {
        // TODO (don't forget to check if the ball is already on the field before doing anything, just in case the event is published at the wrong time for some reason)
    }

    /// <summary>
    /// Replaces the black ball at a random position
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleReplaceBlackRequest(EventReplaceBlackRequest requestEvent)
    {
        // TODO (don't forget to check if the ball is already on the field before doing anything, just in case the event is published at the wrong time for some reason)
    }

    /// <summary>
    /// Returns all collisions that have happened during the turn, including the ones with white ball
    /// </summary>
    /// <returns></returns>
    public List<Tuple<int, int, bool>> TurnCollisions()
    {
        return _turnCollisions;
    }

    /// <summary>
    /// Returns all pocketings that have happened during the turn, including white and black balls
    /// </summary>
    /// <returns></returns>
    public List<Tuple<int, int>> TurnPocketings()
    {
        return _turnPocketings;
    }

    /// <summary>
    /// Things to setup at the start of a new game
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleNewGameSetupRequest(EventNewGameSetupRequest requestEvent)
    {
        BallRoll[] ballRolls = FindObjectsByType<BallRoll>(FindObjectsSortMode.None);
        RemainingBalls = ballRolls.ToList();
    }

}
