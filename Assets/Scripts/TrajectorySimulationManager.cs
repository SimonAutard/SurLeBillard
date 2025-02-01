using System;
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
    // Design pattern du singleton
    private static TrajectorySimulationManager _instance; // instance statique du game state manager
    private static bool isInitialized = false;

    //Gestion de scene
    private static UnityEngine.SceneManagement.Scene simulationScene;
    private static UnityEngine.SceneManagement.Scene physicsScene;
    private static PhysicsScene simulationPhysicsScene;
    private float simulationHeight = 0;

    //Gestion du mimic
    GameObject[] simBandes;
    GameObject[] simPoches ;
    GameObject[] simBalls;

    //Paramètres de simulation
    int maxSteps = 100;
    float timeStep;

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

    private void CreateSimulationScene(EventNewGameSetupRequest request)
    {
        timeStep = Time.fixedDeltaTime;
        physicsScene = SceneManager.GetSceneByName("PhysicsScene");
        LoadSceneParameters param = new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.Physics3D);
        simulationScene = SceneManager.LoadScene("SimulationScene", param);
        simulationPhysicsScene = simulationScene.GetPhysicsScene();
        /*
        SceneManager.LoadSceneAsync("SimulationScene", LoadSceneMode.Additive).completed += operation =>
        {
            simulationScene = SceneManager.GetSceneByName("SimulationScene");
            physicsScene = SceneManager.GetSceneByName("PhysicsScene");
            if (!simulationScene.IsValid())
            {
                Debug.LogError("SimulationScene is not valid.");
                return;
            }

            simulationPhysicsScene = simulationScene.GetPhysicsScene();
        };*/
    }

    /// <summary>
    /// Vide notre simulationScene de tous ses GO
    /// </summary>
    public void ClearSimulationScene()
    {
        //Recuperer tous les objets de la scene de simu
        GameObject[] allobjects = simulationScene.GetRootGameObjects();
        //Choisir la scene de modification
        SceneManager.SetActiveScene(simulationScene);
        // Détruire tous les objets dans la scène
        foreach (GameObject obj in allobjects)
        {
            
            obj.SetActive(false); // oblige de rendre inactif car l'objet ne sera pas detruit avant la fin de la frame, et dautres scripts risqquent dinteragir avec ces objets en attendant
            GameObject.Destroy(obj); // destruction
        }
        //on rend la scene au physicsmanager
        SceneManager.SetActiveScene(physicsScene);

    }

    /// <summary>
    /// Recopie tous les objets pertinents pour la physique du jeu de la vraie scene a la scene de simulation
    /// </summary>
    /// <param name="request"></param>
    public void MimicScene(EventNextTurnUIDisplayRequest request)
    {
        ClearSimulationScene();

        GameObject[] bandes = GameObject.FindGameObjectsWithTag("Bandes");
        GameObject[] poches = GameObject.FindGameObjectsWithTag("Poche");
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Bille");
        List<GameObject[]> allEnvironment = new List<GameObject[]> { bandes, poches, balls };

        simBandes = new GameObject[bandes.Length];
        simPoches = new GameObject[poches.Length];
        simBalls = new GameObject[balls.Length];

        SceneManager.SetActiveScene(simulationScene);
        foreach (GameObject[] environementCategory in allEnvironment)
        {
            foreach (GameObject environmentAtom in environementCategory)
            {
                //Dupliquer lobjet
                GameObject simGO = Instantiate(environmentAtom);

                //retirer le mesh pour le sperf
                simGO.GetComponent<MeshRenderer>().enabled = false;

                //deplacer lobjet loin de la camera
                Vector3 newPosition = simGO.transform.position;
                newPosition += Vector3.up * simulationHeight;
                simGO.transform.position = newPosition;

                //Ajouter l'object aux vecteurs de GO de la simu
                if (simGO.tag == "Bandes") { simBandes[Array.IndexOf(bandes, environmentAtom)] = simGO; }
                if (simGO.tag == "Poche") { simPoches[Array.IndexOf(poches, environmentAtom)] = simGO; }
                if (simGO.tag == "Bille")
                {
                    simGO.GetComponent<BallRoll>().TurnToSimulation(); // Desactiver la bille pour tous les evenements
                    simBalls[Array.IndexOf(balls, environmentAtom)] = simGO;
                }
            }

        }
        SceneManager.SetActiveScene(physicsScene);
    }

    /// <summary>
    /// Simule les trajectories en renvoyant un struct simTrajectory
    /// </summary>
    /// <param name="force"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public SimTrajectory SimulateTrajectory(float force, Vector3 direction)
    {
        SimTrajectory simTrajectory = new SimTrajectory();
        GameObject whiteBall = simBalls.SingleOrDefault(item => item.GetComponent<BallRoll>()._ballId == 0);
        BallRoll whiteBallRoll  = whiteBall.GetComponent<BallRoll>();
        Vector3 origin = whiteBall.transform.position;
        
        bool hitGO = false;
        for (int i = 0; i < maxSteps; i++)
        {
            Vector3 initialDirection = whiteBallRoll.direction;
            simulationPhysicsScene.Simulate(timeStep);
            Vector3 finalDirection = whiteBallRoll.direction;

            if (initialDirection != finalDirection)
                break;
        }

        simTrajectory.whiteBallTraj.Add(origin);
        simTrajectory.whiteBallTraj.Add(whiteBall.transform.position);

        return simTrajectory;
    }

}
