using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct SimTrajectory
{
    public List<Vector3> whiteBallTraj;
    public List<Vector3> secondBallTraj;
}

public class TrajectorySimulationManager : MonoBehaviour
{


    //Gestion de scene
    private UnityEngine.SceneManagement.Scene _simulationScene;
    private UnityEngine.SceneManagement.Scene _realScene;
    public PhysicsScene _realPhysicsScene { get; private set; }
    private PhysicsScene _simulationPhysicsScene;
    private Vector3 offsetVector = Vector3.zero;

    //Gestion du mimic
    List<GameObject> simBandes;
    List<GameObject> simPoches;
    List<BallRoll> simBallRoll;
    GameObject[] allSimBands;
    GameObject realWhiteBall;

    //Paramètres de simulation
    [SerializeField] int maxSteps = 20;
    [SerializeField] CueScript cue;

    //Affichage
    [SerializeField] LineRenderer whiteBallLineRenderer;
    [SerializeField] LineRenderer secondBallLineRenderer;
    private BallRoll secondBallTracked;


    // Design pattern du singleton
    private static TrajectorySimulationManager _instance; // instance statique du game state manager
    public static TrajectorySimulationManager Instance
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
    private void OnEnable()
    {

        // subscribe to all events that this component needs to listen to at all time
        EventBus.Subscribe<EventNewGameSetupRequest>(CreateSimulationScene);
        EventBus.Subscribe<EventNextTurnUIDisplayRequest>(MimicScene);

    }

    private void OnDisable()
    {
        // Unsubscribe from all events before getting destroyed to avoid memory leaks
        EventBus.Unsubscribe<EventNewGameSetupRequest>(CreateSimulationScene);
        EventBus.Unsubscribe<EventNextTurnUIDisplayRequest>(MimicScene);
    }

    private void OnDestroy()
    {
        // Unsubscribe from all events before getting destroyed to avoid memory leaks
        EventBus.Unsubscribe<EventNewGameSetupRequest>(CreateSimulationScene);
        EventBus.Unsubscribe<EventNextTurnUIDisplayRequest>(MimicScene);
    }

    private void Update()
    {
        //______________DEBUG PARTIE A SUPPRIMER_____________
        if (cue.isActiveAndEnabled)
        {
            float radius = cue.radius;
            cue.CalculateForce(radius);
            SimTrajectory simTrajectory = SimulateTrajectory(UISingleton.Instance.force, cue.GetOrbVector());

        }
        //----------------------------------------------------

    }

    private void CreateSimulationScene(EventNewGameSetupRequest request)
    {
        _realScene = SceneManager.GetSceneByName("PhysicsScene");
        _realPhysicsScene = _realScene.GetPhysicsScene();

        _simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _simulationPhysicsScene = _simulationScene.GetPhysicsScene();

        if (!_simulationPhysicsScene.IsValid())
        {
            Debug.LogError("SimulationPhysicsScene is not valid .");
        }
    }

    /// <summary>
    /// Vide notre simulationScene de tous ses GO
    /// </summary>
    public void ClearSimulationScene()
    {
        //Recuperer tous les objets de la scene de simu
        GameObject[] allobjects = _simulationScene.GetRootGameObjects();
        //Choisir la scene de modification
        SceneManager.SetActiveScene(_simulationScene);

        // Détruire tous les objets dans la scène
        foreach (GameObject obj in allobjects)
        {

            obj.SetActive(false); // oblige de rendre inactif car l'objet ne sera pas detruit avant la fin de la frame, et dautres scripts risqquent dinteragir avec ces objets en attendant
            GameObject.Destroy(obj); // destruction
        }
        //on rend la scene au physicsmanager
        SceneManager.SetActiveScene(_realScene);
    }

    /// <summary>
    /// Recopie tous les objets pertinents pour la physique du jeu de la vraie scene a la scene de simulation
    /// </summary>
    /// <param name="request"></param>
    public void MimicScene(EventNextTurnUIDisplayRequest request)
    {
        //Nettoyer la scene
        ClearSimulationScene();

        //Recuperation des references dans la scene reelle
        GameObject[] bandes = GameObject.FindGameObjectsWithTag("Bandes");
        GameObject[] poches = GameObject.FindGameObjectsWithTag("Poche");
        List<GameObject[]> allEnvironment = new List<GameObject[]> { bandes, poches };

        //Reinitialisation des Listes de simulation
        simBandes = new List<GameObject>();
        simPoches = new List<GameObject>();

        //Duplication de toutes les references
        foreach (GameObject[] environementCategory in allEnvironment)
        {
            //Un atome = un objet a dupliquer
            foreach (GameObject environmentAtom in environementCategory)
            {
                //Dupliquer lobjet
                GameObject simGO = Instantiate(environmentAtom, environmentAtom.transform.position, environmentAtom.transform.rotation);
                SceneManager.MoveGameObjectToScene(simGO, _simulationScene);

                //retirer le mesh pour les perf
                simGO.GetComponent<Renderer>().enabled = false;

                //deplacer lobjet loin de la camera
                Vector3 newPosition = simGO.transform.position;
                newPosition += offsetVector;
                simGO.transform.position = newPosition;

                //Ajouter l'object aux vecteurs de GO de la simu
                if (simGO.tag == "Bandes")
                {
                    simBandes.Add(simGO);
                }
                if (simGO.tag == "Poche") { simPoches.Add(simGO); }
            }

        }
    }

