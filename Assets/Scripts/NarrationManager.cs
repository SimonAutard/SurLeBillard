using UnityEngine;
using System;

public class NarrationManager : MonoBehaviour
{
    //Liste des th�mes de billes
    private string[] themesArray = new string[] {"Finances","Sant�","Carri�re","Nature","Amiti�","Amour","Spiritualit�"};
    //Tableau g�n�ral des correspondances entre deux th�mes et leurs proph�ties possibles
    [SerializeField] Prophecy[,] prophecyMasterTable = new Prophecy[1,1]; //initilais� � 1,1 pour les tests


    System.Random random = new System.Random();
    void OnEnable()
    {
        // abonnement � l'evenement collisoin de billes
        BallRoll.TwoBallsCollision += TwoBallsCollisionNarration;
        // Abonnement � l'evenement de clic de la souris
        Controller.OnMouseClicked += CreateRandomStory;
    }

    void OnDisable()
    {
        // d�sbonnement � l'evenement collisoin de billes
        BallRoll.TwoBallsCollision -= TwoBallsCollisionNarration;
        //d�sabonnement de l'venement clic de souris, pour �viter les memory leaks
        Controller.OnMouseClicked -= CreateRandomStory;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //initialisation simple pour test
        Prophecy prophecy = new Prophecy(0);
        prophecyMasterTable[0,0] = prophecy;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void TwoBallsCollisionNarration(string ball1Theme, string ball2Theme)
    {
        Debug.Log(ball2Theme + " " + ball1Theme);
    }

    private void CreateRandomStory(Vector3 useless) 
    {
        int index1 = random.Next(0, themesArray.Length);
        int index2 = random.Next(0, themesArray.Length);
        if (index1 == index2) { index2--; }
        string fullProphecy = prophecyMasterTable[0, 0].GetCompletedProphecy().sentence;
        Debug.Log(fullProphecy);
    }
}
