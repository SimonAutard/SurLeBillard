using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BallRoll : MonoBehaviour
{
    //Variable générale
    PhysicsScene physicsScene;

    //Variables physiques
    [SerializeField] protected float mass = 1.0f;
    public float ballRadius { get; protected set; }
    private List<Collider> currentSuperimposedColliders = new List<Collider>(); // Liste de tous les collider superposes a la bille pendant cette frame
    private Collider colliderInProcessing; // collider en trai detre processe par la physique de rebond

    //Vriables narratives
    public string ballTheme; //th�me de la bille
    public int _ballId;

    //Variables graphiques
    private GameObject puppetGOPrefab;
    private VisualEffect effect;

    // Variables de d�placement
    public float speed;//{ get; protected set; } // vitesse de la bille � chaque instant
    public Vector3 direction { get; protected set; } // direction de la bille � chaque instant. Normalis�.
    [SerializeField] private float dragMultiplicator = 0.5f; // Coef des frottements du tapis sur la bille
    [SerializeField] private float dragAddition = 0.5f; // Coef des frottements du tapis sur la bille
    [SerializeField] private float bandSpeedReductionFactor = 0.8f; //coef d'att�nuation de la vitesse par les bandes
    private float minSpeedToMove = 0.1f;

    //Evenements
    public static event Action<string, string> TwoBallsCollision; // evenement de la collision de deux billes
    public static event Action<BallRoll> BallPocketed; // evenement de destruction de la bille
    protected bool isRealBall = true;

    private void OnEnable()
    {
        effect = GetComponent<VisualEffect>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeBallRollParameters();
    }

    public void InitializeBallRollParameters()
    {
        // Vitese seuil sous laquelle la bille est consideree arretee
        minSpeedToMove = PhysicsManager.Instance.minSpeedForBalls;
        ballRadius = GetComponent<SphereCollider>().radius;
        speed = 0;
        physicsScene = TrajectorySimulationManager.Instance._realPhysicsScene;
    }

    void FixedUpdate()
    {
        // La bille avance ou sarrete
        //RollTheBall(PhysicsManager.Instance.generalTimeStep);
        RollTheBall(Time.deltaTime);
        // Verification de la position de la bille au dessus des poches
        CheckPocketing();
    }

    //Une fois que toutes les update du jeu ont �t� ex�cut�es, lateupdate s'ex�cute
    protected void LateUpdate()
    {
        // On gère les collisions
        HandleCollisions(physicsScene);

    }

    /// <summary>
    /// Fait avancer la bille d'un certain time step
    /// </summary>
    public void RollTheBall(float timestep)
    {
        // Si la vitesse est suffisante, on continue de faire rouelr la bille
        if (speed > minSpeedToMove)
        {
            transform.position += direction * speed * timestep;
            speed -= (speed * dragMultiplicator + dragAddition) * timestep; // les frottements sont incarn�s par une r�duction lin�aire de la vitesse
            if (isRealBall)
            {
                RotateBall(timestep);
            }
        }
        // Si la vitesse est trop faible, on arr�te la bille. Cela donne un crit�re pour terminer la phase de collisions.
        else { speed = 0; }
    }

    /// <summary>
    /// Renvoie le collider dont al bille est en train de gerer la collision, si il existe. Sinon renvoie null
    /// </summary>
    /// <param name="physicsScene"></param>
    /// <returns></returns>
    public Collider HandleCollisions(PhysicsScene physicsScene)
    {
        //recuperation des collider superposes
        //Cas de la vraie scene
        if (isRealBall) { currentSuperimposedColliders = PhysicsManager.Instance.FindCollidersRealScene(this); }
        //Cas de la simulation
        else { currentSuperimposedColliders = TrajectorySimulationManager.Instance.FindCollidersSimulatedScene(this); }
        currentSuperimposedColliders.RemoveAll(item => item == gameObject.GetComponent<Collider>()); //on retirre le collider d ela bille lui meme
        currentSuperimposedColliders.RemoveAll(item => item == null); //on retire les hits qui n'ont pas toruvé de collider

        //Cas ou aucun collider na ete detecte
        if (currentSuperimposedColliders.Count == 0)
        {
            //Dans ce cas, il ny a plus de collider en cours de processing
            colliderInProcessing = null;
        }
        //Cas ou le collider en cours de processing se trouve parmi les collider superposes
        else if (colliderInProcessing != null && currentSuperimposedColliders.Contains(colliderInProcessing))
        {
            //Debug.Log(currentSuperimposedColliders[0].name+" collider remains untocuched, " + colliderInProcessing.name + " is still processed");

            //Dans ce cas, c'est quon na pas fini de process la collision avec lui.
            //On ne fait rien, car le process a deja ete initialise quand cet objet a ete affecte la premiere fois
        }
        //Cas ou le collider en cours de processing ne se trouve plus parmi les collider superposes
        //Cela signifie quon a fini de process la collision avec ce colldier, on peut donc commencer a process une nouvelle collision
        else
        {

            //Mise a jour du collider en cours de process. on prend arbitrairement le premier de la liste des colliders superposes
            colliderInProcessing = currentSuperimposedColliders[0];
            //Dans le cas d'une collision bille-bille, on met egalement a jour le collider en cours de process de cette autre bille
            if (colliderInProcessing.gameObject.tag == "Bille") { colliderInProcessing.GetComponent<BallRoll>().ProcessThisCollider(gameObject.GetComponent<Collider>()); }
            //Gestion de la collision
            AnswerToCollisionWith(currentSuperimposedColliders[0]);

        }
        return colliderInProcessing;
    }

    /// <summary>
    /// Determine si on doit gerer une collision bille-bande ou bille-bille
    /// </summary>
    /// <param name="collider"></param>
    private void AnswerToCollisionWith(Collider collider)
    {
        if (collider.tag == "Bandes")
        {
            //Debug.Log(name + " answers to collision with " + collider.name);
            BounceOnBand(collider);
        }
        if (collider.tag == "Bille")
        {
            //Debug.Log(name + " answers to collision with  " + collider.name);
            BounceOnBall(collider);
        }

    }

    /// <summary>
    /// Fait rebondir la bille sur une autre bille, pass�e en argument
    /// </summary>
    /// <param name="collider"></param>
    protected void BounceOnBall(Collider collider)
    {
        BallRoll collidingBallRoll = collider.GetComponent<BallRoll>();
        float collidingBallMass = collidingBallRoll.mass;
        float collidingBallspeed = collidingBallRoll.speed;
        Vector3 collidingBallDirection = collidingBallRoll.direction;

        Vector3 normalVector = (collider.transform.position - transform.position).normalized;
        Vector3 tangentVector = Vector3.Cross(normalVector, Vector3.up);

        // Calcul des  vitesses tangentielles
        Vector3 v1t;
        v1t = mass * speed * Vector3.Project(direction, tangentVector);
        Vector3 v2t;
        v2t = collidingBallMass * collidingBallspeed * Vector3.Project(collidingBallDirection, tangentVector);

        // Calcul des vitesses normales
        Vector3 v1n;
        v1n = collidingBallMass * collidingBallspeed * Vector3.Project(collidingBallDirection, normalVector);
        Vector3 v2n;
        v2n = mass * speed * Vector3.Project(direction, normalVector);

        // Calcul des vitesses finales de chaque bille
        Vector3 v1f = (v1n + v1t) / mass;
        speed = v1f.magnitude;
        direction = v1f.normalized;
        Vector3 v2f = (v2n + v2t) / collidingBallMass;
        collider.GetComponent<BallRoll>().speed = v2f.magnitude;
        collider.GetComponent<BallRoll>().direction = v2f.normalized;

        //Recupere la polarite du terrain
        bool valence = GetValence();

        //Si la bille est vraie, envoie un signal au narrationManager pour generer une prophetie
        if (isRealBall)
        {
            // Verifiction quaucune des billes percutees nest la blanche
            if (_ballId * collidingBallRoll._ballId != 0)
            {
                // Declencher les VFX
                DropVFXAnchor();
                collidingBallRoll.DropVFXAnchor();
            }

            //Generer la prophetie
            EventBus.Publish(new EventCollisionSignal(_ballId, collidingBallRoll._ballId, ballTheme, collidingBallRoll.ballTheme, valence));
        }

        //TwoBallsCollision?.Invoke(ballSymbol, collider.GetComponent<BallRoll>().ballSymbol);

    }

    /// <summary>
    /// Fait rebondir la bille sur une bande
    /// </summary>
    /// <param name="collider"></param>
    protected void BounceOnBand(Collider collider)
    {

        // Etape 1 = On calcule la normale du rebond, qui d�pend de la bande
        Vector3 normalVector = collider.GetComponent<BandBehavior>().normalVector.normalized; // Vecteur normal de la bande de rebond
        //Debug.Log("normal vector of band is "+normalVector);
        //Etape 2 = On calcule la nouvelle direction de la bille post rebond 
        if (normalVector != Vector3.zero)
        {
            speed = speed * bandSpeedReductionFactor;
            direction = Vector3.Reflect(direction, normalVector).normalized;
            if (isRealBall)
            {
                EventBus.Publish(new EventBounceOnBandSignal());
            }
        }

    }
    /// <summary>
    /// Verifie si la bille a ete empochee
    /// </summary>
    public virtual void CheckPocketing()
    {
        // Raycats du centre de la bille vers le bas
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit);
        //Verification qu'un collider a ete touche et quil sagissait dune poche
        if (hit.collider != null && hit.collider.gameObject.tag == "Poche")
        {
            if (isRealBall) { EventBus.Publish(new EventPocketingSignal(this, 0)); }
            if (!isRealBall) { ImmobilizeBallInSimulation(); }
            Destroy(this.gameObject);
        }

    }

    public void ProcessThisCollider(Collider collider)
    {
        colliderInProcessing = collider;
    }

    /// <summary>
    /// imite la destruction de la bille dans le cadre de la simulation
    /// </summary>
    void ImmobilizeBallInSimulation()
    {
        // Annulation de la vitesse pour que la bille reste en palce et que sa previz de trajectoire la montre immobilisee
        speed = 0;
        // suppression du collider pour que d'autres billes puissent rentrer dans la meme poche
        this.GetComponent<Collider>().enabled = false;
    }

    /// <summary>
    /// Renvoie true si la balle est dans une zone positive, false sinon
    /// </summary>
    /// <returns></returns>
    public bool GetValence()
    {
        if (transform.position.x > 0) { return true; }
        else { return false; }
    }

    public void TurnToSimulation()
    {
        isRealBall = false;
    }

    public void DropVFXAnchor()
    {
        //Ingnorer l'action si c'est la bille blanche
        if (_ballId == 0) { return; }

        //Création de l'ancre
        GameObject dummy = new GameObject("StandInVFXFor" + ballTheme);
        //Positionnement de l'ancre
        dummy.transform.position = transform.position;

        //Trabnsmission du vfx
        VisualEffect anchorEffect = dummy.AddComponent<VisualEffect>();
        anchorEffect.visualEffectAsset = effect.visualEffectAsset;
        //Transmission du sorting layer
        anchorEffect.GetComponent<VFXRenderer>().sortingOrder = effect.GetComponent<VFXRenderer>().sortingOrder;

        //Ajout du script d'autonomie à l'ancre
        VFXAnchor vfxAnchor = dummy.AddComponent<VFXAnchor>();
        //Declenchement du vfx depuis 'lancre
        vfxAnchor.TriggerVFXProcess();

    }

    /// <summary>
    /// Fait rouler la bille sur elle meme dans la direction de son déplacement
    /// </summary>
    /// <param name="timestep"></param>
    private void RotateBall(float timestep)
    {
        //vitesse angulaire
        float angularSpeed = speed / ballRadius;
        //quantité de de degrés avancés pedant la frame
        float movedAngle = angularSpeed * Mathf.Rad2Deg * timestep;
        //axe de rotation
        Vector3 rotationAxis = -Vector3.Cross(direction, Vector3.up).normalized;
        //roulement de la bille
        transform.Rotate(rotationAxis, movedAngle, Space.World);
    }
}