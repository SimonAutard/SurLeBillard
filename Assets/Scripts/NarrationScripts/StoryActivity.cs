using UnityEngine;
using UnityEngine.TextCore.Text;
using System;

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
    public bool AssociatedPlacesComprise(string placeType) { if (Array.IndexOf(AssociatedPlaceTypes,placeType)>-1) return true; else return false; }
    public bool PopularityMin(float popularityMin) { if (popularityMin <= Popularity) return true; else return false; }
    public bool PopularityMax(float popularityMax) { if (popularityMax >= Popularity) return true; else return false; }

    //Methodes de changement des attributs de l'tentite pour leffet de la prophetie
    public void PopularityPlus(float popularityPlus) { Popularity = ValuePlus(Popularity, popularityPlus, MinPop, MaxPop); }
    public void ActivityTypeBecomes(string newActivityType) { ActivityType = newActivityType; }

}

