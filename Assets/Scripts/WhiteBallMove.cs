using UnityEngine;

// La bille hérite de BallRoll qui est la classé générale pour les billes, gérant toute la partie collisions
public class WhiteBallMove : BallRoll
{
    public float forceFactor = 1;// Coef de force du coup de queue
    void OnEnable()
    {
        // Abonnement à l'evenement de clic de la souris
        //Controller.OnMouseClicked += HandleMouseClick;
        EventBus.Subscribe<EventApplyForceToWhiteRequest>(HandleApplyForceEvent);
    }

    void OnDisable()
    {
        //désabonnement de l'venement clic de souris, pour éviter les memory leaks
        //Controller.OnMouseClicked -= HandleMouseClick;
        EventBus.Unsubscribe<EventApplyForceToWhiteRequest>(HandleApplyForceEvent);
    }
    /// <summary>
    /// Fonction de déplacement de la bille blanche reservee au debug car declenchee par clic de souris
    /// </summary>
    /// <param name="clickPosition"></param>
    private void HandleMouseClick(Vector3 clickPosition)
    {
        // Restreint le mouvement aux axes X et Z
        clickPosition.y = transform.position.y;

        // Calcule la direction et la vitesse
        direction = (transform.position - clickPosition).normalized;
        speed = forceFactor * Vector3.Distance(transform.position, clickPosition);
    }

    /// <summary>
    /// Fonction de déplacement de la bille blanche nominale, declenchee par un event du UIManager
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleApplyForceEvent(EventApplyForceToWhiteRequest requestEvent)
    {
        /*
        //Partie debug au cas où l'angle ne amrche pas
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 clickPosition = ray.GetPoint(distance);
            clickPosition.y = transform.position.y;
        }*/

        // Récupère les données du coup
        Vector3 pushVector = requestEvent._vector.normalized;
        float force = requestEvent._force;
        PushThisBall(pushVector, force);

    }
    public void PushThisBall(Vector3 pushVector, float force)
    {
        direction = pushVector;
        speed = force * forceFactor;

    }
}
