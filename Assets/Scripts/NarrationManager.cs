using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class NarrationManager : MonoBehaviour
{
    private GameManager _gameManager;

    System.Random random = new System.Random(); // instance pour les evenemnets aleatoires

    //Liste des thèmes de billes
    private string[] themesArray = new string[] { "Finances", "Santé", "Carrière", "Nature", "Amitié", "Amour", "Spiritualité" };

    //PROPHETIES
    //Tableau général des correspondances entre deux thèmes et leurs prophéties possibles
    [SerializeField] Prophecy[,] prophecyMasterTable = new Prophecy[1, 1]; //initilaisé à 1,1 pour les tests
    private string prophecyFilePath = "Assets/Scripts/Data/prophecydata.csv";

    //STORY ENTITIES
    //Perso principal
    MainCharacter MainCharacter;
    //Listes des types d'entités
    List<StoryCharacter> allCharacters = new List<StoryCharacter>();
    private string characterFilePath = "Assets/Scripts/Data/characterdata.csv";
    List<StoryPlace> allPlaces = new List<StoryPlace>();
    private string placeFilePath = "Assets/Scripts/Data/placedata.csv";
    List<StoryActivity> allStoryActivities = new List<StoryActivity>();
    private string activityFilePath = "Assets/Scripts/Data/activitydata.csv";
    List<StoryItem> allStoryItems = new List<StoryItem>();
    private string itemFilePath = "Assets/Scripts/Data/itemdata.csv";

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
        InitializeProphecies();

        //initialisation entity simple pour test
        StoryCharacter charTest = new StoryCharacter(name: "sarah", mainCharacterBond: 60, health: 10);
        allCharacters.Add(charTest);
        charTest = new StoryCharacter(name: "william", mainCharacterBond: 60, health: 10);
        allCharacters.Add(charTest);
        charTest = new StoryCharacter(name: "henry", mainCharacterBond: 60, health: 60);
        allCharacters.Add(charTest);

        StoryPlace placeTest = new StoryPlace(name: "dublin", mainCharacterBond: 10, placeType: "City");
        allPlaces.Add(placeTest);
        placeTest = new StoryPlace(name: "Moher", mainCharacterBond: 60, placeType: "Nature");
        allPlaces.Add(placeTest);
        placeTest = new StoryPlace(name: "Galway", mainCharacterBond: 60, placeType: "City");
        allPlaces.Add(placeTest);

        //initialisation préphétie simple pour test
        string sentence = "connor va manger avec {0} près de {1}";
        Type[] types = new Type[] { typeof(StoryCharacter), typeof(StoryPlace) };
        (string, object)[] valchar = new (string, object)[] { ("BondMin", 30), ("HealthMin", 30) };
        (string, object)[] valplace = new (string, object)[] { ("BondMin", 30), ("PlaceTypeIs", "City") };
        List<(string, object)[]> listval = new List<(string, object)[]>() { valchar, valplace };
        (string, object)[] updChar = new (string, object)[] { ("BondPlus", 1), ("HealthPlus", 1) };
        (string, object)[] updPlace = new (string, object)[] { ("BondPlus", -100), ("StatePlus", 100) };


        List<(string, object)[]> listupd = new List<(string, object)[]>() { updChar, updPlace };
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
        Debug.Log("etat initial de " + legoProphecy.StoryEntities[0].Name + " - bond = " + legoProphecy.StoryEntities[0].MainCharacterBond + " - health = " + ((StoryCharacter)legoProphecy.StoryEntities[0]).Health);
        Debug.Log("etat initial de " + legoProphecy.StoryEntities[1].Name + " - bond = " + legoProphecy.StoryEntities[1].MainCharacterBond + " - state = " + ((StoryPlace)legoProphecy.StoryEntities[1]).State);
        UpdateStoryEntitiesFromProphecy(legoProphecy.StoryEntities, prophecyMasterTable[0, 0].ProphecyUpdators);
        Debug.Log("etat final de " + legoProphecy.StoryEntities[0].Name + " - bond = " + legoProphecy.StoryEntities[0].MainCharacterBond + " - health = " + ((StoryCharacter)legoProphecy.StoryEntities[0]).Health);
        Debug.Log("etat final de " + legoProphecy.StoryEntities[1].Name + " - bond = " + legoProphecy.StoryEntities[1].MainCharacterBond + " - state = " + ((StoryPlace)legoProphecy.StoryEntities[1]).State);
    }

    /// <summary>
    /// Handles EventStoryUpdateRequest from the reception of the ev ent to the publishing of the delivery
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
    /// Va chercher une  entité qui correspond aux propriétés  fournies en argument 
    /// </summary>
    public StoryEntity GetFittingEntity(Type requiredType, (string, object)[] validators)
    {
        //Liste des entités du type demandé en argument renseigné sous type générique
        List<StoryEntity> possibleEntities = new List<StoryEntity>();
        //Selection de la liste de story entities correspodnant au type demandé
        if (requiredType == typeof(StoryCharacter)) { possibleEntities = allCharacters.Cast<StoryEntity>().ToList(); }
        if (requiredType == typeof(StoryPlace)) { possibleEntities = allPlaces.Cast<StoryEntity>().ToList(); }
        if (requiredType == typeof(StoryActivity)) { possibleEntities = allStoryActivities.Cast<StoryEntity>().ToList(); }
        if (requiredType == typeof(StoryItem)) { possibleEntities = allStoryItems.Cast<StoryEntity>().ToList(); }

        //Liste des entités répondant à tous les critères demandés
        List<StoryEntity> viableEntities = new List<StoryEntity>(); //Initialisée vide et remplie au fur et à mesure
        if (validators != null)
        {
            foreach (StoryEntity entity in possibleEntities) // Check de chaque entite viable
            {
                bool isCandidate = true; // booleen de la candidature de cette entite pour la prophetie
                foreach ((string, object) validator in validators)
                {
                    string methodName = validator.Item1; //nom de la fonction de check de lentite
                    object methodValue = validator.Item2; // valeur associee
                                                          // Trouver la méthode par son nom
                    MethodInfo method = requiredType.GetMethod(methodName);
                    //Verification de l'existence de la méthode demandée
                    if (method != null)
                    {
                        // Appeler la méthode en passant les paramètres
                        isCandidate = (bool)method.Invoke(entity, new object[1] { methodValue });
                    }
                    else { throw new Exception("Méthode introuvable : " + methodName); }
                    // On sort de la boucle des quun critere nest pas verifie pour passer a lentite suivante parmi les candidates
                    if (!isCandidate) { break; }
                }
                // Si la candidature est validée, on ajoute cette entite a la liste des entities viables
                if (isCandidate) { viableEntities.Add(entity); }
            }
        }
        // si aucun validator n'a ete renseigné pour cette entite, alors on prend n'importe quelle entité
        else { viableEntities = possibleEntities; }
        // si aucune entite ne remplissait les critères, alors on en cree une 
        if (viableEntities.Count == 0) { viableEntities.Add(new StoryEntity("fake entity", 50)); }
        // On renvoie une entite aleatoire parmi les viables
        return viableEntities[random.Next(0, viableEntities.Count)];
    }


    /// <summary>
    /// Change létat des story entities du jeu en fonction du résultat de la prophétie
    /// </summary>
    /// <param name="gameEntityList"></param>
    public void UpdateStoryEntitiesFromProphecy(StoryEntity[] gameEntityList, List<(string, object)[]> updators)
    {
        for (int i = 0; i < gameEntityList.Length; i++)
        {
            foreach ((string, object) updator in updators[i])
            {
                string methodName = updator.Item1; //nom de la fonction de'update de lentite
                object methodValue = updator.Item2; // valeur associee

                // Trouver la méthode par son nom
                MethodInfo method = gameEntityList[i].GetType().GetMethod(methodName);
                //Verification de l'existence de la méthode demandée
                if (method != null)
                {
                    // Appeler la méthode en passant les paramètres
                    method.Invoke(gameEntityList[i], new object[1] { methodValue });
                }
                else { throw new Exception("Méthode introuvable : " + methodName); }
            }
        }
    }
    /// <summary>
    /// Cette fonction récupere les donnees csv de propheties et les convertit en instances de propheties
    /// </summary>
    void InitializeProphecies()
    {
        string[] lines = RawDataInitializationChecks(prophecyFilePath);
        if (lines != null)
        {
            // Traitement des lignes restantes (données)
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] cells = line.Split(';');

                // Instanciation de la prophetie
                string sentence = cells[2];

                Type[] entityTypes = ExtractProphecyTypes(cells[3]);

                //INSERER ICI RECUPERATION DES INFOS DE SELF CONDITION

                List<(string, object)[]> validators = ExtractProphecyValidators(cells[5], cells[6], cells[7]);

                List<(string, object)[]> updators = ExtractProphecyValidators(cells[9], cells[10], cells[11]); //Normalement la fonction devrait etre differetnte de celle d'extraction des validateurs, mais leur fonctionnement sont identiques donc on garde la meme

                Prophecy prophecy = new Prophecy(sentence,entityTypes,validators, updators);

                //Rangement de la prophetie

                string theme1 = cells[0];
                string theme2 = cells[1];

                int index1 = Array.IndexOf(themesArray, theme1);
                int index2 = Array.IndexOf( themesArray, theme2);

                prophecyMasterTable[index1, index2] = prophecy;
                prophecyMasterTable[index2, index1] = prophecy;

            }
        }

    }
    /// <summary>
    /// Cette fonction renvoie un tableau de string si le rawdata est valide, null sinon
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    string[] RawDataInitializationChecks(string filePath)
    {
        // Vérifie que le fichier existe
        if (!File.Exists(prophecyFilePath))
        {
            Debug.LogError($"Fichier non trouvé : {prophecyFilePath}");
            return null;
        }

        else
        {
            // Lire toutes les lignes du fichier
            string[] lines = File.ReadAllLines(prophecyFilePath);

            // Vérifier si le fichier contient au moins une ligne (l'en-tête)
            if (lines.Length < 1)
            {
                Debug.LogError("Le fichier CSV est vide !");
                return null;
            }
            else { return lines; }

        }

    }

    Type[] ExtractProphecyTypes(string cell)
    {                // Séparer les noms par la virgule
        string[] classNames = cell.Split(',');
        // Créer un tableau pour stocker les types
        Type[] requiredTypes = new Type[classNames.Length];
        for (int k = 0; k < classNames.Length; k++)
        {
            //string className = classNames[i].Trim(); // Supprimer les espaces inutiles
            requiredTypes[k] = Type.GetType(classNames[k]);
        }
        return requiredTypes;
    }

    List<(string, object)[]> ExtractProphecyValidators(string cell1, string cell2, string cell3)
    {

        List<string> allCells = new List<string>() { cell1,cell2,cell3};

        List<(string, object)[]> result = new List<(string, object)[]>() ;

        foreach (string cell in allCells)
        {
            // Séparer les blocs par les retours à la ligne
            string[] blocks = cell.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            List<(string, object)> validators = new List<(string, object)>();
            foreach (string block in blocks)
            {
                // Séparer les deux chaînes par la virgule
                string[] parts = block.Split(',');

                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    // Tenter de caster la valeur en float ou conserver en string
                    object castedValue;
                    if (float.TryParse(value, out float floatValue))
                    {
                        castedValue = floatValue;
                    }
                    else
                    {
                        castedValue = value;
                    }

                    // Ajouter le tuple dans la liste
                    validators.Add(  (key, castedValue) );
                }
                else
                {
                    Debug.LogWarning($"Bloc mal formé : {block}");
                }
            }
            result.Add(validators.ToArray());
        }

        return result;
    }
}
