using UnityEngine;
using System;
using System.Reflection;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;


public class NarrationManager : MonoBehaviour
{
    private GameManager _gameManager;

    System.Random random = new System.Random(); // instance pour les evenemnets aleatoires

    //Liste des th�mes de billes
    private string[] themesArray = new string[] { "Finances", "Sant�", "Carri�re", "Nature", "Amiti�", "Amour", "Spiritualit�" };

    //PROPHETIES
    //Tableau g�n�ral des correspondances entre deux th�mes et leurs proph�ties possibles
    [SerializeField] Prophecy[,] prophecyMasterTable = new Prophecy[1, 1]; //initilais� � 1,1 pour les tests

    //STORY ENTITIES
    //Perso principal
    MainCharacter MainCharacter;
    //Listes des types d'entit�s
    List<Character> allCharacters = new List<Character>();
    List<Place> allPlaces = new List<Place>();
    List<StoryActivity> allStoryActivities = new List<StoryActivity>();
    List<StoryItem> allStoryItems = new List<StoryItem>();

    // Design pattern du singleton
    private static NarrationManager instance; // instance statique du narration manager

    public static NarrationManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /*
    public NarrationManager(GameManager gameManager)
    {
        _gameManager = gameManager;
    }*/

    void OnEnable()
    {
        // subscribe to all events that this component needs to listen to at all time
        EventBus.Subscribe<EventStoryBitGenerationRequest>(HandleStoryBitRequest);
        EventBus.Subscribe<EventStoryUpdateRequest>(HandleStoryUpdateRequest);
        EventBus.Subscribe<EventLoreRequest>(HandleLoreRequest);

        // abonnement � l'evenement collisoin de billes
        BallRoll.TwoBallsCollision += TwoBallsCollisionNarration;
        // Abonnement � l'evenement de clic de la souris
        Controller.OnMouseClicked += CreateRandomStory;
    }

    void OnDisable()
    {
        // Unsubscribe from all events to avoid memory leaks
        EventBus.Unsubscribe<EventStoryBitGenerationRequest>(HandleStoryBitRequest);
        EventBus.Unsubscribe<EventStoryUpdateRequest>(HandleStoryUpdateRequest);
        EventBus.Unsubscribe<EventLoreRequest>(HandleLoreRequest);

        // d�sbonnement � l'evenement collisoin de billes
        BallRoll.TwoBallsCollision -= TwoBallsCollisionNarration;
        //d�sabonnement de l'venement clic de souris, pour �viter les memory leaks
        Controller.OnMouseClicked -= CreateRandomStory;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {       
        //initialisation entity simple pour test
        Character charTest = new Character(name: "sarah", mainCharacterBond: 60, health: 10);
        allCharacters.Add(charTest);
        charTest = new Character(name: "william", mainCharacterBond: 60, health: 10);
        allCharacters.Add(charTest);
        charTest = new Character(name: "henry", mainCharacterBond: 60, health: 60);
        allCharacters.Add(charTest);

        Place placeTest = new Place(name: "dublin", mainCharacterBond: 10, placeType: "City");
        allPlaces.Add(placeTest);
        placeTest = new Place(name: "Moher", mainCharacterBond: 60, placeType: "Nature");
        allPlaces.Add(placeTest);
        placeTest = new Place(name: "Galway", mainCharacterBond: 60, placeType: "City");
        allPlaces.Add(placeTest);

        //initialisation pr�ph�tie simple pour test
        string sentence = "connor va manger avec {0} pr�s de {1}";
        Type[] types = new Type[] { typeof(Character), typeof(Place) };
        (string, object)[] valchar = new (string, object)[] { ("BondMin", 30), ("HealthMin", 30) };
        (string, object)[] valplace = new (string, object)[] { ("BondMin", 100), ("PlaceTypeIs", "City") };
        List<(string, object)[]> listval = new List<(string, object)[]>() { null, valplace };
        (string, object)[] upd = new (string, object)[] { ("BondPlus", 1) };
        List<(string, object)[]> listupd = new List<(string, object)[]>() { upd, null };
        Prophecy prophecy = new Prophecy(sentence, types, listval, listupd);
        prophecyMasterTable[0, 0] = prophecy;
        CreateRandomStory(Vector3.zero);
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
        LegoProphecy legoProphecy = prophecyMasterTable[0, 0].GetCompletedProphecy();
        Debug.Log(legoProphecy.Sentence);
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

    /// <summary>
    /// Va chercher une  entit� qui correspond aux propri�t�s  fournies en argument 
    /// </summary>
    public StoryEntity GetFittingEntity(Type requiredType, (string, object)[] validators)
    {
        //Liste des entit�s du type demand� en argument renseign� sous type g�n�rique
        List<StoryEntity> possibleEntities = new List<StoryEntity>();
        //Selection de la liste de story entities correspodnant au type demand�
        if (requiredType == typeof(Character)) { possibleEntities = allCharacters.Cast<StoryEntity>().ToList(); }
        if (requiredType == typeof(Place)) { possibleEntities = allPlaces.Cast<StoryEntity>().ToList(); }
        if (requiredType == typeof(StoryActivity)) { possibleEntities = allStoryActivities.Cast<StoryEntity>().ToList(); }
        if (requiredType == typeof(StoryItem)) { possibleEntities = allStoryItems.Cast<StoryEntity>().ToList(); }

        //Liste des entit�s r�pondant � tous les crit�res demand�s
        List<StoryEntity> viableEntities = new List<StoryEntity>(); //Initialis�e vide et remplie au fur et � mesure
        if (validators != null)
        {
            foreach (StoryEntity entity in possibleEntities) // Check de chaque entite viable
            {
                bool isCandidate = true; // booleen de la candidature de cette entite pour la prophetie
                foreach ((string, object) validator in validators)
                {
                    string methodName = validator.Item1; //nom de la fonction de check de lentite
                    object methodValue = validator.Item2; // valeur associee
                                                          // Trouver la m�thode par son nom
                    MethodInfo method = requiredType.GetMethod(methodName);
                    //Verification de l'existence de la m�thode demand�e
                    if (method != null)
                    {
                        // Appeler la m�thode en passant les param�tres
                        isCandidate = (bool)method.Invoke(entity, new object[1] { methodValue });
                    }
                    else { throw new Exception("M�thode introuvable : " + methodName); }
                    // On sort de la boucle des quun critere nest pas verifie pour passer a lentite suivante parmi les candidates
                    if (!isCandidate) { break; }
                }
                // Si la candidature est valid�e, on ajoute cette entite a la liste des entities viables
                if (isCandidate) { viableEntities.Add(entity); }
            }
        }
        // si aucun validator n'a ete renseign� pour cette entite, alors on prend n'importe quelle entit�
        else { viableEntities = possibleEntities; }
        // si aucune entite ne remplissait les crit�res, alors on en cree une 
        if (viableEntities.Count == 0) { viableEntities.Add(new StoryEntity("fake entity", 50)); }
        // On renvoie une entite aleatoire parmi les viables
        return viableEntities[random.Next(0, viableEntities.Count)];
    }
}
