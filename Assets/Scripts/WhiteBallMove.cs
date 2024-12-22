using UnityEngine;

// La bille h�rite de BallRoll qui est la class� g�n�rale pour les billes, g�rant toute la partie collisions
public class WhiteBallMove : BallRoll
{
    void OnEnable()
    {
        // Abonnement � l'evenement de clic de la souris
        Controller.OnMouseClicked += HandleMouseClick;
    }

    void OnDisable()
    {
        //d�sabonnement de l'venement clic de souris, pour �viter les memory leaks
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
