using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    // Design pattern du singleton
    private static PhysicsManager _instance; // instance statique du game state manager

    //Gestion des phases
    public bool dispersionPhase { get; private set; } = false;
    public float minSpeedForBalls { get; private set; } //Vitesse sous laquelle les billes sarretent completement

    //Paramétrage de la physique générale
    public float generalTimeStep { get; private set; }
    public float TimeStepRatio { get { return timeStepRatio; } private set { timeStepRatio = value; } }
    [SerializeField] float timeStepRatio;
    public float CueMinForce { get { return cueMinForce; } private set { cueMinForce = value; } }
    [SerializeField] float cueMinForce;
    public float CueMaxForce { get { return cueMaxForce; } private set { cueMaxForce = value; } }
    [SerializeField] float cueMaxForce;

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
    private int antiInfinityLoopUpperBound = 20;
    public float DragMultiplicator { get { return dragMultiplicator; } private set { dragMultiplicator = value; } }
    [SerializeField] float dragMultiplicator; // Coef multiplicatif des frottements du tapis sur la bille
    public float DragAdditor { get { return dragAdditor; } private set { dragAdditor = value; } }
    [SerializeField] float dragAdditor;// Coef additif des frottements du tapis sur la bille

    //Gestion du terrain
    System.Random random = new System.Random(); // instance pour les evenemnets aleatoires
    [SerializeField] Vector3 leftmostWhiteLinePoint;
    [SerializeField] Vector3 rightmostWhiteLinePoint;
    [SerializeField] Vector3 tableCenter;
    GameObject[] allBands;
    GameObject[] allPockets;

    public float BandSpeedReductionFactor { get { return bandSpeedReductionFactor; } private set { bandSpeedReductionFactor = value; } }
    [SerializeField] public float bandSpeedReductionFactor;//coef d'attnuation de la vitesse par les bandes

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
        generalTimeStep = Time.fixedDeltaTime * timeStepRatio;
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
        int antiInfinityLoop = 0;
        while (!whiteBallReplaced && antiInfinityLoop < antiInfinityLoopUpperBound) // La boucle tourne tant qu'on n'a pas trouvé un endroit convenable pour la bille blanche
        {
            antiInfinityLoop++;
            Debug.Log("Could not find suitable place for white ball, retrying...");

            //On prend un point aléatoire sur la ligne de replacement de la bille blanche
            newPosition = leftmostWhiteLinePoint + UnityEngine.Random.Range(0f, 1f) * (rightmostWhiteLinePoint - leftmostWhiteLinePoint);
            //On capsulecast vers le sol depuis cette position pour vérifier qu'on ne touche pas une autre bille ou bande
            Collider[] collided = Physics.OverlapSphere(newPosition, whiteBallPrefab.GetComponent<SphereCollider>().radius);
            whiteBallReplaced = collided.Length == 0 ? true : false;

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
        int antiInfinityLoop = 0;
        while (!blackBallReplaced && antiInfinityLoop < antiInfinityLoopUpperBound) // La boucle tourne tant qu'on n'a pas trouvé un endroit convenable pour la bille blanche
        {
            antiInfinityLoop++;

            //On prend un point aléatoire dans la zone de replacement de la bille noire
            newPosition = new Vector3(UnityEngine.Random.Range(0, 1f), 0, UnityEngine.Random.Range(0, 1f)) + tableCenter;
            //On capsulecast vers le sol depuis cette position pour vérifier qu'on ne touche pas une autre bille ou bande
            Collider[] collided = Physics.OverlapSphere(newPosition, blackBallPrefab.GetComponent<SphereCollider>().radius);
            blackBallReplaced = collided.Length == 0 ? true : false;
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
        FindAllBalls();
        allBands = GameObject.FindGameObjectsWithTag("Bandes");
        allPockets = FindAllPockets();

    }

    /// <summary>
    /// Fonction lancée au début d'une nouvelle partie par le GameStateManager ET par le PhysicsManager dans HandleNewGameSetupRequest
    /// </summary>
    /// <returns></returns>
    public List<int> FindAllBalls()
    {
        if (RemainingBalls.Count == 0) {
            BallRoll[] ballRolls = FindObjectsByType<BallRoll>(FindObjectsSortMode.None);
            RemainingBalls = ballRolls.ToList();
        }
        List<int> result = new List<int>();
        foreach (BallRoll ballRoll in RemainingBalls) {
            result.Add(ballRoll._ballId);
        }
        return result;
    }

    private GameObject[] FindAllPockets()
    {

        GameObject[] preResult = GameObject.FindGameObjectsWithTag("Poche");
        GameObject[] result = new GameObject[preResult.Length];
        foreach (GameObject pocket in preResult)
        {
            int index = int.Parse(pocket.name.Substring(pocket.name.Length - 1)) - 1;
            result[index] = pocket;
        }
        return result;

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

    /// <summary>
    /// Renvoie -1 si la bille nest pas tombee, sinon renvoie lindex de la poche ou elle est tombee
    /// </summary>
    /// <param name="fallingBall"></param>
    /// <returns></returns>
    public int GetWinningPocketIndex(BallRoll fallingBall)
    {
        int result = -1;
        foreach (GameObject pocket in allPockets)
        {
            Vector3 separatingVector = pocket.transform.position - fallingBall.transform.position;
            if (separatingVector.magnitude < pocket.GetComponent<SphereCollider>().radius)
            {
                result = Array.IndexOf(allPockets, pocket);
                break;
            }
        }
        return result;
    }

    public List<int> GetAllConnectedBallsID(int ballID)
    {
        List<BallRoll> possibleBalls = new List<BallRoll>(RemainingBalls);
        BallRoll mainBall = possibleBalls.Find(ball => ball._ballId == ballID);
        possibleBalls.Remove(mainBall);

        List<int> result = new List<int>();

        foreach (BallRoll targetBall in possibleBalls)
        {
            result.Add(targetBall._ballId);
            Vector3 direction = targetBall.transform.position - mainBall.transform.position;
            RaycastHit[] hit = Physics.SphereCastAll(mainBall.transform.position, mainBall.ballRadius, direction.normalized, direction.magnitude);
            foreach (RaycastHit ray in hit)
            {
                BallRoll hitBallRoll = ray.collider.GetComponent<BallRoll>();
                //NB : BUG POSSIBLE = si il y a un collider qui n'est pas celui d'un ballroll sur le chemin, il sera ignoré par la détection
                if (hitBallRoll != null && hitBallRoll._ballId != mainBall._ballId && hitBallRoll._ballId != targetBall._ballId)
                {
                    result.Remove(targetBall._ballId);
                    break;
                }
            }
        }
        return result;
    }

    public List<int> GetAllConnectedPocketsID(int ballID)
    {
        List<int> result = new List<int>();

        BallRoll mainBall = RemainingBalls.Find(ball => ball._ballId == ballID);
        foreach (GameObject targetPocket in allPockets)
        {
            int i = Array.IndexOf(allPockets, targetPocket);
            //Ajout temporaire de la poche dans al liste des poches possibles. Si elle est invalide, on la retire plus bas
            result.Add(i);
            Vector3 direction = targetPocket.transform.position - mainBall.transform.position;
            RaycastHit[] hit = Physics.SphereCastAll(mainBall.transform.position, mainBall.ballRadius, direction.normalized, direction.magnitude);
            foreach (RaycastHit ray in hit)
            {
                GameObject hitGO = ray.collider.gameObject;
                //Si on trouve un collider qui nest ni la bille de ref, ni la poche, alors cest un obstacle sur le passage
                if (hitGO != null && (hitGO != targetPocket && hitGO != mainBall.gameObject))
                {
                    //On retire cette poche de la liste des poches possibles
                    result.Remove(i);
                    break;
                }
            }
        }
        return result;
    }

    public List<BallRoll> GetRemainingBalls()
    {
        return RemainingBalls;
    }
    public GameObject[] GetPockets()
    {
        return allPockets;
    }

    /// <summary>
    /// Calcule les parametres de frappe de la bille blanche pour un empochement dans la poche donnee et la bille finale donnee
    /// </summary>
    /// <param name="pocketID"></param>
    /// <param name="finalNode"></param>
    /// <param name="hitParameters"></param>
    /// <returns></returns>
    public bool CalculateHitParametersForPath(int pocketID, NTree finalNode, out HitParameters hitParameters)
    {
        bool trajectoryIsViable = true;
        hitParameters = new HitParameters(0, Vector3.zero);

        List<NTree> ballPath = new List<NTree> (finalNode.Ancestry);
        ballPath.Reverse(); //Renversement car le premier element est toujours la bille blanche

        // initialisation de l'algo avec la poche en tant que targetBall
        BallRoll pivotBallRoll = RemainingBalls.Find(ball => ball._ballId == finalNode.NodeData.ballID);

        Vector3 separatingVector = allPockets[pocketID].transform.position - pivotBallRoll.transform.position;

        Vector3 initialPivotSpeed = CalculateInitialSpeedToCrossDistance(pivotBallRoll, separatingVector, Vector3.zero);

        BallRoll targetBallRoll = pivotBallRoll;
        Vector3 targetSpeed = initialPivotSpeed;

        //Debug.Log("separating vector to " + pocketID + " from " + pivotBallRoll.name +" is " + separatingVector);

        foreach (NTree pivotNTree in ballPath)
        {
            //MaJ du pivot
            pivotBallRoll = RemainingBalls.Find(ball => ball._ballId == pivotNTree.NodeData.ballID);

            separatingVector = CalculateInAndOutSpeedVector2FromInAndOutSpeedVector1(targetBallRoll, Vector3.zero, targetSpeed, pivotBallRoll, out Vector3 inPivotSpeed, out Vector3 outPivotSpeed);
            //Debug.Log("separating vector to "+targetBallRoll.name+" from "+pivotBallRoll.name+" is " + separatingVector);

            initialPivotSpeed = CalculateInitialSpeedToCrossDistance(pivotBallRoll, separatingVector, inPivotSpeed);

            //Critère d'arret
            trajectoryIsViable = CheckForTrajectoryViability(separatingVector, pivotBallRoll, targetBallRoll);
            if (!trajectoryIsViable) {
                //Debug.Log("an obstacle was on the last ball path");
                break; }

            //Changement de cible
            targetBallRoll = pivotBallRoll;
            targetSpeed = initialPivotSpeed;

        }
        if (trajectoryIsViable)
        {
            hitParameters = CalculateHitParametersForFirstCollision(targetSpeed, targetBallRoll);
            if (hitParameters.Force < cueMinForce || hitParameters.Force > cueMaxForce) {
                //Debug.Log("necessary force was outside authorized bounds");
                trajectoryIsViable = false; }
            else
            {
                //Fonction de debug de l'IA à supprimer
                //ShowHitParametersDebug(hitParameters,  targetBallRoll.transform.position);
            }
        }

        return trajectoryIsViable;
    }

    /// <summary>
    /// Fonction de debug de l'IA à supprimer
    /// </summary>
    /// <param name="hitParam"></param>
    /// <param name="center"></param>
    private void ShowHitParametersDebug(HitParameters hitParam, Vector3 center)
    {
        LineRenderer lineRenderer = GameObject.Find("DebugLineRenderer").GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, center);
        lineRenderer.SetPosition(1, center + hitParam.Direction*10);
    }

    private Vector3 CalculateInAndOutSpeedVector2FromInAndOutSpeedVector1(BallRoll ball1, Vector3 inSpeed1, Vector3 outSpeed1, BallRoll ball2, out Vector3 inSpeed2, out Vector3 outSpeed2)
    {

        // Dapres BounceOnBall, outSpeed1 = m2/m1*normal_inSpeed2 + tangent_InSpeed1 et outSpeed2 = m1/m2*normal_inSpeed1 + tangent_InSpeed2
        // Simplification en passant dans le repere de la bille 1
        Vector3 relativeOutSpeed1 = outSpeed1 - inSpeed1;
        // En reprenant la formule precedente, on na plus que relativeOutSpeed1 = m2/m1*relativeNormal_inSpeed2  et relativOutSpeed2 = relativeTangent_InSpeed2
        // De ca on peut deduire deux infos
        // A) le vecteur normal entre les billes est forcement la direction de sortie de bille 1
        // B) la composante relative normale de inSpeed2

        //Calcul des vecteurs normal et tangent
        Vector3 normalVector = relativeOutSpeed1.normalized;
        Vector3 tangentVector = Vector3.Cross(normalVector, Vector3.up);


        // Calcul du point de collision de bille2 sur bille1
        Vector3 ball2CollisionPosition = ball1.transform.position - normalVector * (ball1.ballRadius + ball2.ballRadius);
        Vector3 separatingVector = (ball2CollisionPosition - ball2.transform.position);
        Vector3 inDirection2 = separatingVector.normalized;

        // Calcul de la composante relative normale de inSpeed2
        float relativeInSpeed2n = ball1.mass / ball2.mass * relativeOutSpeed1.magnitude;

        // Calcul de la composante tangentielle a partir de la composante normale
        float inSpeed2ratio = Vector3.Dot(inDirection2, tangentVector) / Vector3.Dot(inDirection2, normalVector);
        float relativeInSpeed2t = inSpeed2ratio * relativeInSpeed2n;

        // Calcul des vecteurs 2 incident et sortant relatifs
        Vector3 relativeInSpeed2 = relativeInSpeed2t * tangentVector + relativeInSpeed2n * normalVector;
        Vector3 relativeOutSpeed2 = relativeInSpeed2t * tangentVector;

        // Passage au repere standard
        inSpeed2 = relativeInSpeed2 + inSpeed1;
        outSpeed2 = relativeOutSpeed2 + inSpeed1;

        return separatingVector;
    }

    /// <summary>
    /// Calcule la vitesse initiale d'une bille pour qu'elle ait encore une vitesse donnee apres avoir parcourue une distance donnee
    /// </summary>
    /// <param name="separatingVector"></param>
    /// <param name="targetSpeed"></param>
    /// <returns></returns>
    private Vector3 CalculateInitialSpeedToCrossDistance(BallRoll ballRoll, Vector3 separatingVector, Vector3 targetSpeed)
    {
        float currentSpeed = targetSpeed.magnitude;
        float distanceToCross = separatingVector.magnitude;
        float dragAddition = ballRoll.dragAdditor;
        float dragMultiplicator = ballRoll.dragMultiplicator;

        while (distanceToCross > 0)
        {
            distanceToCross -= currentSpeed * Time.fixedDeltaTime;
            currentSpeed += (currentSpeed * dragMultiplicator + dragAdditor) * Time.fixedDeltaTime;
        }

        return separatingVector.normalized * currentSpeed;
    }
    private bool CheckForTrajectoryViability(Vector3 separatingVector, BallRoll currentBall, BallRoll targetBall)
    {
        bool pathIsBlocked = Physics.SphereCast(currentBall.transform.position, currentBall.ballRadius, separatingVector.normalized, out RaycastHit hitInfo, separatingVector.magnitude*0.98f);
        return !pathIsBlocked;
    }

    /// <summary>
    /// Renvoie la force de queue necessaire pour que la bille frappee atteigne une certaine vitesse
    /// </summary>
    /// <param name="currentBallNecessarySpeed"></param>
    /// <returns></returns>
    private HitParameters CalculateHitParametersForFirstCollision(Vector3 targetSpeed, BallRoll whiteBallRoll)
    {
        WhiteBallMove whiteBallMove = (WhiteBallMove)whiteBallRoll;
        HitParameters hitParameters = new HitParameters(targetSpeed.magnitude / whiteBallMove.forceFactor, targetSpeed.normalized);
        return hitParameters;
    }



}
