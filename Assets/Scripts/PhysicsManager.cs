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
        BallRoll[] ballRolls = FindObjectsByType<BallRoll>(FindObjectsSortMode.None);
        RemainingBalls = ballRolls.ToList();
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

                // collisions info : fastest ball id, slowest ball id, positive or not
                List<Tuple<int, int, bool>> collisions = new List<Tuple<int, int, bool>>();
                // pocketings info : ball id, pocket id
                List<Tuple<int, int>> pocketings = new List<Tuple<int, int>>();

                Debug.Log("Physics Manager: Requesting rules application.");
                EventBus.Publish(new EventApplyRulesRequest(collisions, pocketings));
            }
        }
        
    }

    private void OnEnable()
    {
        // subscribe to all events that this component needs to listen to at all time
        EventBus.Subscribe<EventInitialBallsSetupRequest>(HandleBallsInitialSetupRequest);
        EventBus.Subscribe<EventApplyForceToWhiteRequest>(HandleForceApplicationToWhiteRequest);
        // abonnement à l'evenement empochement de billes
        BallRoll.BallPocketed += UnregisterBall;
    }

    private void OnDisable()
    {
        // Unsubscribe from all events before getting destroyed to avoid memory leaks
        EventBus.Unsubscribe<EventInitialBallsSetupRequest>(HandleBallsInitialSetupRequest);
        EventBus.Unsubscribe<EventApplyForceToWhiteRequest>(HandleForceApplicationToWhiteRequest);
        // abonnement à l'evenement empochement de billes
        BallRoll.BallPocketed += UnregisterBall;
    }

    private void OnDestroy()
    {
        // Unsubscribe from all events before getting destroyed to avoid memory leaks
        EventBus.Unsubscribe<EventInitialBallsSetupRequest>(HandleBallsInitialSetupRequest);
        EventBus.Unsubscribe<EventApplyForceToWhiteRequest>(HandleForceApplicationToWhiteRequest);
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

}
