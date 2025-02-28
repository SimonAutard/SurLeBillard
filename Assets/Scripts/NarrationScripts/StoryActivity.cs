using System.Collections.Generic;
using System.Linq;

public class StoryActivity : StoryEntity
{
    public string ActivityType { get; private set; } = "Leisure";
    public List<string> AssociatedPlaceTypes { get; private set; } = new List<string>() { "Nature" };
    public float Popularity { get; private set; } = 50;
    private float MinPop = 0;
    private float MaxPop = 100;

    //Constructeur naturel
    public StoryActivity(string name, float mainCharacterBond, string activityType, string[] associatedPlaces, float popularity) : base(name, mainCharacterBond)
    {
        Name = name;
        MainCharacterBond = mainCharacterBond;
        ActivityType = activityType;
        AssociatedPlaceTypes = associatedPlaces.ToList();
        Popularity = popularity;
    }

    //Constructeur par validateur
    public StoryActivity((string, object)[] validators) : base(validators)
    {
        Name = "doing some activity";
        foreach (var validator in validators)
        {
            string methodName = validator.Item1; //nom de la fonction de check de lentite
            object methodValue = validator.Item2; // valeur associee
            if (methodName == "ActivityTypeIs")
            {
                ActivityType = (string)methodValue;
                if (ActivityType == "Leisure") Name = "having fun";
                if (ActivityType == "Art") Name = "making art";
                if (ActivityType == "Sport") Name = "doing sport";
                if (ActivityType == "Job") Name = "working";
            }
            if (methodName == "AssociatedPlacesInclude")
            {
                ChangeAssociatedPlace((string)methodValue, true);
                if ((string)methodValue == "Nature") Name = "doing outdoor activities";
                if ((string)methodValue == "City") Name = "doing urban activities";
            }
            if (methodName == "AssociatedPlacesExclude") { ChangeAssociatedPlace((string)methodValue, false); }
            if (methodName == "PopularityMin") { Popularity = (float)methodValue; }
            if (methodName == "PopularityMax") { Popularity = (float)methodValue; }
            if (methodName == "IsJob") { if ((float)methodValue == 1) { BecomesJob(1); Name = "doing his job"; } }
        }
    }

    //Methodes de validation des criteres de l'entite pour la prophetie
    public bool ActivityTypeIs(string type) { if (ActivityType == type) return true; else return false; }
    public bool AssociatedPlacesInclude(string placeType) { if (AssociatedPlaceTypes.Contains(placeType)) return true; else return false; }
    public bool AssociatedPlacesExclude(string placeType) { if (AssociatedPlaceTypes.Contains(placeType)) return false; else return true; }
    public bool PopularityMin(float popularityMin) { if (popularityMin <= Popularity) return true; else return false; }
    public bool PopularityMax(float popularityMax) { if (popularityMax >= Popularity) return true; else return false; }
    public bool IsJob(float boolean) // Si boolean = 1, on demande si le cette activite est le job du MC. Sinon, on demande si ce n'est pas le job du MC. 
    {
        if (NarrationManager.Instance.MainCharacter.Job != null && this == NarrationManager.Instance.MainCharacter.Job)
        {
            return (boolean == 1) ? true : false;
        }
        else return (boolean == 1) ? false : true;
    }

    //Methodes de changement des attributs de l'tentite pour leffet de la prophetie
    public void PopularityPlus(float popularityPlus) { Popularity = ValuePlus(Popularity, popularityPlus, MinPop, MaxPop); }
    public void ActivityTypeBecomes(string newActivityType) { ActivityType = newActivityType; }
    public void BecomesJob(float boolean) { NarrationManager.Instance.MainCharacter.JobBecomes(boolean == 1 ? this : null); }
    private void ChangeAssociatedPlace(string placeType, bool include)
    {
        if (include && AssociatedPlaceTypes.Contains(placeType)) { }
        if (include && !AssociatedPlaceTypes.Contains(placeType)) { AssociatedPlaceTypes.Add(placeType); }
        if (!include && AssociatedPlaceTypes.Contains(placeType)) { AssociatedPlaceTypes.Remove(placeType); }
        if (!include && !AssociatedPlaceTypes.Contains(placeType)) { }
    }

}

