using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using System.Globalization;
//using Unity.Android.Gradle;


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
    MainCharacter MainCharacter;
    //Listes des types d'entit�s
    List<StoryCharacter> allCharacters = new List<StoryCharacter>();
    private string characterFilePath = "Assets/Scripts/RawData/characterdata.csv";
    List<StoryPlace> allPlaces = new List<StoryPlace>();
    private string placeFilePath = "Assets/Scripts/RawData/placedata.csv";
    List<StoryActivity> allStoryActivities = new List<StoryActivity>();
    private string activityFilePath = "Assets/Scripts/RawData/activitydata.csv";
    List<StoryItem> allStoryItems = new List<StoryItem>();
    private string itemFilePath = "Assets/Scripts/RawData/itemdata.csv";

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
        Prophecy prophecy = prophecyMasterTable[1, 0];
        Debug.Log(prophecy.SentenceToFill+" "+
            (prophecy.ProphecyStoryEntityTypes[1]==typeof(StoryPlace)) +" "+
            prophecy.ProphecyValidators[0][0].ToString()+" "+
            prophecy.ProphecyUpdators[0][0].ToString());


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
        if (index1 == index2) { index2--; }
        LegoProphecy legoProphecy = prophecyMasterTable[0, 0].GetCompletedProphecy();
        Debug.Log(legoProphecy.Sentence);
        Debug.Log("etat initial de " + legoProphecy.StoryEntities[0].Name + " - bond = " + legoProphecy.StoryEntities[0].MainCharacterBond + " - health = " + ((StoryCharacter)legoProphecy.StoryEntities[0]).Health);
        Debug.Log("etat initial de " + legoProphecy.StoryEntities[1].Name + " - bond = " + legoProphecy.StoryEntities[1].MainCharacterBond + " - state = " + ((StoryPlace)legoProphecy.StoryEntities[1]).State);
        UpdateStoryEntitiesFromProphecy(legoProphecy.StoryEntities, prophecyMasterTable[0, 0].ProphecyUpdators);
        Debug.Log("etat final de " + legoProphecy.StoryEntities[0].Name + " - bond = " + legoProphecy.StoryEntities[0].MainCharacterBond + " - health = " + ((StoryCharacter)legoProphecy.StoryEntities[0]).Health);
        Debug.Log("etat final de " + legoProphecy.StoryEntities[1].Name + " - bond = " + legoProphecy.StoryEntities[1].MainCharacterBond + " - state = " + ((StoryPlace)legoProphecy.StoryEntities[1]).State);
    }

    private void HandleCollisionSignal(EventCollisionSignal collision)
    {
        int whiteBallID = GameStateManager.Instance.whiteBallID;
        if (collision._fastestBall != whiteBallID && collision._slowestBall != whiteBallID)
        {
            UIProphecy prophecy;
            prophecy._fastestBall = themesArray[collision._fastestBall];
            prophecy._slowestBall = themesArray[collision._slowestBall];
            // TODO: change to collision._isPositive or whatever when it'll be in the event
            prophecy._positive = false;
            // TODO: change to a call to the appropriate method when it's done
            prophecy._prophecy = "Connor fera la teuf et finira vraiment pas bien (THIS IS PLACEHOLDER)";
            _gameProphecies.Add(prophecy);
            _lastProphecies.Add(prophecy);
        }
    }

    /// <summary>
    /// !!!WARNING!!! This isn't used anymore. The step publishing the event listened by this is skipped since the prophecies are now generated at the time of the collision
    /// Delete this when we're sure that it won't be needed ever again
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
    private void HandleNewGameSetupRequest (EventNewGameSetupRequest requestEvent)
    {
        _gameProphecies.Clear();
    }

    /// <summary>
    /// Things to setup at the start of new turn
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleNextPlayerTurnStartRequest (EventNextPlayerTurnStartRequest requestEvent)
    {
        _lastProphecies.Clear();
    }

    /// <summary>
    /// Change l�tat des story entities du jeu en fonction du r�sultat de la proph�tie
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
        string[] lines = RawDataInitializationChecks(prophecyFilePath);
        if (lines != null)
        {
            prophecyMasterTable = new Prophecy[themesArray.Length,themesArray.Length];
            // Traitement des lignes restantes (donn�es)
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] cells = line.Split(';');

                //Rangement de la prophetie
                string theme1 = cells[0];
                string theme2 = cells[1];

                // Instanciation de la prophetie
                string sentence = cells[2];

                Type[] entityTypes = ExtractProphecyTypes(cells[3]);

                //INSERER ICI RECUPERATION DES INFOS DE SELF CONDITION

                List<(string, object)[]> validators = ExtractProphecyValidators(cells[5], cells[6], cells[7]);

                List<(string, object)[]> updators = ExtractProphecyValidators(cells[9], cells[10], cells[11]); //Normalement la fonction devrait etre differetnte de celle d'extraction des validateurs, mais leur fonctionnement sont identiques donc on garde la meme

                Prophecy prophecy = new Prophecy(sentence,entityTypes,validators, updators);

                

                int index1 = Array.IndexOf(themesArray, theme1);
                int index2 = Array.IndexOf( themesArray, theme2);
                Debug.Log(i + " " + index1 + " " + index2);
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
        if (!File.Exists(prophecyFilePath))
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
            using (StreamReader reader = new StreamReader(prophecyFilePath, System.Text.Encoding.GetEncoding("ISO-8859-1")))
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
                Debug.LogError("Le fichier CSV est vide !");
                return null;
            }
            else { return lines; }

        }

    }

    Type[] ExtractProphecyTypes(string cell)
    {                // S�parer les noms par la virgule
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

    List<(string, object)[]> ExtractProphecyValidators(string cell1, string cell2, string cell3)
    {

        List<string> allCells = new List<string>() { cell1,cell2,cell3};

        List<(string, object)[]> result = new List<(string, object)[]>() ;

        foreach (string cell in allCells)
        {
            //Cas sans validator
            if (cell=="") { 
                result.Add(null);
                continue;
            }

            // S�parer les blocs par les retours � la ligne
            string[] blocks = cell.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<(string, object)> validators = new List<(string, object)>();
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
                    validators.Add(  (key, castedValue) );
                }
                else
                {
                    Debug.LogWarning($"Bloc mal form� : {block}");
                }
            }
            result.Add(validators.ToArray());
        }

        return result;
    }
}