    private void MimicBalls()
    {
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Bille");
        simBallRoll = new List<BallRoll>();

        //Un atome = un objet a dupliquer
        foreach (GameObject obj in balls)
        {
            //Dupliquer lobjet
            GameObject simGO = Instantiate(obj, obj.transform.position, obj.transform.rotation);
            SceneManager.MoveGameObjectToScene(simGO, _simulationScene);

            //retirer le mesh pour les perf
            simGO.GetComponent<Renderer>().enabled = false;

            //deplacer lobjet loin de la camera
            Vector3 newPosition = simGO.transform.position;
            newPosition += offsetVector;
            simGO.transform.position = newPosition;

            BallRoll ballroll = simGO.GetComponent<BallRoll>();
            ballroll.InitializeBallRollParameters();
            ballroll.TurnToSimulation(); // Desactiver la bille pour tous les evenements
            simBallRoll.Add(ballroll);
            Debug.Log(ballroll.ballRadius);

        }

    }

    private void ClearSimulationBalls()
    {
        foreach (BallRoll ballRoll in simBallRoll)
        {
            ballRoll.gameObject.SetActive(false);
            Destroy(ballRoll.gameObject);
        }
        simBallRoll = new List<BallRoll>();
    }


    /// <summary>
    /// Simule les trajectories en renvoyant un struct simTrajectory
    /// </summary>
    /// <param name="force"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public SimTrajectory SimulateTrajectory(float force, Vector3 direction)
    {
        //Creation du struct du resultat final
        SimTrajectory simTrajectory = new SimTrajectory();

        //Duplication des billes
        SceneManager.SetActiveScene(_simulationScene);
        MimicBalls();

        //recuperation d'une nouvelle bille blanche
        WhiteBallMove whiteBallMove = (WhiteBallMove)simBallRoll.FirstOrDefault(item => item._ballId == 0);

        //Lancement de la bille selon les parametres de tir actuels
        whiteBallMove.PushThisBall(direction, force);

        //Obliteration de la seconde bille suivie
        ClearSecondLineRenderer();

        //Initialisation du line renderer
        whiteBallLineRenderer.positionCount = maxSteps;
        whiteBallLineRenderer.SetPosition(0, whiteBallMove.transform.position);
        secondBallLineRenderer.positionCount = maxSteps;

        Debug.Log("start simulation");

        //Simulation pour n = maxsteps
        for (int i = 1; i < maxSteps; i++)
        {
            //Simulation
            Debug.Log("start simulation step " + i);
            SimulateOwnPhysicsStep(PhysicsManager.Instance.generalTimeStep);

            //Mise a jour du linerenderer a apritr de la nouvelle position de la bille blanche
            whiteBallLineRenderer.SetPosition(i, whiteBallMove.transform.position);

            //MaJ Linerenderer pour seconde bille
            if (secondBallTracked != null)
            {
                secondBallLineRenderer.SetPosition(i, secondBallTracked.transform.position);
                //Verification que le line avant collision est bien positionne
                if (i > 0 && secondBallLineRenderer.GetPosition(i - 1) == Vector3.zero)
                {
                    //si on decouvre que cest la premiere frame ou on a collision, alors on MaJ toutes les positions anterieures du linerenderer
                    for (int j = 0; j < i; j++)
                    {
                        secondBallLineRenderer.SetPosition(j, secondBallTracked.transform.position);
                    }
                }
            }

        }

        //processus de destruction des billes de simulation
        ClearSimulationBalls();
        SceneManager.SetActiveScene(_realScene);

        return simTrajectory;
    }

    /// <summary>
    /// Simulation dune etape delta time de notre propre physique
    /// </summary>
    /// <param name="timestep"></param>
    void SimulateOwnPhysicsStep(float timestep)
    {
        //Imitation Update
        foreach (BallRoll obj in simBallRoll)
        {
            //deplacement
            obj.RollTheBall(timestep);
            //detection dempochement
            obj.CheckPocketing();

        }
        //Imitation lateUpdate
        foreach (BallRoll obj in simBallRoll)
        {
            //mise a jour de la possibiltie de collision
            Collider collider = obj.HandleCollisions(_simulationPhysicsScene);
            //detection de la premiere bille touchee par la bille blanche
            if (obj._ballId == 0 && collider != null) { HandleWhiteFirstCollision(collider); }

        }

    }

    void HandleWhiteFirstCollision(Collider collider)
    {
        if (collider.gameObject.TryGetComponent<BallRoll>(out BallRoll ballRoll) && secondBallTracked == null)
        {
            secondBallTracked = ballRoll;
        }
    }

    void ClearSecondLineRenderer()
    {

        secondBallTracked = null;
        secondBallLineRenderer.positionCount = 0;
    }

    public List<Collider> FindCollidersSimulatedScene(BallRoll centralBall)
    {
        return PhysicsManager.Instance.FindCurrentCollidingItems(simBallRoll, simBandes.ToArray(), centralBall);
    }
}
