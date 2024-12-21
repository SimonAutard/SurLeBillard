using UnityEngine;


public class WhiteBallMove : BallRoll
{

    void OnEnable()
    {
        Controller.OnMouseClicked += HandleMouseClick;
    }

    void OnDisable()
    {
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
