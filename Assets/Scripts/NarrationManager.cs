using UnityEngine;

public class NarrationManager : MonoBehaviour
{
    void OnEnable()
    {
        // Abonnement à l'evenement de clic de la souris
        BallRoll.TwoBallsCollision += TwoBallsCollisionNarration;
    }

    void OnDisable()
    {
        //désabonnement de l'venement clic de souris, pour éviter les memory leaks
        BallRoll.TwoBallsCollision -= TwoBallsCollisionNarration;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void TwoBallsCollisionNarration(string ball1Symbol, string ball2Symbol)
    {
        Debug.Log(ball2Symbol + " " + ball1Symbol);
    }
}
