using UnityEngine;
using System;
using System.Reflection;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

public class NarrationManagerLouis : MonoBehaviour
{
    System.Random random = new System.Random(); // instance pour les evenemnets aleatoires
    //Liste des thèmes de billes
    private string[] themesArray = new string[] {"Finances","Santé","Carrière","Nature","Amitié","Amour","Spiritualité"};
    
    //PROPHETIES
    //Tableau général des correspondances entre deux thèmes et leurs prophéties possibles
    [SerializeField] Prophecy[,] prophecyMasterTable = new Prophecy[1,1]; //initilaisé à 1,1 pour les tests

    //STORY ENTITIES
    //Perso principal
    MainCharacter MainCharacter;
    //Listes des types d'entités
    List<Character> allCharacters = new List<Character>();
    List<Place> allPlaces = new List<Place>();
    List<StoryActivity> allStoryActivities = new List<StoryActivity>();
    List<StoryItem> allStoryItems = new List<StoryItem>();
    
    // Design pattern du singleton
    private static NarrationManagerLouis instance; // instance statique du narration manager
  
    public static NarrationManagerLouis Instance
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
        // abonnement à l'evenement collisoin de billes
        BallRoll.TwoBallsCollision += TwoBallsCollisionNarration;
        // Abonnement à l'evenement de clic de la souris
        Controller.OnMouseClicked += CreateRandomStory;
    }

    void OnDisable()
    {
        // désbonnement à l'evenement collisoin de billes
        BallRoll.TwoBallsCollision -= TwoBallsCollisionNarration;
        //désabonnement de l'venement clic de souris, pour éviter les memory leaks
        Controller.OnMouseClicked -= CreateRandomStory;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //initialisation entity simple por test
        Character charTest = new Character(name: "sarah", mainCharacterBond: 60, health: 10);
        allCharacters.Add(charTest);
         charTest = new Character(name:"william",mainCharacterBond:60, health:10);
        allCharacters.Add(charTest);
         charTest = new Character(name: "henry", mainCharacterBond: 60, health: 60);
        allCharacters.Add(charTest);

        Place placeTest = new Place(name: "dublin", mainCharacterBond: 10, placeType: "City");
        allPlaces.Add(placeTest);
        placeTest = new Place(name: "Moher", mainCharacterBond: 60, placeType: "Nature");
        allPlaces.Add(placeTest);
        placeTest = new Place(name: "Galway", mainCharacterBond: 60, placeType: "City");
        allPlaces.Add(placeTest);

        //initialisation préphétie simple pour test
        string sentence = "connor va manger avec {0} près de {1}";
        Type[] types = new Type[] {typeof(Character), typeof(Place)};
        (string, object)[] valchar = new (string, object)[] { ("BondMin", 30),("HealthMin",30) };
        (string, object)[] valplace = new (string, object)[] { ("BondMin", 100),("PlaceTypeIs","City") };
        List<(string, object)[]> listval = new List<(string, object)[]>() { null,valplace };
        (string, object)[] upd = new (string, object)[] { ("BondPlus", 1) };
        List<(string, object)[]> listupd = new List<(string, object)[]>() { upd,null };
        Prophecy prophecy = new Prophecy(sentence, types, listval, listupd);
        prophecyMasterTable[0,0] = prophecy;
        CreateRandomStory(Vector3.zero);
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
        LegoProphecy legoProphecy = prophecyMasterTable[0, 0].GetCompletedProphecy();
        Debug.Log(legoProphecy.Sentence);
    }

    /// <summary>
    /// Va chercher une  entité qui correspond aux propriétés  fournies en argument 
    /// </summary>
    public StoryEntity GetFittingEntity(Type requiredType, (string, object)[] validators)
    {
        //Liste des entités du type demandé en argument renseigné sous type générique
        List<StoryEntity> possibleEntities = new List<StoryEntity>();
        //Selection de la liste de story entities correspodnant au type demandé
        if (requiredType == typeof(Character)) { possibleEntities = allCharacters.Cast<StoryEntity>().ToList(); }
        if (requiredType == typeof(Place)) { possibleEntities = allPlaces.Cast<StoryEntity>().ToList(); }
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
}
