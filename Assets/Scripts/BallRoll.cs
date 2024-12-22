using UnityEngine;
using System;

public class BallRoll : MonoBehaviour
{
    //Variables g�n�rales
    [SerializeField] protected float mass = 1.0f;
    public bool canYetCollide = true; //true par d�faut, devient false pour le reste de la frame une fois qu'elle a tap� une autre bille

    // Variables de d�placement
    [SerializeField]    protected float speed = 0; // vitesse de la bille � chaque instant
    protected Vector3 direction; // direction de la bille � chaque instant. Normalis�.
    private float drag = 0.5f; // Coef des frottements du tapis sur la bille
    [SerializeField] private float bandSpeedReductionFactor = 0.8f; //coef d'att�nuation de la vitesse par les bandes


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        // Si la vitesse est suffisante, on continue de faire rouelr la bille
        if (speed > 0.1f)
        {
            transform.position += direction * speed * Time.deltaTime;
            speed -= drag * Time.deltaTime; // les frottements sont incarn�s par une r�duction lin�aire de la vitesse
        }
        // Si la vitesse est trop faible, on arr�te la bille. Cela donne un crit�re pour terminer la phase de collisions.
        else { speed = 0; }
    }
    //une fois que toutes les update du jeu ont �t� ex�cut�es, lateupdate s'ex�cute
    private void LateUpdate()
    {
        canYetCollide = true; //maintenant que l'autre bille percut�e a r�solue sa collision, on peut reouvrir la bille locale aux collisions
    }

    protected void OnTriggerEnter(Collider collider)
    {
        
        if (collider.tag == "Bandes")
        {
            Debug.Log(this.name + " percute " + collider.gameObject.name);
            BounceOnBand(collider);
        }
        if (collider.tag == "Bille")
        {
            Debug.Log(this.name+ " percute "+collider.gameObject.name);
            BounceOnBall(collider);
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

            canYetCollide = false;
        }
    }

    /// <summary>
    /// Fait rebondir la bille sur une bande
    /// </summary>
    /// <param name="collider"></param>
    protected void BounceOnBand(Collider collider)
    {
        float xBand = collider.transform.position.x;
        float zBand = collider.transform.position.z;
        Debug.Log(xBand + " " + zBand);

        // Etape 1 = On calcule la normale du rebond, qui d�pend de la bande
        Vector3 normalVector; // Vecteur normal de la bande de rebond
        CalculateBandNormalVector(xBand, zBand, out normalVector);

        //Etape 2 = On calcule la nouvelle direction de la bille post rebond 
        if (normalVector != Vector3.zero)
        {
            speed = speed * bandSpeedReductionFactor;
            direction = Vector3.Reflect( direction, normalVector).normalized;
        }
        
    }
    /// <summary>
    /// Calcule la normale d'une bande dont les coordonn�es X et Z sont en param�tres
    /// </summary>
    /// <param name="xBand"></param>
    /// <param name="zBand"></param>
    /// <param name="normalVector"></param>
    protected void CalculateBandNormalVector(float xBand, float zBand, out Vector3 normalVector)
    {
        if (Math.Abs(xBand) < 1)
        {  // Cas o� la bande est d�cal�e selon l'axe z, et non pas x (donc coordonn�e X est petite)
            normalVector = (zBand * Vector3.back).normalized;
            Debug.Log("normal is " + normalVector);
        }
        else if (Math.Abs(zBand) < 1)
        { // Cas o� la bande est d�cal�e selon l'axe x, et non pas z (donc coordonn�e Z est petite)
            normalVector = (xBand * Vector3.left).normalized;

            Debug.Log("normal is " + normalVector);
        }
        else
        {
            normalVector = Vector3.zero;
            Debug.Log("Coordon�es de la bande incorrectes");
        }

    }
}
