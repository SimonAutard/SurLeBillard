using UnityEngine;

// La bille hérite de BallRoll qui est la classé générale pour les billes, gérant toute la partie collisions
public class WhiteBallMove : BallRoll
{
    void OnEnable()
    {
        // Abonnement à l'evenement de clic de la souris
        Controller.OnMouseClicked += HandleMouseClick;
    }

    void OnDisable()
    {
        //désabonnement de l'venement clic de souris, pour éviter les memory leaks
        Controller.OnMouseClicked -= HandleMouseClick;
    }

    private void HandleMouseClick(Vector3 clickPosition)
    {
        // Restreint le mouvement aux axes X et Z
        clickPosition.y = transform.position.y;

        // Calcule la direction et la vitesse
        direction = (transform.position - clickPosition).normalized;
        speed = Vector3.Distance(transform.position, clickPosition);
    }

}
