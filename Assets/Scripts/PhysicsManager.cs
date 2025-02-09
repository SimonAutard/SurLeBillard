using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    // Design pattern du singleton
    private static PhysicsManager _instance; // instance statique du game state manager

    //Gestion des phases
    private bool dispersionPhase = false;
    public float minSpeedForBalls { get; private set; }

    //Paramétrage de la physique
    public float generalTimeStep {  get; private set; }
    [SerializeField] private float timeStepRatio;

    //Gestion du gameplay 
    //Array des billes restantes
    List<BallRoll> RemainingBalls = new List<BallRoll>();
    // collisions that have happened during the turn (all of them, even white)
    private List<Tuple<int, int, bool>> _turnCollisions = new List<Tuple<int, int, bool>>();
    // pocketings that have happened during the turn (all of them, even white and black)
    private List<Tuple<int, int>> _turnPocketings = new List<Tuple<int, int>>();

    //Paramétrage des billes
    [SerializeField] GameObject whiteBallPrefab;
    [SerializeField] GameObject blackBallPrefab;

    //Gestion du terrain
    System.Random random = new System.Random(); // instance pour les evenemnets aleatoires
    [SerializeField] Vector3 leftmostWhiteLinePoint;
    [SerializeField] Vector3 rightmostWhiteLinePoint;
    [SerializeField] Vector3 tableCenter;
    GameObject[] allBands;
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
        allBands = GameObject.FindGameObjectsWithTag("Bandes");
        generalTimeStep = Time.fixedDeltaTime * timeStepRatio ;
    }

    private void Update()
    {
        // Vérification qu'on est en phase de dispersion
        if (dispersionPhase)
        {
            // Ceci revient à vérifier que toutes les billes sont arretees
            bool BallsAllMotionless = true;
            foreach (BallRoll ball in RemainingBalls)
            {
                // ON controle la vitesse de toutes les billes existantes
                if (ball.speed > minSpeedForBalls)
                {
                    //Des qu'on trouve une bille qui n'est pas arretee, on peut arreter la vérification ici
                    BallsAllMotionless = false;
                    break;
                }
            }
            // Si toutes les billes sont arretees, on leve l'evenement signalant la fin de la phase de dispersion
            if (BallsAllMotionless)
            {
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
        EventBus.Subscribe<EventPocketingSignal>(UnregisterBall);

    }

    private void OnDisable()
    {
        // Unsubscribe from all events before getting destroyed to avoid memory leaks
        EventBus.Unsubscribe<EventInitialBallsSetupRequest>(HandleBallsInitialSetupRequest);
        EventBus.Unsubscribe<EventApplyForceToWhiteRequest>(HandleForceApplicationToWhiteRequest);
        EventBus.Unsubscribe<EventReplaceWhiteRequest>(HandleReplaceWhiteRequest);
        EventBus.Unsubscribe<EventReplaceBlackRequest>(HandleReplaceBlackRequest);
        EventBus.Unsubscribe<EventNewGameSetupRequest>(HandleNewGameSetupRequest);
        EventBus.Unsubscribe<EventPocketingSignal>(UnregisterBall);
    }

    private void OnDestroy()
    {
        // Unsubscribe from all events before getting destroyed to avoid memory leaks
        EventBus.Unsubscribe<EventInitialBallsSetupRequest>(HandleBallsInitialSetupRequest);
        EventBus.Unsubscribe<EventApplyForceToWhiteRequest>(HandleForceApplicationToWhiteRequest);
        EventBus.Unsubscribe<EventReplaceWhiteRequest>(HandleReplaceWhiteRequest);
        EventBus.Unsubscribe<EventReplaceBlackRequest>(HandleReplaceBlackRequest);
        EventBus.Unsubscribe<EventNewGameSetupRequest>(HandleNewGameSetupRequest);
        EventBus.Unsubscribe<EventPocketingSignal>(UnregisterBall);
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
        //Une nouvelle phase de cou pcommence, on supprime les donnees du tour precedent
        _turnCollisions.Clear();
        _turnPocketings.Clear();
        //On declare entrer en phase de dispersion
        dispersionPhase = true;
        Debug.Log("Physics Manager: Applying force to white ball.");

    }

    private void UnregisterBall(EventPocketingSignal pocketingEvent)
    {
        //Mise a jour de la liste des billes actives
        RemainingBalls.Remove(pocketingEvent._ball);
        //Mise a jour des billes empochees ce tour
        _turnPocketings.Add(new Tuple<int, int>(pocketingEvent._ball._ballId, pocketingEvent._pocketID));
    }

    /// <summary>
    /// Replaces the white ball at a random position
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleReplaceWhiteRequest(EventReplaceWhiteRequest requestEvent)
    {
        // check if whiteball is already in play before doing anything, just in case the event is published at the wrong time for some reason
        foreach (BallRoll ball in RemainingBalls)
        {
            if (ball._ballId == GameStateManager.Instance.whiteBallID) { return; }
        }

        bool whiteBallReplaced = false;
        Vector3 newPosition = Vector3.zero; // position de placement de la nouvelle bille
        int antiInifinityLoop = 0;
        while (!whiteBallReplaced && antiInifinityLoop < 10) // La boucle tourne tant qu'on n'a pas trouvé un endroit convenable pour la bille blanche
        {
            antiInifinityLoop++;

            Debug.Log("Could not find suitable place for white ball, retrying...");
            //On prend un point aléatoire sur la ligne de replacement de la bille blanche
            newPosition = leftmostWhiteLinePoint + UnityEngine.Random.Range(0f, 1f) * (rightmostWhiteLinePoint - leftmostWhiteLinePoint);
            //On capsulecast vers le sol depuis cette position pour vérifier qu'on ne touche pas une autre bille ou bande
            whiteBallReplaced = !Physics.SphereCast(newPosition, whiteBallPrefab.GetComponent<SphereCollider>().radius, Vector3.down, out RaycastHit hitInfo);
            //Si on a touché, on reprend la boucle

        }
        //instanciation de la nouvelle bille blanche
        GameObject newWhiteBall = Instantiate(whiteBallPrefab, newPosition, Quaternion.identity);
        //MaJ du conteneur des billes
        RemainingBalls.Add(newWhiteBall.GetComponent<BallRoll>());
        Debug.Log("white ball was recreated");
        EventBus.Publish(new EventBallWasCreated(newWhiteBall));
    }

    /// <summary>
    /// Replaces the black ball at a random position
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleReplaceBlackRequest(EventReplaceBlackRequest requestEvent)
    {
        // TODO (don't forget to check if the ball is already on the field before doing anything, just in case the event is published at the wrong time for some reason)
        foreach (BallRoll ball in RemainingBalls)
        {
            if (ball._ballId == GameStateManager.Instance.blackBallID) { return; }
        }

        bool blackBallReplaced = false;
        Vector3 newPosition = Vector3.zero; // position de placement de la nouvelle bille
        int antiInifinityLoop = 0;
        while (!blackBallReplaced && antiInifinityLoop < 10) // La boucle tourne tant qu'on n'a pas trouvé un endroit convenable pour la bille blanche
        {
            antiInifinityLoop++;

            //On prend un point aléatoire dans la zone de replacement de la bille noire
            newPosition = new Vector3(UnityEngine.Random.Range(0, 1f), 0, UnityEngine.Random.Range(0, 1f)) + tableCenter;
            //On capsulecast vers le sol depuis cette position pour vérifier qu'on ne touche pas une autre bille ou bande
            blackBallReplaced = !Physics.SphereCast(newPosition, blackBallPrefab.GetComponent<SphereCollider>().radius, Vector3.down, out RaycastHit hitInfo);
            //Si on a touché, on reprend la boucle
        }
        //instanciation de la nouvelle bille noire
        GameObject newBlackBall = Instantiate(blackBallPrefab, newPosition, Quaternion.identity);
        //MaJ du conteneur des billes
        RemainingBalls.Add(newBlackBall.GetComponent<BallRoll>());
        //EventBus.Publish(new EventBallWasCreated(newBlackBall));
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
        allBands = GameObject.FindGameObjectsWithTag("Bandes");
    }


    /// <summary>
    /// renvoie les collider superposés a une bille dans la scene reelle
    /// </summary>
    /// <param name="centralBall"></param>
    /// <returns></returns>
    public List<Collider> FindCollidersRealScene(BallRoll centralBall)
    {
        return FindCurrentCollidingItems(RemainingBalls, allBands, centralBall);
    }

    /// <summary>
    /// renvoie les colliders superposés à une bille dans une scene donnee
    /// </summary>
    /// <param name="allBalls"></param>
    /// <param name="allBands"></param>
    /// <param name="centralBall"></param>
    /// <returns></returns>
    public List<Collider> FindCurrentCollidingItems(List<BallRoll> allBalls, GameObject[] allBands, BallRoll centralBall)
    {
        List<Collider> result = new List<Collider>(); // Liste des collider trouvés
        float radius1 = centralBall.ballRadius; //rayon de la bille centrale
        float radius2; //rayon de la bille potentiellement superposee
        foreach (BallRoll ballRoll in allBalls)
        {
            radius2 = ballRoll.ballRadius;
            //Mesure de la distance entre les deux billes
            if ((ballRoll.transform.position - centralBall.transform.position).magnitude <= radius1 + radius2)
            {
                //Si les deux billes sont plus proches que la somme de leurs rayons, c'est quelles sont superposees
                result.Add(ballRoll.gameObject.GetComponent<Collider>()); //On ajoute le colldier de la bille superposee aux resultats
            }
        }
        //On verifie egalement si on a trouve une bande superposee
        Collider bandCollided = FindCollidingBand(allBands, centralBall);
        //Si on a trouve une bande, on lajoute a la liste
        if (bandCollided != null) { result.Add(bandCollided); }
        return result;

    }
    /// <summary>
    /// Renvoie une bande aleatoire parmi toutes les bandes superposees a la bille donnee en argument
    /// </summary>
    /// <param name="allBands"></param>
    /// <param name="centralBall"></param>
    /// <returns></returns>
    private Collider FindCollidingBand(GameObject[] allBands, BallRoll centralBall)
    {
        Collider result = null; //resultats

        float ballradius = centralBall.ballRadius;
        //Verification de collision pour chque bande. Des quune bande est detectee comme superposee, on sort de la boucle, car on ne renvoie quune seule bande quoi quil arrive
        foreach (GameObject band in allBands)
        {
            //Comme toutes les bandes ne sont pas orientees de la meme facon, il faut dabord morpher le vecteur position de la bille dans le systeme de coordonnees de la bande
            Vector3 P1local = centralBall.transform.position - band.transform.position; // position de la bille dans le repere de la bande post rotation
            Vector3 P2local = Quaternion.Inverse(band.transform.rotation) * P1local; // position de la bille dans le repere de la bande pre rotation
            Vector3 P = band.transform.position + P2local; //position de la bille dans le repere monde pre rotation
            P.y = band.transform.position.y;
            //Distance vectorielle entre la bille et la bande
            Vector3 dis = P - band.transform.position;
            //Distances selon chaque axe
            float xDis = Math.Abs(dis.x);
            float zDis = Math.Abs(dis.z);
            //Demi longueur et largeur de la bande
            float xCap = band.transform.localScale.x / 2;
            float zCap = band.transform.localScale.z / 2;
            //Cas 1 : la bille est assez eloignee selon l'un des axes pour etre certainement hors de la zone de collision
            if (xDis > xCap + ballradius || zDis > zCap + ballradius)
            {
                continue;
            }
            //Cas 2 : la bille est assez proche selon l'un des axes pour etre certainement dans la zone de collision
            else if (xDis < xCap || zDis < zCap)
            {
                result = band.GetComponent<Collider>(); //stockage du collider de la bande
                break; // sortie de boucle immediate car on ne garde quune seule bande quoi quil arive
            }
            //Cas 3 : la bille est dans un des carres de cote ballRadius a lun des coins de la bande
            //Il faut calculer la distance de la bille a ce coin pour savoir si il  ya collision
            else
            {
                
                //Recuperation des coordonnes des 4 coins de la bande
                Vector3[] bandCorners = GetBandCorners(band, xCap, zCap);
                //Calcul de la distance pour chaque coin
                foreach (Vector3 corner in bandCorners)
                {
                    //Calcul de la distance bille-coin
                    if ((P - corner).magnitude < ballradius)
                    {
                        result = band.GetComponent<Collider>(); //stockage du collider de la bande
                        break;// sortie de boucle immediate car on ne garde quune seule bande quoi quil arive
                    }
                }
                if (result != null) { break; }// sortie de boucle immediate car on ne garde quune seule bande quoi quil arive
            }

        }
        return result;
    }

    /// <summary>
    /// Calcule les positions monde des 4 angles dune bande
    /// </summary>
    /// <param name="band"></param>
    /// <param name="xCap"></param>
    /// <param name="zCap"></param>
    /// <returns></returns>
    private Vector3[] GetBandCorners(GameObject band, float xCap, float zCap)
    {
        Vector3[] result = new Vector3[4];
        Vector3 B = band.transform.position;
        result[0] = new Vector3(xCap, 0, zCap) + B;
        result[1] = new Vector3(-xCap, 0, zCap) + B;
        result[2] = new Vector3(xCap, 0, -zCap) + B;
        result[3] = new Vector3(-xCap, 0, -zCap) + B;
        return result;
    }
}
