using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;


public class NarrationManager : MonoBehaviour
{
    System.Random random = new System.Random(); // instance pour les evenemnets aleatoires
    private List<UIProphecy> _lastProphecies = new List<UIProphecy>();
    private List<UIProphecy> _gameProphecies = new List<UIProphecy>();

    //Liste des th�mes de billes
    private string[] themesArray = new string[] { "Finances", "Sant�", "Carri�re", "Nature", "Amiti�", "Amour", "Spiritualit�" };

    //PROPHETIES
    //Tableau g�n�ral des correspondances entre deux th�mes et leurs proph�ties possibles
    [SerializeField] Prophecy[,] prophecyMasterTable;
    private string prophecyFilePath = "Assets/Scripts/RawData/NSData(PositiveProphecyRawData).csv";

    //STORY ENTITIES
    //Perso principal
    public MainCharacter MainCharacter { get; private set; }
    //Listes des types d'entit�s
    List<StoryCharacter> allCharacters = new List<StoryCharacter>();
    private string characterFilePath = "Assets/Scripts/RawData/NSData(StoryCharacterRawData).csv";
    List<StoryPlace> allPlaces = new List<StoryPlace>();
    private string placeFilePath = "Assets/Scripts/RawData/NSData(StoryPlaceRawData).csv";
    List<StoryActivity> allStoryActivities = new List<StoryActivity>();
    private string activityFilePath = "Assets/Scripts/RawData/NSData(StoryActivityRawData).csv";
    List<StoryItem> allStoryItems = new List<StoryItem>();
    private string itemFilePath = "Assets/Scripts/RawData/NSData(StoryItemRawData).csv";

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

    void OnEnable()
    {
        // subscribe to all events that this component needs to listen to at all time
        EventBus.Subscribe<EventProphecyGenerationRequest>(HandleProphecyGenerationRequest);
        EventBus.Subscribe<EventCollisionSignal>(HandleCollisionSignal);
        EventBus.Subscribe<EventNewGameSetupRequest>(HandleNewGameSetupRequest);
        EventBus.Subscribe<EventNextPlayerTurnStartRequest>(HandleNextPlayerTurnStartRequest);

        // abonnement à l'evenement collisoin de billes
        BallRoll.TwoBallsCollision += TwoBallsCollisionNarration;
        // Abonnement à l'evenement de clic de la souris
        Controller.OnMouseClicked += CreateRandomStory;
    }

    void OnDisable()
    {
        // Unsubscribe from all events to avoid memory leaks
        EventBus.Unsubscribe<EventProphecyGenerationRequest>(HandleProphecyGenerationRequest);
        EventBus.Unsubscribe<EventCollisionSignal>(HandleCollisionSignal);
        EventBus.Unsubscribe<EventNewGameSetupRequest>(HandleNewGameSetupRequest);
        EventBus.Unsubscribe<EventNextPlayerTurnStartRequest>(HandleNextPlayerTurnStartRequest);

        // d�sbonnement � l'evenement collisoin de billes
        BallRoll.TwoBallsCollision -= TwoBallsCollisionNarration;
        //d�sabonnement de l'venement clic de souris, pour �viter les memory leaks
        Controller.OnMouseClicked -= CreateRandomStory;
    }

