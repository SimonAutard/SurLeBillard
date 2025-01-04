using UnityEngine;
using System;

public class NarrationManager : MonoBehaviour
{
    private GameManager _gameManager;

    //Liste des thèmes de billes
    private string[] themesArray = new string[] {"Finances","Santé","Carrière","Nature","Amitié","Amour","Spiritualité"};
    //Tableau général des correspondances entre deux thèmes et leurs prophéties possibles
    [SerializeField] Prophecy[,] prophecyMasterTable = new Prophecy[1,1]; //initilaisé à 1,1 pour les tests

    System.Random random = new System.Random();
    void OnEnable()
    {
        // subscribe to all events that this component needs to listen to at all time
        EventBus.Subscribe<EventStoryBitGenerationRequest>(HandleStoryBitRequest);
        EventBus.Subscribe<EventStoryUpdateRequest>(HandleStoryUpdateRequest);
        EventBus.Subscribe<EventLoreRequest>(HandleLoreRequest);

        // abonnement à l'evenement collisoin de billes
        BallRoll.TwoBallsCollision += TwoBallsCollisionNarration;
        // Abonnement à l'evenement de clic de la souris
        Controller.OnMouseClicked += CreateRandomStory;
    }

    void OnDisable()
    {
        // Unsubscribe from all events to avoid memory leaks
        EventBus.Unsubscribe<EventStoryBitGenerationRequest>(HandleStoryBitRequest);
        EventBus.Unsubscribe<EventStoryUpdateRequest>(HandleStoryUpdateRequest);
        EventBus.Unsubscribe<EventLoreRequest>(HandleLoreRequest);

        // désbonnement à l'evenement collisoin de billes
        BallRoll.TwoBallsCollision -= TwoBallsCollisionNarration;
        //désabonnement de l'venement clic de souris, pour éviter les memory leaks
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

    // TODO : change method so it can handle the parameters that'll be sent to it
    private void CreateRandomStory(Vector3 useless) 
    {
        int index1 = random.Next(0, themesArray.Length);
        int index2 = random.Next(0, themesArray.Length);
        if (index1 == index2) { index2--; }
        string fullProphecy = prophecyMasterTable[0, 0].GetCompletedProphecy().sentence;
        Debug.Log(fullProphecy);
    }

    /// <summary>
    /// Handles EventStoryUpdateRequest from the reception of the event to the publishing of the delivery
    /// </summary>
    /// <param name="requestEvent">The event containing the data relative to what type of event is requested</param>
    private void HandleStoryBitRequest(EventStoryBitGenerationRequest requestEvent)
    {
        string pos = (requestEvent._positive) ? "positive" : "negative";
        Debug.Log($"NarrationManager: Received a {pos} story bit generation request with '{requestEvent._wordA}' and '{requestEvent._wordB}'");
        // Change this to a CreateRandomStory call when the method will be updated
        string storyBit = GenerateStoryBit(requestEvent._wordA, requestEvent._wordB, requestEvent._positive);
        Debug.Log($"NarrationManager: Publishing story bit generated: {storyBit}");
        EventBus.Publish(new EventStoryBitGenerationDelivery(storyBit));
    }

    /// <summary>
    /// Internal method used to generate a story bit. >>> Placeholder, can be deleted once CreateRandomStory is good to go
    /// </summary>
    /// <param name="wordA">The first theme the story bit is based on</param>
    /// <param name="wordB">The second theme the story bit is based on</param>
    /// <param name="positive">States if the story bit should be positive or not</param>
    /// <returns>The generated story bit</returns>
    private string GenerateStoryBit(string wordA, string wordB, bool positive)
    {
        // TODO everything related to the generation of the story bit, be it Cave of Qud algo or LLM prompt
        string pos = (positive) ? "positive" : "negative";
        return $"Placeholder {pos} story bit based on '{wordA}' and '{wordB}'";
    }
    /// <summary>
    /// Example of method that could be used to handle requests to update the lore. Doesn't do anything for now
    /// </summary>
    /// <param name="requestEvent"></param>

    private void HandleStoryUpdateRequest(EventStoryUpdateRequest requestEvent)
    {
        Debug.Log($"NarrationManager: Updating story with...");
        // TODO update the story with whatever we sent
    }

    /// <summary>
    /// Example of method that could be used to handle requests to deliver lore data (from the reception of the event to the publishing of the delivery). 
    /// Delivers a placeholder string for now
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleLoreRequest(EventLoreRequest requestEvent)
    {
        Debug.Log($"NarrationManager: Received lore request about...");
        string lore = BuildLoreDelivery();
        Debug.Log($"NarrationManager: Publishing lore requested: {lore}");
        EventBus.Publish(new EventLoreDelivery(lore));
    }

    /// <summary>
    /// Example of method that could be used to build the lore data requested, in a specific format
    /// </summary>
    /// <returns>Placeholder string for now</returns>
    private string BuildLoreDelivery()
    {
        // TODO assemble a string or whatever format we decide, containing all the lore info requested.
        string lore = "Placeholder lore";
        return lore;
    }
}
