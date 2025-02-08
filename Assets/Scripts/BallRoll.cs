using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallRoll : MonoBehaviour
{
    //Variable générale
    PhysicsScene physicsScene;

    //Variables physiques
    [SerializeField] protected float mass = 1.0f;
    public float ballRadius { get; protected set; }
    public bool canYetCollide = true; //true par d�faut, devient false pour le reste de la frame une fois qu'elle a tap� une autre bille ou une bande
    private Collider currentSuperimposedCollider;

    //Vriables narratives
    public string ballTheme; //th�me de la bille
    public int _ballId;

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

    void Update()
    {
        // La bille avance ou sarrete
        RollTheBall(Time.deltaTime);
        // On gère les collisions
        HandleCollisions(physicsScene);
        // Verification de la position de la bille au dessus des poches
        CheckPocketing();
    }

    //Une fois que toutes les update du jeu ont �t� ex�cut�es, lateupdate s'ex�cute
    protected void LateUpdate()
    {
        UpdateCollisionCondition(); //maintenant que l'autre bille percut�e a r�solue sa collision, on peut reouvrir la bille locale aux collisions
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
        }
        // Si la vitesse est trop faible, on arr�te la bille. Cela donne un crit�re pour terminer la phase de collisions.
        else { speed = 0; }
    }
    public Collider HandleCollisions(PhysicsScene physicsScene)
    {
        /*
        // On recupere tous els collider superposes
        Collider[] hits = new Collider[10];
        int hitNb = physicsScene.OverlapSphere(transform.position, ballRadius, hits, ~0, QueryTriggerInteraction.UseGlobal);
        List<Collider> hitsList = hits.ToList(); // transformation en liste
        */
        List<Collider> hitsList = new List<Collider>();
        if (isRealBall) { hitsList = PhysicsManager.Instance.FindCollidersRealScene(this); }
        else{ hitsList = TrajectorySimulationManager.Instance.FindCollidersSimlatedScene(this); }
        hitsList.RemoveAll(item => item == gameObject.GetComponent<Collider>()); //on retirre le collider d ela bille lui meme
        hitsList.RemoveAll(item => item == null); //on retire les hits qui n'ont pas toruvé de collider

        if (hitsList.Count == 0)
        {
            currentSuperimposedCollider = null;
        }
        else
        {
            Debug.Log(name + " se superpose a " + hitsList[0].name);
            //Stockage du collider alien pour UpdateCollisionCondition
            currentSuperimposedCollider = hitsList[0];
            //Gestion de la collision
            AnswerToCollisionWith(hitsList[0]);

        }
        return currentSuperimposedCollider;
    }

    /// <summary>
    /// Regit las conditions sous elsquelles la bille peut percuter une autre
    /// </summary>
    public void UpdateCollisionCondition()
    {
        if (canYetCollide)
        {
            if (currentSuperimposedCollider != null)
            {
                //Rien a faire, deja gere pendant AnswerToCollisionWith
            }
            else
            { //Rien a faire, la bille pouvait percuter et na rien percute
            }
        }
        else
        {
            if (currentSuperimposedCollider != null)
            {
                //Rien a faire, la bille nest pas encore sortie de lautre collider
            }
            else { canYetCollide = true; } // la bille est sortie de lautre collider, elle peut de nouveau percuter
        }
    }
    /*
    protected void OnTriggerEnter(Collider collider)
    {
        currentSuperimposedCollider = collider;
        AnswerToCollisionWith(collider);
    }
    */
    public void AnswerToCollisionWith(Collider collider)
    {
        if (canYetCollide)
        {
            if (collider.tag == "Bandes")
            {
                Debug.Log(name + " percute " + collider.name);
                BounceOnBand(collider);
                canYetCollide = false;
            }
            if (collider.tag == "Bille")
            {
                Debug.Log(name + " percute " + collider.name);
                BounceOnBall(collider);
                canYetCollide = false;
            }
        }
    }

    /// <summary>
    /// Fait rebondir la bille sur une autre bille, pass�e en argument
    /// </summary>
    /// <param name="collider"></param>
    protected void BounceOnBall(Collider collider)
    {
        if (collider.GetComponent<BallRoll>().canYetCollide)
        {
            float collidingBallMass = collider.GetComponent<BallRoll>().mass;
            float collidingBallspeed = collider.GetComponent<BallRoll>().speed;
            Vector3 collidingBallDirection = collider.GetComponent<BallRoll>().direction;

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


            bool valence = GetValence();

            if (isRealBall)
            {
                EventBus.Publish(new EventCollisionSignal(_ballId, collider.GetComponent<BallRoll>()._ballId, ballTheme, collider.GetComponent<BallRoll>().ballTheme, valence));
            }

            //TwoBallsCollision?.Invoke(ballSymbol, collider.GetComponent<BallRoll>().ballSymbol);
        }
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


}