    private void OnDestroy()
    {
        // Unsubscribe from all events to avoid memory leaks
        EventBus.Unsubscribe<EventProphecyGenerationRequest>(HandleProphecyGenerationRequest);
        EventBus.Unsubscribe<EventCollisionSignal>(HandleCollisionSignal);
        EventBus.Unsubscribe<EventNewGameSetupRequest>(HandleNewGameSetupRequest);
        EventBus.Unsubscribe<EventNextPlayerTurnStartRequest>(HandleNextPlayerTurnStartRequest);

        // d�sbonnement � l'evenement collisoin de billes
        BallRoll.TwoBallsCollision -= TwoBallsCollisionNarration;
        //d�sabonnement de l'venement clic de souris, pour �viter les memory leaks
        Controller.OnMouseClicked -= CreateRandomStory;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeProphecies();
        InitializeStoryCharacters();
        InitializeStoryPlaces();
        InitializeStoryItems();
        InitializeStoryActivities();
        InitializeMainCharacter();
        /*
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

        //initialisation pr�ph�tie simple pour test
        string sentence = "connor va manger avec {0} pr�s de {1}";
        Type[] types = new Type[] { typeof(StoryCharacter), typeof(StoryPlace) };
        (string, object)[] valchar = new (string, object)[] { ("BondMin", 30), ("HealthMin", 30) };
        (string, object)[] valplace = new (string, object)[] { ("BondMin", 30), ("PlaceTypeIs", "City") };
        List<(string, object)[]> listval = new List<(string, object)[]>() { valchar, valplace };
        (string, object)[] updChar = new (string, object)[] { ("BondPlus", 1), ("HealthPlus", 1) };
        (string, object)[] updPlace = new (string, object)[] { ("BondPlus", -100), ("StatePlus", 100) };


        List<(string, object)[]> listupd = new List<(string, object)[]>() { updChar, updPlace };
        Prophecy prophecy = new Prophecy(sentence, types, listval, listupd);
        prophecyMasterTable[0, 0] = prophecy;
        CreateRandomStory(Vector3.zero);*/
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
        Debug.Log(" prophecy for " + themesArray[index1] + " " + themesArray[index2]);
        if (index1 == index2) { index2--; }
        LegoProphecy legoProphecy = prophecyMasterTable[index1, index2].GetCompletedProphecy();
        Debug.Log(legoProphecy.Sentence);

        DebugLogEntitiesState(legoProphecy,"initial");
        UpdateStoryEntitiesFromProphecy(legoProphecy.StoryEntities, prophecyMasterTable[index1, index2].ProphecyUpdators);
        DebugLogEntitiesState(legoProphecy,"final");
    }

    private void HandleCollisionSignal(EventCollisionSignal collision)
    {
        // TODO :
        //  - generate prophecy based on collision
        //  - add the prophecy to _lastProphecies

        UIProphecy placeholderProphecy;
        placeholderProphecy._fastestBall = "ball1";
        placeholderProphecy._slowestBall = "ball2";
        placeholderProphecy._positive = false;
        placeholderProphecy._prophecy = "Connor fera la teuf et finira vraiment pas bien";
        _gameProphecies.Add(placeholderProphecy);
        _lastProphecies.Add(placeholderProphecy);

        // reset _lastProphecies
    }

