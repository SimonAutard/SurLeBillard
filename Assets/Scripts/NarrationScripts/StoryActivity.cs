using System;
using System.Linq;

public class StoryActivity : StoryEntity
{
    public string ActivityType { get; private set; }
    public string[] AssociatedPlaceTypes { get; private set; }
    public float Popularity { get; private set; }
    private float MinPop = 0;
    private float MaxPop = 100;
    public StoryActivity(string name = "generic activity", float mainCharacterBond = 50, string activityType = "generic type", string[] associatedPlaces = null, float popularity = 50)
    {
        Name = name;
        MainCharacterBond = mainCharacterBond;
        ActivityType = activityType;
        AssociatedPlaceTypes = associatedPlaces ?? new string[] { "generic place" };
        Popularity = popularity;
    }

    //Methodes de validation des criteres de l'entite pour la prophetie
    public bool ActivityTypeIs(string type) { if (ActivityType == type) return true; else return false; }
    public bool AssociatedPlacesComprise(string placeType) { if (AssociatedPlaceTypes.Contains(placeType)) return true; else return false; }
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

}