    /// <summary>
    /// Handles EventStoryUpdateRequest from the reception of the ev ent to the publishing of the delivery
    /// </summary>
    /// <param name="requestEvent">The event containing the data relative to what type of event is requested</param>
    private void HandleProphecyGenerationRequest(EventProphecyGenerationRequest requestEvent)
    {
        Debug.Log($"NarrationManager: Fetching turn collisions.");
        List<Tuple<int, int, bool>> collisions = GameStateManager.Instance.LastCollisions();
        foreach (Tuple<int, int, bool> collision in collisions)
        {
            string pos;
            if (collision.Item3 == true)
            {
                pos = "positive";
            }
            else
            {
                pos = "negative";
            }
            Debug.Log($"NarrationManager: Generating a {pos} prophecy based on ball n°{collision.Item1.ToString()} and ball n°{collision.Item2.ToString()}.");
            // TODO: Generate prophecy and store it (with the associated themes used for generation and wether it's positive or not) so it can be easily accessed by the UIManager.
            // Maybe keep a list refering to prophecies generated in the last turn
        }
        Debug.Log("NarrationManager: calling NextStep");
        EventBus.Publish(new EventGameloopNextStepRequest());
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
    /// Va chercher une  entit� qui correspond aux propri�t�s  fournies en argument 
    /// </summary>
    public StoryEntity GetFittingEntity(Type requiredType, (string, object)[] validators)
    {
        //Liste des entit�s du type demand� en argument renseign� sous type g�n�rique
        List<StoryEntity> possibleEntities = new List<StoryEntity>();
        //Selection de la liste de story entities correspodnant au type demand�
        if (requiredType == typeof(StoryCharacter)) { possibleEntities = allCharacters.Cast<StoryEntity>().ToList(); }
        if (requiredType == typeof(StoryPlace)) { possibleEntities = allPlaces.Cast<StoryEntity>().ToList(); }
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

    /// <summary>
    /// returns prophecies that have been generated since the start of the game
    /// </summary>
    /// <returns></returns>
    public List<UIProphecy> GameProphecies()
    {
        return _gameProphecies;
    }

    /// <summary>
    /// returns prophecies that have been generated during the previous shot
    /// </summary>
    /// <returns></returns>
    public List<UIProphecy> LastTurnProphecies()
    {
        return _lastProphecies;
    }

    /// <summary>
    /// returns the most recently generated prophecy
    /// </summary>
    /// <returns></returns>
    public UIProphecy LastProphecy()
    {
        return _gameProphecies[_gameProphecies.Count - 1];
    }

    /// <summary>
    /// Things to setup at the start of a new game
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleNewGameSetupRequest(EventNewGameSetupRequest requestEvent)
    {
        _gameProphecies.Clear();
    }

    /// <summary>
    /// Things to setup at the start of new turn
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleNextPlayerTurnStartRequest(EventNextPlayerTurnStartRequest requestEvent)
    {
        _lastProphecies.Clear();
    }

    /// <summary>
    /// Change l�tat des story entities du jeu en fonction du r�sultat de la proph�tie
    /// </summary>
    /// <param name="gameEntityList"></param>
    public void UpdateStoryEntitiesFromProphecy(StoryEntity[] gameEntityList, List<(string, object)[]> updators)
    {
        //On change l'etat des entites les unes apres les autres
        for (int i = 0; i < gameEntityList.Length; i++)
        {
            //Si une entite na pas de updators, cest quon na pas besoin de la changer
            if (updators[i] == null) { continue; }
            foreach ((string, object) updator in updators[i])
            {
                string methodName = updator.Item1; //nom de la fonction de'update de lentite
                object methodValue = updator.Item2; // valeur associee

                // Trouver la m�thode par son nom
                MethodInfo method = gameEntityList[i].GetType().GetMethod(methodName);
                //Verification de l'existence de la m�thode demand�e
                if (method != null)
                {
                    // Appeler la m�thode en passant les param�tres
                    method.Invoke(gameEntityList[i], new object[1] { methodValue });
                }
                else { throw new Exception("M�thode introuvable : " + methodName); }
            }
        }
    }
    /// <summary>
    /// Cette fonction r�cupere les donnees csv de propheties et les convertit en instances de propheties
    /// </summary>
    void InitializeProphecies()
    {
        //Recuperation du csv sous forme de string[]
        string[] lines = RawDataInitializationChecks(prophecyFilePath);
        if (lines != null)
        {
            //Initialisation de la prophecyMasterTable
            prophecyMasterTable = new Prophecy[themesArray.Length, themesArray.Length];
            // Traitement des lignes restantes (donn�es)
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] cells = line.Split(';');

                //Coordonnees de la prophetie dans la prophecyMasterTable
                string theme1 = cells[0];
                string theme2 = cells[1];
                int index1 = Array.IndexOf(themesArray, theme1);
                int index2 = Array.IndexOf(themesArray, theme2);

                string sentence = cells[2];

                Type[] entityTypes = ExtractProphecyTypes(cells[3]);

                //INSERER ICI RECUPERATION DES INFOS DE SELF CONDITION

                List<(string, object)[]> validators = ExtractProphecyValidators(cells[5], cells[6], cells[7]);

                //INSERER ICI RECUPERATION DES INFOS DE SELF EFFECT

                List<(string, object)[]> updators = ExtractProphecyValidators(cells[9], cells[10], cells[11]); //Normalement la fonction devrait etre differetnte de celle d'extraction des validateurs, mais leur fonctionnement sont identiques donc on garde la meme

                Prophecy prophecy = new Prophecy(sentence, entityTypes, validators, updators);

                // On renseigne la prophetie aux deux intersections du tableau double entree pour la retrouver facilement
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
        // V�rifie que le fichier existe
        if (!File.Exists(filePath))
        {
            Debug.LogError($"Fichier non trouv� : {prophecyFilePath}");
            return null;
        }
        // On continue si le fichier existe
        else
        {
            //Extraction
            string[] lines;
            //On pr�cise la methode d'encodage pour garder les accents
            using (StreamReader reader = new StreamReader(filePath, System.Text.Encoding.GetEncoding("ISO-8859-1")))
            {
                lines = reader
                    .ReadToEnd()                       // Lire tout le contenu
                    .Split(Environment.NewLine)        // S�parer par les sauts de ligne
                    .Where(line =>                     // Filtrer les lignes non vides
                        !string.IsNullOrWhiteSpace(line) &&  // Pas vide ni espaces
                        line.Trim(';').Length > 0)          // Pas seulement des points-virgules
                    .ToArray();                        // Convertir en tableau
            }

            // V�rifier si le fichier contient au moins une ligne (l'en-t�te)
            if (lines.Length < 1)
            {
                Debug.LogError("Le fichier CSV " + filePath + " est vide !");
                return null;
            }
            else { return lines; }

        }

    }

    Type[] ExtractProphecyTypes(string cell)
    {
        // S�parer les noms par la virgule
        string[] classNames = cell.Split(',');
        // Cr�er un tableau pour stocker les types
        Type[] requiredTypes = new Type[classNames.Length];
        for (int k = 0; k < classNames.Length; k++)
        {
            //string className = classNames[i].Trim(); // Supprimer les espaces inutiles
            requiredTypes[k] = Type.GetType(classNames[k]);
        }
        return requiredTypes;
    }

    /// <summary>
    /// Transforme les 3 cellules d'une ligne du csv prophetie en List<(string,object)[]> comprehensible par le constructeur de prophetie
    /// </summary>
    /// <param name="cell1">validateurs de la 1ere entite</param>
    /// <param name="cell2">validateurs de la 2eme entite</param>
    /// <param name="cell3">validateurs de la 3eme entite</param>
    /// <returns></returns>
    List<(string, object)[]> ExtractProphecyValidators(string cell1, string cell2, string cell3)
    {
        //Concatenation des cellules de valdiation des 3 entites de la prophetie
        List<string> allCells = new List<string>() { cell1, cell2, cell3 };
        //Liste vide accueillant les valdiateurs
        List<(string, object)[]> result = new List<(string, object)[]>();
        //On parcourt chaque cellule de la ligne csv
        foreach (string cell in allCells)
        {
            //Cas sans validator
            if (cell == "")
            {
                result.Add(null); // le constructeur de prophetie se debrouille meme sans validateur
                continue;
            }

            // S�parer les criteres par les espace sdans la cellule
            string[] blocks = cell.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            // Initialisation du validateur de la cellule
            List<(string, object)> validators = new List<(string, object)>();
            //On parcourt chaque critere du validateur
            foreach (string block in blocks)
            {
                // S�parer les deux cha�nes par la virgule
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
                    validators.Add((key, castedValue));
                }
                else
                {
                    Debug.LogWarning($"Validateur mal �crit : {block} dans" + cell);
                }
            }
            //On assemble tous les criteres pour former le validateur associe a cette cellule
            result.Add(validators.ToArray());
        }

        return result;
    }

    /**
     * FONCTIONS DE CONVERSION DU CSV EN DIFFERETNES STORY ENTITIES DU JEU
     * */
    private void InitializeStoryCharacters()
    {
        //Extraction du csv en string
        string[] lines = RawDataInitializationChecks(characterFilePath);
        if (lines != null)
        {
            //lecture de chaque ligne du csv
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                //separation de la ligne en cellules
                string[] cells = line.Split(';');
                //cr�ation de l'entit�
                StoryCharacter entity = new StoryCharacter(cells[0], int.Parse(cells[1]), cells[2], int.Parse(cells[3]), int.Parse(cells[4]));
                //ajout de lentite a la liste qui lui correspond
                allCharacters.Add(entity);

            }
        }
    }
    private void InitializeStoryPlaces()
    {
        string[] lines = RawDataInitializationChecks(placeFilePath);
        if (lines != null)
        {
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] cells = line.Split(';');

                StoryPlace entity = new StoryPlace(cells[0], float.Parse(cells[1]), cells[2], float.Parse(cells[3]));

                allPlaces.Add(entity);
            }
        }
    }
    private void InitializeStoryItems()
    {
        string[] lines = RawDataInitializationChecks(itemFilePath);
        if (lines != null)
        {
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] cells = line.Split(';');

                StoryItem entity = new StoryItem(cells[0], int.Parse(cells[1]), cells[2], int.Parse(cells[3]));

                allStoryItems.Add(entity);
            }
        }
    }
    private void InitializeStoryActivities()
    {
        string[] lines = RawDataInitializationChecks(activityFilePath);
        if (lines != null)
        {
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] cells = line.Split(';');

                StoryActivity entity = new StoryActivity(cells[0], int.Parse(cells[1]), cells[2], cells[3].Split(','), int.Parse(cells[4]));

                allStoryActivities.Add(entity);
            }
        }
    }
    private void InitializeMainCharacter()
    {
        (string, object)[] placeValidator = { ("NameIs", "Galway") };
        StoryPlace livingPlace = (StoryPlace)GetFittingEntity(typeof(StoryPlace), placeValidator);

        (string, object)[] characterValidator = { ("NameIs", "Harold") };
        StoryCharacter boss = (StoryCharacter)GetFittingEntity(typeof(StoryCharacter), characterValidator);

        (string, object)[] activityValidator = { ("NameIs", "tondre des moutons") };
        StoryActivity job = (StoryActivity)GetFittingEntity(typeof(StoryActivity), activityValidator);

        MainCharacter = new MainCharacter("Connor", livingPlace, job, boss, 30, 30);
    }
    private void DebugLogEntitiesState(LegoProphecy legoProphecy,string stateName)
    {
        foreach (StoryEntity entity in legoProphecy.StoryEntities)
        {
            if (entity is StoryCharacter storyCharacter)
            {
                Debug.Log("etat "+stateName + " de "+storyCharacter.Name +
                    ": bond = " + storyCharacter.MainCharacterBond +
                    "; Money = " + storyCharacter.Money +
                    "; health = " + storyCharacter.Health);

            }
            else if (entity is StoryPlace storyPlace)
            {
                Debug.Log("etat " + stateName + " de " + storyPlace.Name +
                    ": bond = " + storyPlace.MainCharacterBond +
                    "; type = " + storyPlace.PlaceType +
                    "; state = " + storyPlace.State);

            }
            else if (entity is StoryItem storyItem)
            {
                Debug.Log("etat " + stateName + " de " + storyItem.Name +
                    ": bond = " + storyItem.MainCharacterBond +
                    "; type = " + storyItem.ItemType +
                    "; state = " + storyItem.State);

            }
            else if (entity is StoryActivity storyActivity)
            {
                Debug.Log("etat " + stateName + " de " + storyActivity.Name +
                    " : bond = " + storyActivity.MainCharacterBond +
                    "; type = " + storyActivity.ActivityType +
                    "; popularity = " + storyActivity.Popularity);

            }
        }
    }
}
